using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLite
{
    public partial class Form1 : Form
    {

        SQLite db;

        public Form1(SQLite d)
        {
            db = d;
            InitializeComponent();
        }

        private void ButtonBuildQFDictionary_Click(object sender, EventArgs e)
        {
            db.MakeQFDictionary();
            this.ProgressQFDictionary.PerformStep();
        }

        private void ButtonPrintQFDictionary_Click(object sender, EventArgs e)
        {
            db.PrintQFDictionary();
        }

        private void ButtonPrintMetadataTables_Click(object sender, EventArgs e)
        {
         
            List<string> tables = db.PrintMetadataTables();
            foreach (var table in tables)
            {
                comboBox1.Items.Add(table);
            }

        }

        private void ButtonFillMetadatabase_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to rebuild the metadatabase, it might take some time?", "Confirm I have balls of steel", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                db.FillMetaDBWithIDFQF(ProgressMetadatabase);
            }
        }
        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine((string)comboBox1.SelectedItem);
            db.ReadDatabase((string)comboBox1.SelectedItem);
        }
    }
}
