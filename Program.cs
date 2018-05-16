using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    class Program
    {
        const string metadatastring = "metadata.sqlite";
        const string cardatabasestring = "CarDatabase.sqlite";


        static void Main(string[] args)
        {

            SQLite db = new SQLite();




            //db.CreateDatabaseFile(metadatastring);
             //db.MakeQFDictionary();
             //db.FillMetaDBWithIDF(cardatabasestring,metadatastring,"brand");

            // db.FillMetaDBWithIDF(cardatabasestring, metadatastring, "model");


            
            // db.ReadDatabase(metadatastring, "model");
            db.ReadDatabase(metadatastring, "brand");

            Console.ReadLine();

        }
       
    }
}
