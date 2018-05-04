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
        
        public void ReadDatabase()
        {
            OpenConnection();

            string sql = "select * from autompg order by model_year desc";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["model"] + "\tScore: " + reader["model_year"]);

            Console.ReadLine();


            CloseConnection();
        }



        public void BuildDatabase()
        {

            CreateDatabaseFile();
            OpenConnection();


            StreamReader sr = new StreamReader("autompg.sql");

            string input;

            while((input = sr.ReadLine()) != null)
            {
                ExecuteCommand(input);


            }

            CloseConnection();
        }
        
        void CreateExampleTable()
        {

            string sql = "create table highscores (name varchar(20), score int)";
            ExecuteCommand(sql);
            sql = "insert into highscores (name, score) values ('Me', 3000)";
            ExecuteCommand(sql);
            sql = "insert into highscores (name, score) values ('Myself', 6000)";
            ExecuteCommand(sql);
            sql = "insert into highscores (name, score) values ('And I', 9001)";
            ExecuteCommand(sql);
        }


        void ExecuteCommand(string com)
        {
            SQLiteCommand command = new SQLiteCommand(com, dbconnection);
            command.ExecuteNonQuery();

        }


        void CreateDatabaseFile()
        {
            SQLiteConnection.CreateFile("CarDatabase.sqlite");
        }
        void OpenConnection()
        {
            dbconnection = new SQLiteConnection("Data Source=CarDatabase.sqlite;Version=3;");
            dbconnection.Open();
        }
        void CloseConnection()
        {
            dbconnection.Close();
        }
    }


}
