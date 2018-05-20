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
            this.SuspendLayout();
            // 
            // ButtonPrintMetadataTables
            // 
            this.ButtonPrintMetadataTables.Location = new System.Drawing.Point(12, 184);
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
            this.ComboBoxShowMDTable.Location = new System.Drawing.Point(12, 221);
            this.ComboBoxShowMDTable.Name = "ComboBoxShowMDTable";
            this.ComboBoxShowMDTable.Size = new System.Drawing.Size(128, 21);
            this.ComboBoxShowMDTable.TabIndex = 6;
            this.ComboBoxShowMDTable.SelectedValueChanged += new System.EventHandler(this.ComboBox1_SelectedValueChanged);
            // 
            // ProgressMetadatabase
            // 
            this.ProgressMetadatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ProgressMetadatabase.Location = new System.Drawing.Point(146, 221);
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
            this.ButtonFillMetadatabase.Location = new System.Drawing.Point(146, 183);
            this.ButtonFillMetadatabase.Name = "ButtonFillMetadatabase";
            this.ButtonFillMetadatabase.Size = new System.Drawing.Size(286, 32);
            this.ButtonFillMetadatabase.TabIndex = 4;
            this.ButtonFillMetadatabase.Text = "Create metadatbase";
            this.ButtonFillMetadatabase.UseVisualStyleBackColor = true;
            this.ButtonFillMetadatabase.Click += new System.EventHandler(this.ButtonFillMetadatabase_Click);
            // 
            // ButtonSearch
            // 
            this.ButtonSearch.Location = new System.Drawing.Point(357, 83);
            this.ButtonSearch.Name = "ButtonSearch";
            this.ButtonSearch.Size = new System.Drawing.Size(75, 23);
            this.ButtonSearch.TabIndex = 6;
            this.ButtonSearch.Text = "Search";
            this.ButtonSearch.UseVisualStyleBackColor = true;
            this.ButtonSearch.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextInputSearch
            // 
            this.TextInputSearch.Location = new System.Drawing.Point(12, 83);
            this.TextInputSearch.Name = "TextInputSearch";
            this.TextInputSearch.Size = new System.Drawing.Size(339, 20);
            this.TextInputSearch.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 39);
            this.label1.TabIndex = 8;
            this.label1.Text = "To search input a search querry in the following way:\r\nk = 6, brand = \'volkswagen" +
    "\';\r\ncylinders = 4, brand = \'ford\';";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 254);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboBoxShowMDTable);
            this.Controls.Add(this.TextInputSearch);
            this.Controls.Add(this.ButtonPrintMetadataTables);
            this.Controls.Add(this.ProgressMetadatabase);
            this.Controls.Add(this.ButtonSearch);
            this.Controls.Add(this.ButtonFillMetadatabase);
            this.Name = "Form1";
            this.Text = "Database";
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
    }
}