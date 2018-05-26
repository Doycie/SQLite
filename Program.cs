using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace SQLite
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            SQLite db = new SQLite();

            //db.BuildDatabase("CarDatabase.sqlite", );

            Form1 window = new Form1(db);
            System.Windows.Forms.Application.Run(window);
            
            
            Console.ReadLine();

        }
       
    }
}
