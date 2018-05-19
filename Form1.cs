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
            List<string> tables = db.PrintMetadataTables();
            if (tables.Count > 0)
            {
                this.ProgressMetadatabase.Maximum = tables.Count;
                this.ProgressMetadatabase.Value = tables.Count;
            }
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

            comboBox1.Items.Clear();
            List<string> tables = db.PrintMetadataTables();
            foreach (var table in tables)
            {
                Console.WriteLine(table);
                
                comboBox1.Items.Add(table);
                
            }

        }

        private void ButtonFillMetadatabase_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to rebuild the metadatabase, it might take some time?", "Confirm", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                db.FillMetaDBWithIDFQF(ProgressMetadatabase);

            }
        }
        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string selectedItem = (string)comboBox1.SelectedItem;
            if(selectedItem.EndsWith("_Occurence"))
            {
                db.ReadDatabase_Occurence(selectedItem);
            }
            else
            {
                Console.WriteLine(selectedItem);
                db.ReadDatabase_IDFQF(selectedItem);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.topK();
        }
    }
}
