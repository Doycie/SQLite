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
            this.ProgressQFDictionary = new System.Windows.Forms.ProgressBar();
            this.ButtonBuildQFDictionary = new System.Windows.Forms.Button();
            this.ButtonPrintQFDictionary = new System.Windows.Forms.Button();
            this.ButtonPrintMetadataTables = new System.Windows.Forms.Button();
            this.GroupMeta = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ProgressMetadatabase = new System.Windows.Forms.ProgressBar();
            this.ButtonFillMetadatabase = new System.Windows.Forms.Button();
            this.GroupQF = new System.Windows.Forms.GroupBox();
            this.GroupMeta.SuspendLayout();
            this.GroupQF.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressQFDictionary
            // 
            this.ProgressQFDictionary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ProgressQFDictionary.Location = new System.Drawing.Point(16, 54);
            this.ProgressQFDictionary.Maximum = 2;
            this.ProgressQFDictionary.Minimum = 1;
            this.ProgressQFDictionary.Name = "ProgressQFDictionary";
            this.ProgressQFDictionary.Size = new System.Drawing.Size(354, 18);
            this.ProgressQFDictionary.Step = 1;
            this.ProgressQFDictionary.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressQFDictionary.TabIndex = 0;
            this.ProgressQFDictionary.Value = 1;
            // 
            // ButtonBuildQFDictionary
            // 
            this.ButtonBuildQFDictionary.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonBuildQFDictionary.Location = new System.Drawing.Point(16, 16);
            this.ButtonBuildQFDictionary.Name = "ButtonBuildQFDictionary";
            this.ButtonBuildQFDictionary.Size = new System.Drawing.Size(128, 32);
            this.ButtonBuildQFDictionary.TabIndex = 1;
            this.ButtonBuildQFDictionary.Text = "Make QF dictionary";
            this.ButtonBuildQFDictionary.UseVisualStyleBackColor = false;
            this.ButtonBuildQFDictionary.Click += new System.EventHandler(this.ButtonBuildQFDictionary_Click);
            // 
            // ButtonPrintQFDictionary
            // 
            this.ButtonPrintQFDictionary.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonPrintQFDictionary.Location = new System.Drawing.Point(160, 16);
            this.ButtonPrintQFDictionary.Name = "ButtonPrintQFDictionary";
            this.ButtonPrintQFDictionary.Size = new System.Drawing.Size(128, 32);
            this.ButtonPrintQFDictionary.TabIndex = 2;
            this.ButtonPrintQFDictionary.Text = "Print QF Dictionary";
            this.ButtonPrintQFDictionary.UseVisualStyleBackColor = false;
            this.ButtonPrintQFDictionary.Click += new System.EventHandler(this.ButtonPrintQFDictionary_Click);
            // 
            // ButtonPrintMetadataTables
            // 
            this.ButtonPrintMetadataTables.Location = new System.Drawing.Point(16, 16);
            this.ButtonPrintMetadataTables.Name = "ButtonPrintMetadataTables";
            this.ButtonPrintMetadataTables.Size = new System.Drawing.Size(128, 32);
            this.ButtonPrintMetadataTables.TabIndex = 3;
            this.ButtonPrintMetadataTables.Text = "Retrieve Metadata tables";
            this.ButtonPrintMetadataTables.UseVisualStyleBackColor = true;
            this.ButtonPrintMetadataTables.Click += new System.EventHandler(this.ButtonPrintMetadataTables_Click);
            //
            // GroupMeta
            // 
            this.GroupMeta.BackColor = System.Drawing.SystemColors.Control;
            this.GroupMeta.Controls.Add(this.comboBox1);
            this.GroupMeta.Controls.Add(this.ProgressMetadatabase);
            this.GroupMeta.Controls.Add(this.ButtonFillMetadatabase);
            this.GroupMeta.Controls.Add(this.ButtonPrintMetadataTables);
            this.GroupMeta.Location = new System.Drawing.Point(22, 116);
            this.GroupMeta.Name = "GroupMeta";
            this.GroupMeta.Size = new System.Drawing.Size(384, 128);
            this.GroupMeta.TabIndex = 4;
            this.GroupMeta.TabStop = false;
            this.GroupMeta.Text = "Metadata Control Panel";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(150, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(144, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.ComboBox1_SelectedValueChanged);
            // 
            // ProgressMetadatabase
            // 
            this.ProgressMetadatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ProgressMetadatabase.Location = new System.Drawing.Point(16, 102);
            this.ProgressMetadatabase.Minimum = 1;
            this.ProgressMetadatabase.Name = "ProgressMetadatabase";
            this.ProgressMetadatabase.Size = new System.Drawing.Size(354, 20);
            this.ProgressMetadatabase.Step = 1;
            this.ProgressMetadatabase.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressMetadatabase.TabIndex = 5;
            this.ProgressMetadatabase.Value = 1;
            List<string> tables = db.PrintMetadataTables();
            if (tables.Count > 0)
            { 
                this.ProgressMetadatabase.Maximum = tables.Count;
                this.ProgressMetadatabase.Value = tables.Count;
            }
            // 
            // ButtonFillMetadatabase
            // 
            this.ButtonFillMetadatabase.Location = new System.Drawing.Point(16, 64);
            this.ButtonFillMetadatabase.Name = "ButtonFillMetadatabase";
            this.ButtonFillMetadatabase.Size = new System.Drawing.Size(278, 32);
            this.ButtonFillMetadatabase.TabIndex = 4;
            this.ButtonFillMetadatabase.Text = "Fill Metadatabase with all catogorical attributes";
            this.ButtonFillMetadatabase.UseVisualStyleBackColor = true;
            this.ButtonFillMetadatabase.Click += new System.EventHandler(this.ButtonFillMetadatabase_Click);
            // 
            // GroupQF
            // 
            this.GroupQF.BackColor = System.Drawing.SystemColors.Control;
            this.GroupQF.Controls.Add(this.ButtonBuildQFDictionary);
            this.GroupQF.Controls.Add(this.ButtonPrintQFDictionary);
            this.GroupQF.Controls.Add(this.ProgressQFDictionary);
            this.GroupQF.Location = new System.Drawing.Point(22, 12);
            this.GroupQF.Name = "GroupQF";
            this.GroupQF.Size = new System.Drawing.Size(384, 98);
            this.GroupQF.TabIndex = 5;
            this.GroupQF.TabStop = false;
            this.GroupQF.Text = "QF Control Panel";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 640);
            this.Controls.Add(this.GroupQF);
            this.Controls.Add(this.GroupMeta);
            this.Name = "Form1";
            this.Text = "Form1";
            this.GroupMeta.ResumeLayout(false);
            this.GroupQF.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupQF;
        private System.Windows.Forms.ProgressBar ProgressQFDictionary;
        private System.Windows.Forms.Button ButtonBuildQFDictionary;
        private System.Windows.Forms.Button ButtonPrintQFDictionary;

        private System.Windows.Forms.GroupBox GroupMeta;
        private System.Windows.Forms.Button ButtonPrintMetadataTables;
        private System.Windows.Forms.ProgressBar ProgressMetadatabase;
        private System.Windows.Forms.Button ButtonFillMetadatabase;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}