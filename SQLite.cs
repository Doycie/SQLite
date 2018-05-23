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

        private const string metadatastring = "metadata.sqlite";
        private const string cardatabasestring = "CarDatabase.sqlite";
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

            // Extract  attribute name from table name and add correct valuetype
            string attributeName;
            string column1;
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
            else
            {
                attributeName = tableName;
                column1 = "\tidfqf: ";
            }

            // Print all values
            while (reader.Read())
                Console.WriteLine(attributeName + " value: " + reader[0] + column1 + reader[1]);

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

        private int QFMax = 0;

        //QF functions
        public void MakeQFDictionary()
        {
            if (qfoccurrences.Count > 1)
            {
                Console.WriteLine("QF values have already been put in a dictionary");
                return;
            }
            StreamReader sr = new StreamReader("workload.txt");
            string input;

            // Skip first two lines
            input = sr.ReadLine();
            input = sr.ReadLine();

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
                        // Remove brackets
                        string cats = inputSplit[position + 3];
                        cats = cats.TrimStart('(');
                        cats = cats.TrimEnd(')');

                        // Get all attribute values
                        string[] inputSplit2 = cats.Split(',');

                        foreach (var i in inputSplit2)
                        {
                            // Remove '' from start and end
                            string t = i.Substring(1, i.Length - 2);

                            // Add attribute, value pair to dictionary
                            var key = Tuple.Create(inputSplit[position + 1], t);
                            if (qfoccurrences.ContainsKey(key))
                                qfoccurrences[key] += times;
                            else
                                qfoccurrences.Add(key, times);
                        }
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

            // Get highest attribute value occurence
            int max = 0;
            foreach (int value in qfoccurrences.Values)
            {
                if (value > max)
                    max = value;
            }
            QFMax = max;
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

            ExecuteCommand("CREATE TABLE " + attributeName + " (" + attributeName + " text, idfqf real)");

            foreach (KeyValuePair<string, int> vp in occurrences)
            {
                float qf = 1;
                Tuple<string, string> key = Tuple.Create(attributeName, vp.Key);
                if (qfoccurrences.ContainsKey(key))
                {
                    qf = (float)(qfoccurrences[key] + 1) / (float)(QFMax + 1);
                }

                ExecuteCommand("INSERT into " + attributeName + " VALUES ( '" + vp.Key + "'," + Math.Log(total / vp.Value) * qf + ");");
            }

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

            // Add attribute value t and its number of occurrences in database to table
            ExecuteCommand("CREATE TABLE " + attributeName + "_databaseValues (t double, occurrences integer)");

            // Fill table
            foreach (var kv in uniqueValues)
                ExecuteCommand("INSERT into " + attributeName + "_databaseValues VALUES (" + kv.Key + ", " + kv.Value + " );");

            // Add attribute value t and its number of occurrences in workload to table
            ExecuteCommand("CREATE TABLE " + attributeName + "_workloadValues (t double, occurrences integer)");

            // Fill table
            foreach (var kv in qfoccurrences)
            {
                // Search for corresponding attribute name
                if (kv.Key.Item1.Equals(attributeName))
                    ExecuteCommand("INSERT into " + attributeName + "_workloadValues VALUES (" + Convert.ToDouble(kv.Key.Item2) + ", " + kv.Value + ");");
            }

            CloseConnection();
        }

        // Calculate IDF or QF and bandwith for q
        public Tuple<double, double> numericSmooth(List<Tuple<double, int>> tValues, double qValue)
        {
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
                Console.Write(reader["id"].ToString());
                // Add all query attributes to list
                foreach (string c in CatogoricalValues)
                    AttributeOccurence.Add(Tuple.Create(c, reader[c].ToString(), int.Parse(reader["id"].ToString())));
                foreach (string n in NumericValues)
                    AttributeOccurence.Add(Tuple.Create(n, reader[n].ToString(), int.Parse(reader["id"].ToString())));
            }

            // Open connection with metadatabase
            CloseConnection();
            OpenConnection(metadatastring);

            // Create Occurence tables
            foreach (string c in CatogoricalValues)
                ExecuteCommand("CREATE TABLE " + c + "_Occurence (" + c + " text, id integer)");
            foreach (string n in NumericValues)
                ExecuteCommand("CREATE TABLE " + n + "_Occurence (" + n + " double, id integer)");

            ProgressMetadatabase.Maximum = AttributeOccurence.Count;
            ProgressMetadatabase.Value = 0;

            // Fill Occurence tables with values from list
            foreach (var t in AttributeOccurence)
            {
                ExecuteCommand("INSERT into " + t.Item1 + "_Occurence" + " VALUES ( '" + t.Item2 + "', '" + t.Item3 + "' );");
                ProgressMetadatabase.PerformStep();
            }

            CloseConnection();
        }

        //BuildDatabase from txt file
        public void BuildDatabase(string nameOfDatabase, string fileToLoadFrom)
        {
            CreateDatabaseFile(nameOfDatabase);
            OpenConnection(nameOfDatabase);
            StreamReader sr = new StreamReader(fileToLoadFrom);
            string input;
            while ((input = sr.ReadLine()) != null)
            {
                ExecuteCommand(input);
            }
            CloseConnection();
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
            sql = "select * from " + attributeName + "_Occurence WHERE " + attributeName + " == '" + attributeValue + "' order by id";
            command = new SQLiteCommand(sql, dbconnection);
            reader = command.ExecuteReader();
            int expectedID = 1;
            while (reader.Read())
            {
                while (int.Parse(reader["id"].ToString()) > expectedID) {
                    similarity.Add(Tuple.Create(expectedID, 0.0));
                    expectedID++;
                }
                similarity.Add(Tuple.Create(expectedID, idfqf));
                expectedID++;
            }
            while(expectedID < 397)
            {
                similarity.Add(Tuple.Create(expectedID, 0.0));
                expectedID++;
            }

            CloseConnection();
            similarity.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            // Sort on similarity and return
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

            similarity.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            // Sort on similarity and return
            return similarity;
        }

        //The topK search function
        public void topK(string search, System.Windows.Forms.DataGridView dgv, System.Windows.Forms.Label searchLabel)
        {
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
                    attributeSimilarities.Add(catigoricalSimilarity(attributeName, attributeValue));
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

            List<double> MaximumAttributes = new List<double>();
            int maxP = 0;

            for (int i = 0; i < attributeSimilarities.Count; i++)
            {
                if (attributeSimilarities[i].Count > maxP)
                    maxP = attributeSimilarities[i].Count;

                MaximumAttributes.Add(attributeSimilarities[i][0].Item2);
            }

            double threshold = MaximumAttributes.Sum();

            List<Tuple<int, double>> finalIDS = new List<Tuple<int, double>>();
            Dictionary<int, Tuple<double, double>> buffer = new Dictionary<int, Tuple<double, double>>();

            for (int i = 0; i < attributeSimilarities.Count; i++)
            {
                buffer[attributeSimilarities[i][0].Item1] = Tuple.Create(attributeSimilarities[i][0].Item2, threshold);
            }

            List<int> toremove = new List<int>();
            foreach (var kvp in buffer)
            {
                if (kvp.Value.Item1 >= threshold)
                {
                    finalIDS.Add(Tuple.Create(kvp.Key, kvp.Value.Item2));
                    toremove.Add(kvp.Key);
                }
            }
            foreach (int r in toremove)
            {
                buffer.Remove(r);
            }

            int p = 1;
            double oldthreshold = threshold;
            while (finalIDS.Count < k)
            {
                //Go over all attributes ex volkswagen and cylinders
                for (int i = 0; i < attributeSimilarities.Count; i++)
                {
                    //As long as there are id's with that attribute
                    MaximumAttributes[i] = (attributeSimilarities[i][p].Item2);
                }

                double maxT = 0.0;
                foreach(var kvp in buffer)
                {
                    if(maxT< kvp.Value.Item2)
                    {
                        maxT = kvp.Value.Item2;
                    }
                }

                threshold = maxT;
                double tthreshold = MaximumAttributes.Sum();

                for (int i = 0; i < attributeSimilarities.Count; i++)
                {
                    int id = 0;

                    id = attributeSimilarities[i][p].Item1;

                    if (buffer.ContainsKey(id))
                    {
                        buffer[id] = Tuple.Create(buffer[id].Item1 + MaximumAttributes[i], oldthreshold);
                    }
                    else
                    {
                        buffer[id] = Tuple.Create(MaximumAttributes[i], tthreshold);
                    }
                }
                toremove = new List<int>();
                foreach (var kvp in buffer)
                {
                    if (kvp.Value.Item1 >= threshold)
                    {
                        finalIDS.Add(Tuple.Create(kvp.Key, kvp.Value.Item2));
                        toremove.Add(kvp.Key);
                    }
                }
                foreach (int r in toremove)
                {
                    buffer.Remove(r);
                }

                oldthreshold = tthreshold;
                p++;
                if (p > maxP)
                {
                    Console.WriteLine("Did not find any more similar cars");
                    break;
                }
            }

            OpenConnection(cardatabasestring);

            //finalIDS.Sort((x, y) =>x.Item2.CompareTo(y.Item2));

            sw.Stop();

            dgv.Rows.Clear();
            dgv.ColumnCount = AllValues.Length;
            for (int i = 0; i < AllValues.Length; i++)
            {
                dgv.Columns[i].Name = AllValues[i];
            }

            // Console.Clear();
            // Console.WriteLine("Search querry: '" + search + "' Found the top " + finalIDS.Count + " results in: " + sw.Elapsed.TotalSeconds + "s!");
            searchLabel.Text = ("Search querry: '" + search + "' Found the top " + finalIDS.Count + " but limiting to "+ k +" results in: " + sw.Elapsed.TotalSeconds + "s!");

            for (int j = 0; j < finalIDS.Count && j < k; j++)
            {
                string sql = "select * from autompg WHERE id=" + finalIDS[j].Item1.ToString();
                SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                SQLiteDataReader reader = command.ExecuteReader();

                string[] row = new string[AllValues.Length];

                Console.Write("|" + (j + 1) + "|" + Math.Round(finalIDS[j].Item2, 2) + "|: ");
                for (int r = 0; r < reader.FieldCount; r++)
                {
                    row[r] = reader[r].ToString();

                        Console.Write((string)reader[r].ToString() + "\t");
                }

                dgv.Rows.Add(row);
                Console.WriteLine();
            }

            CloseConnection();

            //int tot = 0;
            //// test printje
            //foreach (List<Tuple<int, double>> attributeSimilarity in attributeSimilarities)
            //{
            //    foreach (Tuple<int, double> kv in attributeSimilarity)
            //    {
            //        tot++;
            //        Console.WriteLine("document id: " + kv.Item1 + " similarity: " + kv.Item2);
            //    }

            //    Console.WriteLine("--------------------------------");
            //    Console.WriteLine("-------------" + tot  +"------------------");
            //    Console.WriteLine("--------------------------------");
            //}

            //////////////////////////////////////////////////////////////////////
            /*
            //A list of tuples for the search terms, ex. <brand,volkswagen>
            List<Tuple<string, string>> terms = new List<Tuple<string, string>>();

            //Default k value is this value plus one so 10
            int k = 10;
            string[] searchTerms = search.Split(',');

            foreach (var st in searchTerms)
            {
                int offset = 0;
                string[] searchTerm = st.Split();
                if (searchTerm[0] == "")
                    offset++;
                if (searchTerm[offset + 0] == "k")
                {
                    k = int.Parse(searchTerm[offset + 2]) ;
                }
                else
                {
                    terms.Add(Tuple.Create(searchTerm[offset + 0], searchTerm[offset + 2]));
                }
            }

            //Buffer to hold an ID and the corresponding minimum and maximum values that ID can have so far
            Dictionary<int, Tuple<float,float>> buffer = new Dictionary<int, Tuple<float, float>>();

            //List to hold the final top k result
            List<Tuple<int,float>> finalIDS = new List<Tuple<int,float>>();
            //List to hold the current maximum values
            List<float> MaxAttributes = new List<float>();

            OpenConnection(metadatastring);

            //Go over each term to add to the maximum list and the treshhold is them all added up
            foreach (var t in terms) {
                string sql = "select idfqf from " + t.Item1 + " order by idfqf DESC limit 1" ;
                SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                SQLiteDataReader reader = command.ExecuteReader();
                MaxAttributes.Add( float.Parse((reader[0].ToString() )) );
            }
            float treshhold = MaxAttributes.Sum();

            //Go over the terms again to add them to the buffer, minimum value is that attributes value and maximum is the treshhold
            int i = 0;
            foreach ( var t in terms)
            {
                string sql = "select id from " + t.Item1 + "_Occurence WHERE  " + t.Item1 + "=" + t.Item2;
                SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    buffer[int.Parse(reader[0].ToString())] = Tuple.Create(MaxAttributes[i], treshhold);
                }
                i++;
            }

            foreach(var value in buffer)
            {
                if( value.Value.Item1 >= treshhold)
                {
                    finalIDS.Add(Tuple.Create(value.Key,value.Value.Item1));
                }
            }

            int it = 1;
            float oldthreshold = treshhold;
            //As long as we need more results
            while(finalIDS.Count < k)
            {
                for(i = 0; i< terms.Count;i++)
                {
                    string sql = "select idfqf from " + terms[i].Item1 + " order by idfqf DESC limit "+(it+1)+" offset " + it;
                    SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    MaxAttributes[i] = (float.Parse((reader[0].ToString())));
                }
                treshhold = MaxAttributes.Sum();

                for(int j = 0; j < terms.Count; j++) {
                    string sql = "select id from " + terms[j].Item1 + "_Occurence WHERE  " + terms[j].Item1 + "=" + terms[j].Item2;
                    SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = int.Parse(reader[0].ToString());

                        if (buffer.ContainsKey(id))
                        {
                            buffer[id] = Tuple.Create(buffer[id].Item1 + MaxAttributes[j], oldthreshold);
                        }
                        else
                        {
                            buffer[id] = Tuple.Create(MaxAttributes[j], treshhold);
                        }
                    }
                }

                foreach (var value in buffer)
                {
                    if (value.Value.Item1 >= treshhold)
                    {
                        finalIDS.Add(Tuple.Create(value.Key, value.Value.Item1));
                    }
                }

                oldthreshold = treshhold;
                it++;
            }

            CloseConnection();

            OpenConnection(cardatabasestring);

            //Console.Clear();
            finalIDS.Sort((x, y) => y.Item2.CompareTo(x.Item2));

           for(int j = 0; j< k; j++) {
                string sql = "select * from autompg WHERE id=" + finalIDS[j].Item1.ToString();
                SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
                SQLiteDataReader reader = command.ExecuteReader();

                for(int r = 0; r < reader.FieldCount;r++)
                {
                    Console.Write((string)reader[r].ToString() + ",");
                }

                Console.WriteLine(" || score: " + finalIDS[j].Item2);
            }

            CloseConnection();
            */
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