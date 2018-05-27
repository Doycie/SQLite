using System.Collections.Generic;

namespace SQLite
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonPrintMetadataTables = new System.Windows.Forms.Button();
            this.ComboBoxShowMDTable = new System.Windows.Forms.ComboBox();
            this.ProgressMetadatabase = new System.Windows.Forms.ProgressBar();
            this.ButtonFillMetadatabase = new System.Windows.Forms.Button();
            this.ButtonSearch = new System.Windows.Forms.Button();
            this.TextInputSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonPrintMetadataTables
            // 
            this.ButtonPrintMetadataTables.Location = new System.Drawing.Point(1163, 174);
            this.ButtonPrintMetadataTables.Name = "ButtonPrintMetadataTables";
            this.ButtonPrintMetadataTables.Size = new System.Drawing.Size(128, 32);
            this.ButtonPrintMetadataTables.TabIndex = 3;
            this.ButtonPrintMetadataTables.Text = "Retrieve Metadata tables";
            this.ButtonPrintMetadataTables.UseVisualStyleBackColor = true;
            this.ButtonPrintMetadataTables.Click += new System.EventHandler(this.ButtonPrintMetadataTables_Click);
            // 
            // ComboBoxShowMDTable
            // 
            this.ComboBoxShowMDTable.FormattingEnabled = true;
            this.ComboBoxShowMDTable.Location = new System.Drawing.Point(1139, 212);
            this.ComboBoxShowMDTable.Name = "ComboBoxShowMDTable";
            this.ComboBoxShowMDTable.Size = new System.Drawing.Size(152, 21);
            this.ComboBoxShowMDTable.TabIndex = 6;
            this.ComboBoxShowMDTable.SelectedValueChanged += new System.EventHandler(this.ComboBox1_SelectedValueChanged);
            // 
            // ProgressMetadatabase
            // 
            this.ProgressMetadatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ProgressMetadatabase.Location = new System.Drawing.Point(1005, 93);
            this.ProgressMetadatabase.Minimum = 1;
            this.ProgressMetadatabase.Name = "ProgressMetadatabase";
            this.ProgressMetadatabase.Size = new System.Drawing.Size(286, 20);
            this.ProgressMetadatabase.Step = 1;
            this.ProgressMetadatabase.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressMetadatabase.TabIndex = 5;
            this.ProgressMetadatabase.Value = 1;
            // 
            // ButtonFillMetadatabase
            // 
            this.ButtonFillMetadatabase.Location = new System.Drawing.Point(1005, 55);
            this.ButtonFillMetadatabase.Name = "ButtonFillMetadatabase";
            this.ButtonFillMetadatabase.Size = new System.Drawing.Size(286, 32);
            this.ButtonFillMetadatabase.TabIndex = 4;
            this.ButtonFillMetadatabase.Text = "Create metadatbase";
            this.ButtonFillMetadatabase.UseVisualStyleBackColor = true;
            this.ButtonFillMetadatabase.Click += new System.EventHandler(this.ButtonFillMetadatabase_Click);
            // 
            // ButtonSearch
            // 
            this.ButtonSearch.Location = new System.Drawing.Point(12, 135);
            this.ButtonSearch.Name = "ButtonSearch";
            this.ButtonSearch.Size = new System.Drawing.Size(462, 82);
            this.ButtonSearch.TabIndex = 6;
            this.ButtonSearch.Text = "Search";
            this.ButtonSearch.UseVisualStyleBackColor = true;
            this.ButtonSearch.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextInputSearch
            // 
            this.TextInputSearch.Location = new System.Drawing.Point(12, 109);
            this.TextInputSearch.Name = "TextInputSearch";
            this.TextInputSearch.Size = new System.Drawing.Size(462, 20);
            this.TextInputSearch.TabIndex = 7;
            this.TextInputSearch.KeyUp += TextBoxKeyUp;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 52);
            this.label1.TabIndex = 8;
            this.label1.Text = "To search input a search querry in the following way:\r\nk = \'6\', brand = \'volkswag" +
    "en\';\r\ncylinders = \'4\', brand = \'ford\';\r\nMake sure to add quotation marks even to" +
    " numeric values\r\n";
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Location = new System.Drawing.Point(12, 220);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(0, 13);
            this.SearchLabel.TabIndex = 8;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 259);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1279, 307);
            this.dataGridView1.TabIndex = 9;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(480, 135);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(110, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Sort final top K list";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(480, 158);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(196, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "Use attribute similarity from workload";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1005, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(286, 32);
            this.button1.TabIndex = 12;
            this.button1.Text = "Create car database";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1303, 578);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SearchLabel);
            this.Controls.Add(this.ComboBoxShowMDTable);
            this.Controls.Add(this.TextInputSearch);
            this.Controls.Add(this.ButtonPrintMetadataTables);
            this.Controls.Add(this.ProgressMetadatabase);
            this.Controls.Add(this.ButtonSearch);
            this.Controls.Add(this.ButtonFillMetadatabase);
            this.Name = "Form1";
            this.Text = "Database";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonPrintMetadataTables;
        private System.Windows.Forms.ProgressBar ProgressMetadatabase;
        private System.Windows.Forms.Button ButtonFillMetadatabase;
        private System.Windows.Forms.ComboBox ComboBoxShowMDTable;
        private System.Windows.Forms.Button ButtonSearch;
        private System.Windows.Forms.TextBox TextInputSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button1;
    }
}