﻿using System;
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
            db.BuildDatabase();

            db.ReadDatabase();

  
        }
       
    }
}
