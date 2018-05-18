using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace SQLite
{
    public class SQLite
    {
        private SQLiteConnection dbconnection;

        const string metadatastring = "metadata.sqlite";
        const string cardatabasestring = "CarDatabase.sqlite";

        //Read IDFQF value for attribute
        public void ReadDatabase( string attributeName)
        {
            OpenConnection(metadatastring);

            string sql = "select * from " + attributeName + " order by idfqf";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())

                Console.WriteLine(attributeName + ": " + reader[attributeName] + "\tIDFQF: " + reader["idfqf"]);

            CloseConnection();
        }

        //Print all IDFQF attributes that are in the metadatabase
        public List<string> PrintMetadataTables()
        {
            List<string> tables = new List<string>();
            string sql = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            OpenConnection(metadatastring);
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while(reader.Read())
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

            input = sr.ReadLine();
            input = sr.ReadLine();

            while ((input = sr.ReadLine()) != null)
            {
                int position = 0;

                string[] inputSplit = input.Split();

                int times = int.Parse(inputSplit[0]);

                while (inputSplit[position] != "WHERE")
                {
                    position++;
                }

                while (position < inputSplit.Length - 1)
                {
                    if (inputSplit[position + 2] == "IN" )
                    {
                        
                        string cats = inputSplit[position + 3];
                        cats = cats.TrimStart('(');
                        cats = cats.TrimEnd(')');

                        string[] inputSplit2 = cats.Split(',');

                        foreach (var l in inputSplit2)
                        {
                            var key = Tuple.Create(inputSplit[position + 1], l);
                            if (qfoccurrences.ContainsKey(key))
                                qfoccurrences[key] += times;
                            else
                                qfoccurrences.Add(key, times);
                        }
                            position += 4;
                    }
                    else
                    {
                        var key = Tuple.Create(inputSplit[position + 1], inputSplit[position + 3]);
                        if (qfoccurrences.ContainsKey(key))
                            qfoccurrences[key] += times;
                        else
                            qfoccurrences.Add(key, times);
                        position += 4;
                    }
                }
            }

            int max = 0;

            foreach (int value in qfoccurrences.Values)
            {
                if (value > max)
                {
                    max = value;
                }
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

        //Add all IDFQF attributes
        public void FillMetaDBWithIDFQF(System.Windows.Forms.ProgressBar ProgressMetadatabase )
        {
            CreateDatabaseFile(metadatastring);

            string[] CatogoricalValues = { "cylinders", "model_year", "origin", "brand", "model","type" };
            ProgressMetadatabase.Maximum = CatogoricalValues.Length;
            ProgressMetadatabase.Value = 1;
            foreach(var s in CatogoricalValues)
            {
                FillMetaDBWithIDF(s);
                ProgressMetadatabase.PerformStep();
            }

        }
        //Add an attribute for IDFQF values
        public void FillMetaDBWithIDF(  string attributeName)
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
                Tuple<string, string> key = Tuple.Create(attributeName, "'" + vp.Key + "'");
                if (qfoccurrences.ContainsKey(key))
                {
                    qf = (float)(qfoccurrences[key] + 1) / (float)(QFMax + 1);
                }

                ExecuteCommand("INSERT into " + attributeName + " VALUES ( '" + vp.Key + "'," + Math.Log(total / vp.Value) * qf + ");");
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