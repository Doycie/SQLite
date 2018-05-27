using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SQLite
{
    public class SQLite
    {
        private SQLiteConnection dbconnection;

        private const string metadatastring = "data/metadata.sqlite";
        private const string cardatabasestring = "data/CarDatabase.sqlite";
        private const string fileToLoadFrom = "data/autompg.sql";
        private const string workloadFile = "data/workload.txt";
        private string[] CatogoricalValues = { "cylinders", "model_year", "origin", "brand", "model", "type" };
        private string[] NumericValues = { "mpg", "displacement", "horsepower", "weight", "acceleration" };
        private string[] AllValues = { "id", "mpg", "cylinders", "displacement", "horsepower", "weight", "acceleration", "model_year", "origin", "brand", "model", "type" };

        public void readDatabase(string tableName)
        {
            OpenConnection(metadatastring);

            // Open table
            string sql = "select * from " + tableName;
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();

            bool attsim = false;
            // Extract  attribute name from table name and add correct valuetype
            string attributeName = "";
            string column1 = "";
            if (tableName.EndsWith("_Occurence"))
            {
                attributeName = tableName.Substring(0, tableName.Length - 10);
                column1 = "\tdocument id: ";
            }
            else if (tableName.EndsWith("Values"))
            {
                attributeName = tableName.Substring(0, tableName.Length - 15);
                column1 = "\ttimes ";
            }
            else if (tableName.EndsWith("Attsim"))
            {
                attsim = true;
            }
            else
            {
                attributeName = tableName;
                column1 = "\tidfqf: ";
            }
            if (!attsim)
                // Print all values
                while (reader.Read())
                    Console.WriteLine(attributeName + " value: " + reader[0] + column1 + reader[1]);
            else
            {
                while (reader.Read())
                    Console.WriteLine(attributeName + " value: " + reader[0] + " " + reader[1] + " " + reader[2]);
            }
            CloseConnection();
        }

        //Print all IDFQF attributes that are in the metadatabase
        public List<string> GetMetadataTables()
        {
            List<string> tables = new List<string>();
            string sql = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";

            OpenConnection(metadatastring);
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tables.Add((string)reader[0]);
            }

            CloseConnection();
            return tables;
        }

        //QF Values
        private Dictionary<Tuple<string, string>, int> qfoccurrences = new Dictionary<Tuple<string, string>, int>();

        private Dictionary<string, Dictionary<Tuple<string, string>, double>> attributesimilarity = new Dictionary<string, Dictionary<Tuple<string, string>, double>>();

        private int QFMax = 0;

        //QF functions
        public void MakeQFDictionary()
        {
            // [brand][opel] = [(2,65),...]
            Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> AttributeSimilarityLists = new Dictionary<string, Dictionary<string, List<Tuple<int, int>>>>();

            if (qfoccurrences.Count > 1)
            {
                Console.WriteLine("QF values have already been put in a dictionary");
                return;
            }
            StreamReader sr = new StreamReader(workloadFile);
            string input;

            // Skip first two lines
            input = sr.ReadLine();
            input = sr.ReadLine();

            int inTuple = 0;
            // Read all lines
            while ((input = sr.ReadLine()) != null)
            {
                // Splits on white spaces
                string[] inputSplit = input.Split();

                // Keep track of number of parsed words
                int position = 0;

                // First item is times searched
                int times = int.Parse(inputSplit[0]);

                // Skip till WHERE
                while (inputSplit[position] != "WHERE")
                    position++;

                // Read rest of line
                while (position < inputSplit.Length - 1)
                {
                    // Parsing changes when IN occures
                    if (inputSplit[position + 2] == "IN")
                    {
                        string typeofattribute = inputSplit[position + 1];
                        if (!AttributeSimilarityLists.ContainsKey(typeofattribute))
                        {
                            AttributeSimilarityLists[typeofattribute] = new Dictionary<string, List<Tuple<int, int>>>();
                        }

                        if (typeofattribute == "type")
                        {
                            int w = 0;
                        }

                        string cats = inputSplit[position + 3];
                        if (!cats.EndsWith(")"))
                            cats += " " + inputSplit[position + 4];
                        cats = cats.TrimStart('(');
                        cats = cats.TrimEnd(')');

                        string[] inputSplit2 = cats.Split(',');

                        foreach (var i in inputSplit2)
                        {
                            // Remove '' from start and end
                            string t = i.Substring(1, i.Length - 2);
                            // Add attribute, value pair to dictionary
                            if (!AttributeSimilarityLists[typeofattribute].ContainsKey(t))
                            {
                                AttributeSimilarityLists[typeofattribute][t] = new List<Tuple<int, int>>();
                                AttributeSimilarityLists[typeofattribute][t].Add(Tuple.Create(inTuple, times));
                            }
                            else
                            {
                                AttributeSimilarityLists[typeofattribute][t].Add(Tuple.Create(inTuple, times));
                            }
                        }
                        inTuple++;
                        position += 4;
                    }
                    else
                    {
                        // Remove '' from start and end
                        string t = inputSplit[position + 3].Substring(1, inputSplit[position + 3].Length - 2);

                        // Add attribute, value pair to dictionary
                        var key = Tuple.Create(inputSplit[position + 1], t);
                        if (qfoccurrences.ContainsKey(key))
                            qfoccurrences[key] += times;
                        else
                            qfoccurrences.Add(key, times);
                        position += 4;
                    }
                }
            }

            // [brand][opel] = [(2,65),...]
            foreach (var kvp in AttributeSimilarityLists)
            {
                attributesimilarity[kvp.Key] = new Dictionary<Tuple<string, string>, double>();
                foreach (var att in kvp.Value)
                {
                    foreach (var attTwo in kvp.Value)
                    {
                        if (!attributesimilarity[kvp.Key].ContainsKey(Tuple.Create(att.Key, attTwo.Key)) && att.Key != attTwo.Key)
                        {
                            int intersection = 0;
                            int overlap = 0;

                            int attCounter = 0;
                            int attTwoCounter = 0;

                            foreach (var p in att.Value)
                            {
                                overlap += p.Item2;
                            }
                            foreach (var p in attTwo.Value)
                            {
                                overlap += p.Item2;
                            }

                            while ((attCounter < att.Value.Count && attTwoCounter < attTwo.Value.Count))
                            {
                                if (att.Value[attCounter].Item1 == attTwo.Value[attTwoCounter].Item1)
                                {
                                    intersection += att.Value[attCounter].Item2;
                                    attCounter++; attTwoCounter++;
                                }
                                else if (att.Value[attCounter].Item1 < attTwo.Value[attTwoCounter].Item1)
                                {
                                    attCounter++;
                                }
                                else
                                {
                                    attTwoCounter++;
                                }
                            }
                            double sim = (double)intersection / (double)overlap;
                            if (sim != 0)
                                attributesimilarity[kvp.Key][Tuple.Create(att.Key, attTwo.Key)] = sim;
                        }
                    }
                }
            }

            // Get highest attribute value occurence
            int max = 0;
            foreach (int value in qfoccurrences.Values)
            {
                if (value > max)
                    max = value;
            }
            QFMax = max;
        }

        public void addCatergoricAttributeSimilarity(string attributeName)
        {
            OpenConnection(metadatastring);
            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", dbconnection);
            sqlComm.ExecuteNonQuery();

            ExecuteCommand("CREATE TABLE " + attributeName + "_Attsim" + " (" + "first" + " text, second text, idfqf real)");

            foreach (var kvp in attributesimilarity[attributeName])
            {
                ExecuteCommand("INSERT into " + attributeName + "_Attsim VALUES ( '" + kvp.Key.Item1 + "','" + kvp.Key.Item2 + "'," + kvp.Value + ");");
            }
            
            sqlComm = new SQLiteCommand("end", dbconnection);
            sqlComm.ExecuteNonQuery();
            CloseConnection();
        }

        public void PrintQFDictionary()
        {
            foreach (var kvp in qfoccurrences)
            {
                Console.WriteLine(kvp);
            }
        }

        //Create meta database and add attribute tables for IDFQF and Occurence in database
        public void fillMetaDB(System.Windows.Forms.ProgressBar ProgressMetadatabase)
        {
            // Create meta database
            CreateDatabaseFile(metadatastring);

            // Use visual progress bar
            ProgressMetadatabase.Maximum = CatogoricalValues.Length + NumericValues.Length;
            ProgressMetadatabase.Value = 1;

            // Add occurence to meta database
            FillMetaDBWithAttributeOccurence(ProgressMetadatabase);

            foreach (var kvp in attributesimilarity)
            {
                addCatergoricAttributeSimilarity(kvp.Key);
            }
            // Add categorical attributes
            foreach (var c in CatogoricalValues)
            {
                addCategoricIDFQFTable(c);
                ProgressMetadatabase.PerformStep();
            }

            // Add numeric attributes
            foreach (var n in NumericValues)
            {
                addNumericValuesTable(n);
                ProgressMetadatabase.PerformStep();
            }
        }

        // Add categoric IDF*QF values to meta database
        public void addCategoricIDFQFTable(string attributeName)
        {
            OpenConnection(cardatabasestring);

            double total = 0;
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            string sql = "select * from autompg";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string m = reader[attributeName].ToString();
                total++;
                if (occurrences.ContainsKey(m))
                    occurrences[m] += 1;
                else
                    occurrences.Add(m, 1);
            }

            CloseConnection();

            OpenConnection(metadatastring);

            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", dbconnection);
            sqlComm.ExecuteNonQuery();

            ExecuteCommand("CREATE TABLE " + attributeName + " (" + attributeName + " text, idfqf real)");

            foreach (KeyValuePair<string, int> vp in occurrences)
            {
                // pretend there is only 1 occurence when there are non
                float qf = 2 / (float)(QFMax + 1);
                Tuple<string, string> key = Tuple.Create(attributeName, vp.Key);
                if (qfoccurrences.ContainsKey(key))
                {
                    qf = (float)(qfoccurrences[key] + 1) / (float)(QFMax + 1);
                }

                ExecuteCommand("INSERT into " + attributeName + " VALUES ( '" + vp.Key + "'," + Math.Log(total / vp.Value) * qf + ");");
            }

            
            sqlComm = new SQLiteCommand("end", dbconnection);
            sqlComm.ExecuteNonQuery();

            CloseConnection();
        }

        // Add all numeric values to meta database
        public void addNumericValuesTable(string attributeName)
        {
            OpenConnection(cardatabasestring);




            // Keep track of every value t's number of occurrences
            Dictionary<double, int> uniqueValues = new Dictionary<double, int>();

            // Count occurrences
            string sql = "select * from autompg";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                double t = (double)reader[attributeName];

                // Existing t
                if (uniqueValues.ContainsKey(t))
                    uniqueValues[t] += 1;
                // New t
                else
                    uniqueValues.Add(t, 1);
            }

            // Open connection to meta database
            CloseConnection();
            OpenConnection(metadatastring);


            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", dbconnection );
            sqlComm.ExecuteNonQuery();
            // Add attribute value t and its number of occurrences in database to table
            ExecuteCommand("CREATE TABLE " + attributeName + "_databaseValues (t double, occurrences integer)");

            // Fill table
            foreach (var kv in uniqueValues)
            {
                ExecuteCommand("INSERT into " + attributeName + "_databaseValues VALUES (" + kv.Key + ", " + kv.Value + " );");
            }
            // Add attribute value t and its number of occurrences in workload to table
            ExecuteCommand("CREATE TABLE " + attributeName + "_workloadValues (t double, occurrences integer)");

            // Fill table
            foreach (var kv in qfoccurrences)
            {
                // Search for corresponding attribute name
                if (kv.Key.Item1.Equals(attributeName))
                {
                    ExecuteCommand("INSERT into " + attributeName + "_workloadValues VALUES (" + Convert.ToDouble(kv.Key.Item2) + ", " + kv.Value + ");");
                }
            }
           
            sqlComm = new SQLiteCommand("end", dbconnection);
            sqlComm.ExecuteNonQuery();

            CloseConnection();
        }

        // Calculate IDF or QF and bandwith for q
        public Tuple<double, double> numericSmooth(List<Tuple<double, int>> tValues, double qValue)
        {
            if(tValues.Count == 0)
            {
                return Tuple.Create(1.0, 0.0);
            }
            List<double> allTValues = new List<double>();

            // Add every value the correct amount of times
            foreach (var vk in tValues)
            {
                for (int i = 0; i < vk.Item2; i++)
                    allTValues.Add(vk.Item1);
            }

            // Get average
            double avg = allTValues.Average();

            // Calculate number of elements and standard deviation
            double sum = 0;
            foreach (double ti in allTValues)
                sum += Math.Pow(ti - avg, 2);

            int n = allTValues.Count;
            double sd = Math.Sqrt((sum) / n);

            // Calculate bandwith h
            double h = 1.06 * sd * Math.Pow(n, -0.2);

            sum = 0;
            foreach (double ti in allTValues)
                sum += Math.Pow(Math.E, -0.5 * Math.Pow((ti - qValue) / h, 2));

            // Add pair to dictionary
            return Tuple.Create(Math.Log(n / sum), h);
        }

        // Get attribute occurence from database and add as table to the meta database
        public void FillMetaDBWithAttributeOccurence(System.Windows.Forms.ProgressBar ProgressMetadatabase)
        {
            ProgressMetadatabase.Value = 1;

            // Open query database
            OpenConnection(cardatabasestring);

            // List with <Attribute, Attribute value, query id> tuples
            List<Tuple<string, string, int>> AttributeOccurence = new List<Tuple<string, string, int>>();

            // Loop over all queries
            string sql = "select * from autompg";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Add all query attributes to list
                foreach (string c in CatogoricalValues)
                    AttributeOccurence.Add(Tuple.Create(c, reader[c].ToString(), int.Parse(reader["id"].ToString())));
                foreach (string n in NumericValues)
                    AttributeOccurence.Add(Tuple.Create(n, reader[n].ToString(), int.Parse(reader["id"].ToString())));
            }

            // Open connection with metadatabase
            CloseConnection();
            OpenConnection(metadatastring);

            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", dbconnection);
            sqlComm.ExecuteNonQuery();

            // Create Occurence tables
            foreach (string c in CatogoricalValues)
            {
                ExecuteCommand("CREATE TABLE " + c + "_Occurence (" + c + " text, id integer)");
            }
            foreach (string n in NumericValues)
            {
                ExecuteCommand("CREATE TABLE " + n + "_Occurence (" + n + " double, id integer)");
            }
            ProgressMetadatabase.Maximum = AttributeOccurence.Count;
            ProgressMetadatabase.Minimum = 0;
            ProgressMetadatabase.Value = 0;

            // Fill Occurence tables with values from list
            foreach (var t in AttributeOccurence)
            {
                ExecuteCommand("INSERT into " + t.Item1 + "_Occurence" + " VALUES ( '" + t.Item2 + "', '" + t.Item3 + "' );");

                ProgressMetadatabase.PerformStep();
            }

            
            sqlComm = new SQLiteCommand("end", dbconnection);
            sqlComm.ExecuteNonQuery();

            CloseConnection();
        }

        //BuildDatabase from txt file
        public void BuildDatabase()
        {
            CreateDatabaseFile(cardatabasestring);
            OpenConnection(cardatabasestring);
            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", dbconnection);
            sqlComm.ExecuteNonQuery();
            StreamReader sr = new StreamReader(fileToLoadFrom);
            string input;
            while ((input = sr.ReadLine()) != null)
            {
                ExecuteCommand(input);
            }

            sqlComm = new SQLiteCommand("end", dbconnection);
            sqlComm.ExecuteNonQuery();
            CloseConnection();
        }

        public List<Tuple<int, double>> catigoricalSimilarityWithAttSim(string attributeName, string attributeValue)
        {
            OpenConnection(metadatastring);

            string sql;
            SQLiteDataReader reader;
            SQLiteCommand command;

            Dictionary<string, double> attsimdict;
            string inclause = "(";

            attsimdict = new Dictionary<string, double>();
            //get attributeSimilartiy to others
            sql = "select * from " + attributeName + "_Attsim WHERE first" + " == '" + attributeValue + "'";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                inclause = inclause + "'" + (reader["second"].ToString()) + "',";
                attsimdict.Add(reader["second"].ToString(), double.Parse((reader["idfqf"].ToString())));
            }

            inclause += "'" + attributeValue + "'";
            inclause += " )";

            // Get idf*qf value of given attribute value
            sql = "select * from " + attributeName + " WHERE " + attributeName + " == '" + attributeValue + "'";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();
            reader.Read();
            double idfqf = double.Parse((reader["idfqf"].ToString()));

            // <t value, occurrence> pairs from database
            List<Tuple<int, double>> similarity = new List<Tuple<int, double>>();

            // Get all documents containg the given attribute value
            sql = "select * from " + attributeName + "_Occurence WHERE " + attributeName + " IN " + inclause + ";";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();

            int expectedID = 1;
            List<int> missedID = new List<int>();
            while (reader.Read())
            {
                // Keep track of id's that will get sim 0
                while (int.Parse(reader["id"].ToString()) > expectedID)
                {
                    // ID 359 doesnt exist
                    if (expectedID != 359)
                        missedID.Add(expectedID);
                    expectedID++;
                }

                if (attsimdict.ContainsKey(reader[attributeName].ToString()))
                {
                    similarity.Add(Tuple.Create(expectedID, idfqf * attsimdict[reader[attributeName].ToString()]));
                }
                else
                {
                    similarity.Add(Tuple.Create(expectedID, idfqf));
                }
                expectedID++;
            }
            // Add missed ids with sim 0
            foreach (int id in missedID)
                similarity.Add(Tuple.Create(id, 0.0));

            // Add more possibly more ids with sim 0
            while (expectedID <= 396)
            {
                // Expected id still doesnt exist
                if (expectedID != 359)
                    similarity.Add(Tuple.Create(expectedID, 0.0));
                expectedID++;
            }

            CloseConnection();

            similarity.Sort((y, x) => x.Item2.CompareTo(y.Item2));
            // Return similarity
            return similarity;
        }

        // Get sorted S(t, q) for every document given a catigorical attribute value q
        public List<Tuple<int, double>> catigoricalSimilarity(string attributeName, string attributeValue)
        {
            OpenConnection(metadatastring);

            // Get idf*qf value of given attribute value
            string sql = "select * from " + attributeName + " WHERE " + attributeName + " == '" + attributeValue + "'";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            double idfqf = double.Parse((reader["idfqf"].ToString()));

            // <t value, occurrence> pairs from database
            List<Tuple<int, double>> similarity = new List<Tuple<int, double>>();

            // Get all documents containg the given attribute value
            sql = "select * from " + attributeName + "_Occurence WHERE " + attributeName + " == '" + attributeValue + "'";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();
            int expectedID = 1;
            List<int> missedID = new List<int>();
            while (reader.Read())
            {
                // Keep track of id's that will get sim 0
                while (int.Parse(reader["id"].ToString()) > expectedID)
                {
                    // ID 359 doesnt exist
                    if (expectedID != 359)
                        missedID.Add(expectedID);
                    expectedID++;
                }

                similarity.Add(Tuple.Create(expectedID, idfqf));

                expectedID++;
            }
            // Add missed ids with sim 0
            foreach (int id in missedID)
                similarity.Add(Tuple.Create(id, 0.0));

            // Add more possibly more ids with sim 0
            while (expectedID <= 396)
            {
                // Expected id still doesnt exist
                if (expectedID != 359)
                    similarity.Add(Tuple.Create(expectedID, 0.0));
                expectedID++;
            }

            CloseConnection();

            // Return similarity
            return similarity;
        }

        // Get sorted S(T, Q) for every document given a numeric attribute value q
        public List<Tuple<int, double>> numericSimilarity(string attributeName, double attributeValue)
        {
            OpenConnection(metadatastring);

            // <t value, occurrence> pairs from database
            List<Tuple<double, int>> databaseValues = new List<Tuple<double, int>>();

            // Add all values from table to dictionary
            string sql = "select * from " + attributeName + "_databaseValues";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                databaseValues.Add(Tuple.Create(double.Parse(reader["t"].ToString()), int.Parse((reader["occurrences"].ToString()))));

            // <t value, occurrence> pairs from workload
            List<Tuple<double, int>> workloadValues = new List<Tuple<double, int>>();

            // Add all values from table to dictionary
            sql = "select * from " + attributeName + "_workloadValues";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();
            while (reader.Read())
                workloadValues.Add(Tuple.Create(double.Parse(reader["t"].ToString()), int.Parse(reader["occurrences"].ToString())));

            // Calculate IDF, QF and bandwith for given value
            Tuple<double, double> idfh = numericSmooth(databaseValues, attributeValue);
            Tuple<double, double> qfh = numericSmooth(workloadValues, attributeValue);

            // Get IDFQF value and database bandwith
            double idfqf = idfh.Item1 * qfh.Item1;
            double h = idfh.Item2;

            // <id, similarity> pairs
            List<Tuple<int, double>> similarity = new List<Tuple<int, double>>();

            // Loop over every document with a value for the given attribute
            sql = "select * from " + attributeName + "_Occurence";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Documents without attribute get similarity 0
                int id = int.Parse(reader["id"].ToString());

                // Calculate similarity
                similarity.Add(Tuple.Create(id, Math.Pow(Math.E, -0.5 * Math.Pow((double.Parse(reader[attributeName].ToString()) - attributeValue) / h, 2)) * idfqf));
            }
            CloseConnection();

            // Sort on similarity and return
            similarity.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            return similarity;
        }

        //The topK search function
        public void topK(string search, System.Windows.Forms.DataGridView dgv, System.Windows.Forms.Label searchLabel, System.Windows.Forms.CheckBox sorttopkbox, System.Windows.Forms.CheckBox attsimbox)
        {
            bool skip = false;
            bool perfectSort = sorttopkbox.Checked;
            bool attsim = attsimbox.Checked;

            if (search == "")
            {
                Console.WriteLine("Invalid search empty string");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int k = 10;
            // test querietje      brand = 'ford', cylinders = 4, mpg = 18;

            // List containing every similarity between document attribute values and query attribute value, for every attribute in query
            List<List<Tuple<int, double>>> attributeSimilarities = new List<List<Tuple<int, double>>>();

            // Add whitespace and remove ; for easier parsing
            search = " " + search.Substring(0, search.Length - 1);

            // Get seperate attribute = value parts
            foreach (string attribute in search.Split(','))
            {
                // Get attribute name and value
                string attributeName = attribute.Split()[1];
                string attributeValue = attribute.Split()[3].Replace("\'", "");

                // Add similarity between attributeValue and every document to the list
                if (CatogoricalValues.Contains(attributeName))
                {
                    if (attsim && (attributeName == "brand" || attributeName == "type"))

                        attributeSimilarities.Add(catigoricalSimilarityWithAttSim(attributeName, attributeValue));
                    else
                        attributeSimilarities.Add(catigoricalSimilarity(attributeName, attributeValue));
                }
                else if (NumericValues.Contains(attributeName))
                    attributeSimilarities.Add(numericSimilarity(attributeName, double.Parse(attributeValue)));
                else if (attributeName == "k")
                    k = int.Parse(attributeValue);
                else
                    Console.WriteLine("'" + attributeName + "' is not a valid attribute");
            }

            if (attributeSimilarities.Count == 0)
            {
                Console.WriteLine("Invalid search not enough attributes");
                return;
            }

            // Create buffer and topk
            Dictionary<int, List<Tuple<bool, double>>> buffer = new Dictionary<int, List<Tuple<bool, double>>>();
            Dictionary<int, List<Tuple<bool, double>>> topk = new Dictionary<int, List<Tuple<bool, double>>>();

            // Loop over all rows
            for (int row = 0; row < attributeSimilarities[0].Count; row++)
            {
                // ??? iets anders dan met een hashset doen?
                // Keep track of values in this row
                List<Tuple<int, double>> rowAttributes = new List<Tuple<int, double>>();
                HashSet<int> uniqueOID = new HashSet<int>();
                decimal threshold = 0;

                // Loop over all attributes in this row
                for (int attribute = 0; attribute < attributeSimilarities.Count; attribute++)
                {
                    int OID = attributeSimilarities[attribute][row].Item1;
                    double similarity = attributeSimilarities[attribute][row].Item2;

                    // Add Attribute OID and similarity from current row
                    rowAttributes.Add(Tuple.Create(OID, similarity));
                    uniqueOID.Add(OID);

                    // Keep track of treshold
                    threshold += Convert.ToDecimal(similarity);
                }

                if (!skip)
                {
                    // update buffer
                    foreach (var OIDsim in buffer)
                    {
                        // Loop over all attribute similarities binnen item
                        for (int attribute = 0; attribute < OIDsim.Value.Count; attribute++)
                        {
                            // Update if correct similarity isnt found yet
                            if (!OIDsim.Value[attribute].Item1)
                            {
                                bool equalOID = false;

                                // Correct similarity is found
                                if (OIDsim.Key == rowAttributes[attribute].Item1)
                                    equalOID = true;

                                // Update attribute with new similarity
                                OIDsim.Value[attribute] = Tuple.Create(equalOID, rowAttributes[attribute].Item2);
                            }
                        }
                    }
                }

                // update topk
                foreach (var OIDsim in topk)
                {
                    // Loop over all attribute similarities binnen item
                    for (int attribute = 0; attribute < OIDsim.Value.Count; attribute++)
                    {
                        // Update if correct similarity isnt found yet
                        if (!OIDsim.Value[attribute].Item1)
                        {
                            bool equalOID = false;

                            // Correct similarity is found
                            if (OIDsim.Key == rowAttributes[attribute].Item1)
                                equalOID = true;

                            // Update attribute with new similarity
                            OIDsim.Value[attribute] = Tuple.Create(equalOID, rowAttributes[attribute].Item2);
                        }
                    }
                }

                if (!skip)
                {
                    // Add possible new OID sim to buffer
                    foreach (int OID in uniqueOID)
                    {
                        // Add OID if we dont have it already
                        if (!buffer.ContainsKey(OID) && !topk.ContainsKey(OID))
                        {
                            // Create OID value list
                            List<Tuple<bool, double>> OIDValue = new List<Tuple<bool, double>>();
                            foreach (var tuple in rowAttributes)
                            {
                                // Correct similarity for OID
                                if (OID == tuple.Item1)
                                    OIDValue.Add(Tuple.Create(true, tuple.Item2));
                                // Wrong similarity for OID
                                else
                                    OIDValue.Add(Tuple.Create(false, tuple.Item2));
                            }

                            // Add to buffer
                            buffer.Add(OID, OIDValue);
                        }
                    }
                }

                bool possibleTopk = false;
                if (!skip)
                {
                    List<int> removedOID = new List<int>();
                    // Move from buffer to topk
                    foreach (var OIDsim in buffer)
                    {
                        // Get lower and upper range for every item in buffer
                        decimal lowerRange = 0;
                        decimal upperRange = 0;
                        for (int attribute = 0; attribute < OIDsim.Value.Count; attribute++)
                        {
                            if (OIDsim.Value[attribute].Item1)
                                lowerRange += Convert.ToDecimal(OIDsim.Value[attribute].Item2);
                            upperRange += Convert.ToDecimal(OIDsim.Value[attribute].Item2);
                        }

                        // Add to topk if lower range is above threshold
                        if (lowerRange >= threshold)
                        {
                            topk.Add(OIDsim.Key, OIDsim.Value);
                            removedOID.Add(OIDsim.Key);
                        }

                        // If an upper range is over the threshold its possibly better
                        if (upperRange > threshold)
                            possibleTopk = true;
                    }

                    // Remove from buffer
                    foreach (int OID in removedOID)
                        buffer.Remove(OID);
                }

                if (skip)
                {
                    bool allSimilarities = true;
                    foreach (var OIDsim in topk)
                    {
                        // Check if all values attribute vallues are found
                        for (int attribute = 0; attribute < OIDsim.Value.Count; attribute++)
                        {
                            if (!OIDsim.Value[attribute].Item1)
                                allSimilarities = false;
                        }
                    }

                    // Stop if all values are found
                    if (allSimilarities)
                        break;
                }
                // Stop when k is reached and there arent any possibly better solotions
                else if (topk.Count >= k && !possibleTopk)
                {
                    if (perfectSort)
                        skip = true;
                    else
                        break;
                }
            }

            OpenConnection(cardatabasestring);

            sw.Stop();

            dgv.Rows.Clear();
            dgv.ColumnCount = AllValues.Length;
            for (int i = 0; i < AllValues.Length; i++)
            {
                dgv.Columns[i].Name = AllValues[i];
            }

            searchLabel.Text = ("Search querry: '" + search + "' Found the top " + topk.Count + " but limiting to " + k + " results in: " + sw.Elapsed.TotalSeconds + "s!");

            // Get OID, <upperRange, lowerRange>
            List<Tuple<int, Tuple<decimal, decimal>>> topkNew = new List<Tuple<int, Tuple<decimal, decimal>>>();
            foreach (var OIDsim in topk)
            {
                // Get lower and upper range for every item in buffer
                decimal lowerRange = 0;
                decimal upperRange = 0;
                for (int attribute = 0; attribute < OIDsim.Value.Count; attribute++)
                {
                    if (OIDsim.Value[attribute].Item1)
                        lowerRange += Convert.ToDecimal(OIDsim.Value[attribute].Item2);

                    upperRange += Convert.ToDecimal(OIDsim.Value[attribute].Item2);
                }

                // Add OID with <upperRange, lowerRange>
                topkNew.Add(Tuple.Create(OIDsim.Key, Tuple.Create(lowerRange, upperRange)));
            }

            // Sort on lower range, if equal sort on upper range
            topkNew.Sort((x, y) =>
            {
                int result = y.Item2.Item1.CompareTo(x.Item2.Item1);
                return result == 0 ? y.Item2.Item2.CompareTo(x.Item2.Item2) : result;
            });

            foreach (var OIDsim in topkNew.Take(k))
            {
                string sql = "select * from autompg WHERE id=" + OIDsim.Item1.ToString();
                SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                SQLiteDataReader reader = command.ExecuteReader();

                string[] row = new string[AllValues.Length];

                //Console.Write("|" + OIDsim.Item2.Item1 + " - " + OIDsim.Item2.Item2 + "|: ");
                for (int r = 0; r < reader.FieldCount; r++)
                {
                    row[r] = reader[r].ToString();

                    //     Console.Write((string)reader[r].ToString() + "\t");
                }

                dgv.Rows.Add(row);
                //  Console.WriteLine();
            }

            CloseConnection();
        }

        //Database helpers
        private void ExecuteCommand(string com)
        {
            SQLiteCommand command = new SQLiteCommand(com, dbconnection);
            command.ExecuteNonQuery();
        }

        public void CreateDatabaseFile(string name)
        {
            SQLiteConnection.CreateFile(name);
        }

        private void OpenConnection(string name)
        {
            dbconnection = new SQLiteConnection("Data Source=" + name + ";Version=3;");
            dbconnection.Open();
        }

        private void CloseConnection()
        {
            dbconnection.Close();
        }
    }
}