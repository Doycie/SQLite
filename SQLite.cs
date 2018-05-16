using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SQLite
{
    class SQLite
    {

        SQLiteConnection dbconnection;
        


        public void ReadDatabase(string name, string attributeName)
        {
            OpenConnection(name);

            string sql = "select * from "+ attributeName+" order by idfqf";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                
                Console.WriteLine(attributeName+": " + reader[attributeName] + "\tIDFQF: " + reader["idfqf"]);

  


            CloseConnection();
        }


        Dictionary<Tuple<string, string>, int> qfoccurrences = new Dictionary<Tuple<string, string>, int>();
        int QFMax = 0;
        public void MakeQFDictionary()
        {

            StreamReader sr = new StreamReader("workload.txt");
            string input;

            input = sr.ReadLine();
            input = sr.ReadLine();
            
            while ((input = sr.ReadLine()) != null)
            {
                int position = 0;

                string[] inputSplit = input.Split();
                
                int times = int.Parse(inputSplit[0]);

                while(inputSplit[position] != "WHERE")
                {
                    position++;
                }

                while (position < inputSplit.Length- 1)
                {

                    var key = Tuple.Create(inputSplit[position + 1], inputSplit[position + 3]);

                    if (qfoccurrences.ContainsKey(key))
                    {
                        qfoccurrences[key] += times;
                    }
                    else
                    {
                        qfoccurrences.Add(key, times);
                    }

                    position += 4;
                }

            }

            int max = 0;

            foreach (int value in qfoccurrences.Values)
            {
                if( value > max)
                {
                    max = value;
                }
            }

            QFMax = max;
         

     
        }

        public void FillMetaDBWithIDF(string OriginalDatabase, string metaDatabase , string attributeName )
        {



            OpenConnection(OriginalDatabase);


            double total = 0;
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            string sql = "select * from autompg";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read()) {

                
                string m = reader[attributeName].ToString();
                total++;
                if (occurrences.ContainsKey(m))
                    occurrences[m] += 1;
                else
                    occurrences.Add(m,1);

            }

            CloseConnection();

            OpenConnection(metaDatabase);
            
            ExecuteCommand("CREATE TABLE " + attributeName  + " ("+ attributeName + " text, idfqf real)");

            foreach ( KeyValuePair<string,int> vp in occurrences)
            {
                float qf = 1;
                Tuple<string,string> key = Tuple.Create(attributeName, "'"+vp.Key+"'");
                if (qfoccurrences.ContainsKey(key))
                {
                    qf = (float)(qfoccurrences[key]+1) / (float)(QFMax+1);
                }
           

                ExecuteCommand("INSERT into " + attributeName+ " VALUES ( '"  + vp.Key + "'," + Math.Log( total/ vp.Value ) * qf + ");");
            }

            CloseConnection();
        }

        public void BuildDatabase(string nameOfDatabase, string fileToLoadFrom)
        {

            CreateDatabaseFile(nameOfDatabase);
            OpenConnection(nameOfDatabase);
            StreamReader sr = new StreamReader(fileToLoadFrom);
            string input;
            while((input = sr.ReadLine()) != null)
            {
                ExecuteCommand(input);
            }
            CloseConnection();
        }
        



        void ExecuteCommand(string com)
        {
            SQLiteCommand command = new SQLiteCommand(com, dbconnection);
            command.ExecuteNonQuery();
        }

        public void CreateDatabaseFile(string name)
        {
            SQLiteConnection.CreateFile(name);
        }
        void OpenConnection(string name)
        {
            dbconnection = new SQLiteConnection("Data Source="+ name + ";Version=3;");
            dbconnection.Open();
        }
        void CloseConnection()
        {
            dbconnection.Close();
        }
    }


}
