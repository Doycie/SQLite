using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLite
{
    class SQLite
    {

        SQLiteConnection dbconnection;

        public void Run()
        {
            OpenConnection();


           // CreateExampleTable();

            string sql = "select * from highscores order by score desc";
            SQLiteCommand command = new SQLiteCommand(sql, dbconnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);

            Console.ReadLine();

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
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
        }
        void OpenConnection()
        {
            dbconnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            dbconnection.Open();
        }
        void CloseConnection()
        {
            dbconnection.Close();
        }
    }


}
