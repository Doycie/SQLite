using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    class Program
    {
        static void Main(string[] args)
        {

            SQLite db = new SQLite();

            Form1 window = new Form1(db);
            System.Windows.Forms.Application.Run(window);


            Console.ReadLine();

        }
       
    }
}
