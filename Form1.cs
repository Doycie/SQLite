using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SQLite
{
    public partial class Form1 : Form
    {
        private SQLite db;

        public Form1(SQLite d)
        {
            db = d;
            InitializeComponent();
            List<string> tables = db.GetMetadataTables();
            if (tables.Count > 0)
            {
                this.ProgressMetadatabase.Maximum = tables.Count;
                this.ProgressMetadatabase.Value = tables.Count;
            }
        }

        
        private void ButtonPrintMetadataTables_Click(object sender, EventArgs e)
        {
            ComboBoxShowMDTable.Items.Clear();
            List<string> tables = db.GetMetadataTables();
            foreach (var table in tables)
            {
                Console.WriteLine(table);

                ComboBoxShowMDTable.Items.Add(table);
            }
        }

        private void ButtonFillMetadatabase_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to rebuild the metadatabase, it might take some time?", "Confirm", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                db.MakeQFDictionary();
                db.fillMetaDB(ProgressMetadatabase);
            }
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string selectedItem = (string)ComboBoxShowMDTable.SelectedItem;
            db.readDatabase(selectedItem);
         
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                db.topK(TextInputSearch.Text, dataGridView1,SearchLabel);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.topK(TextInputSearch.Text, dataGridView1 , SearchLabel);
        }


    }
}