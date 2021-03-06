﻿namespace CSV_Converter
{
    partial class CsvToKmlConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CsvToKmlConverter));
            this.BtnInputBrowse = new System.Windows.Forms.Button();
            this.openFileDialogInput = new System.Windows.Forms.OpenFileDialog();
            this.lblInput = new System.Windows.Forms.Label();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.BtnOutputBrowse = new System.Windows.Forms.Button();
            this.BtnConvert = new System.Windows.Forms.Button();
            this.lblFeedback = new System.Windows.Forms.Label();
            this.saveFileDialogOutput = new System.Windows.Forms.SaveFileDialog();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOutfileNaming = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClear = new System.Windows.Forms.Button();
            this.tsHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnInputBrowse
            // 
            this.BtnInputBrowse.Location = new System.Drawing.Point(326, 52);
            this.BtnInputBrowse.Name = "BtnInputBrowse";
            this.BtnInputBrowse.Size = new System.Drawing.Size(75, 23);
            this.BtnInputBrowse.TabIndex = 2;
            this.BtnInputBrowse.Text = "Browse";
            this.BtnInputBrowse.UseVisualStyleBackColor = true;
            this.BtnInputBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // openFileDialogInput
            // 
            this.openFileDialogInput.FileName = "in.csv";
            this.openFileDialogInput.Filter = "CSV files|*.csv|All files|*.*";
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(12, 37);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(50, 13);
            this.lblInput.TabIndex = 100;
            this.lblInput.Text = "Input File";
            // 
            // tbInput
            // 
            this.tbInput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbInput.Location = new System.Drawing.Point(15, 54);
            this.tbInput.Name = "tbInput";
            this.tbInput.ReadOnly = true;
            this.tbInput.Size = new System.Drawing.Size(305, 20);
            this.tbInput.TabIndex = 1;
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbOutput.Location = new System.Drawing.Point(15, 109);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.Size = new System.Drawing.Size(305, 20);
            this.tbOutput.TabIndex = 3;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(12, 92);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(58, 13);
            this.lblOutput.TabIndex = 40;
            this.lblOutput.Text = "Output File";
            // 
            // BtnOutputBrowse
            // 
            this.BtnOutputBrowse.Location = new System.Drawing.Point(326, 107);
            this.BtnOutputBrowse.Name = "BtnOutputBrowse";
            this.BtnOutputBrowse.Size = new System.Drawing.Size(75, 23);
            this.BtnOutputBrowse.TabIndex = 4;
            this.BtnOutputBrowse.Text = "Browse";
            this.BtnOutputBrowse.UseVisualStyleBackColor = true;
            this.BtnOutputBrowse.Click += new System.EventHandler(this.BtnOutputBrowse_Click);
            // 
            // BtnConvert
            // 
            this.BtnConvert.Location = new System.Drawing.Point(326, 160);
            this.BtnConvert.Name = "BtnConvert";
            this.BtnConvert.Size = new System.Drawing.Size(75, 23);
            this.BtnConvert.TabIndex = 5;
            this.BtnConvert.Text = "Convert";
            this.BtnConvert.UseVisualStyleBackColor = true;
            this.BtnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // lblFeedback
            // 
            this.lblFeedback.AutoSize = true;
            this.lblFeedback.Location = new System.Drawing.Point(12, 143);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(52, 13);
            this.lblFeedback.TabIndex = 7;
            this.lblFeedback.Text = "feedback";
            // 
            // saveFileDialogOutput
            // 
            this.saveFileDialogOutput.Filter = "KML Files|*.kml|All files|*.*";
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(15, 159);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(305, 23);
            this.pBar.TabIndex = 101;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenu,
            this.tsHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(492, 24);
            this.menuStrip1.TabIndex = 102;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsMenu
            // 
            this.tsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOutfileNaming,
            this.tsMenuExit});
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(50, 20);
            this.tsMenu.Text = "Menu";
            // 
            // tsMenuOutfileNaming
            // 
            this.tsMenuOutfileNaming.Name = "tsMenuOutfileNaming";
            this.tsMenuOutfileNaming.Size = new System.Drawing.Size(215, 22);
            this.tsMenuOutfileNaming.Text = "Output File based on Input";
            this.tsMenuOutfileNaming.Click += new System.EventHandler(this.tsMenuOutfileNaming_Click);
            // 
            // tsMenuExit
            // 
            this.tsMenuExit.Name = "tsMenuExit";
            this.tsMenuExit.Size = new System.Drawing.Size(215, 22);
            this.tsMenuExit.Text = "Exit";
            this.tsMenuExit.Click += new System.EventHandler(this.tsMenuExit_Click);
            // 
            // tsHelp
            // 
            this.tsHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsHelpAbout});
            this.tsHelp.Name = "tsHelp";
            this.tsHelp.Size = new System.Drawing.Size(44, 20);
            this.tsHelp.Text = "Help";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(407, 107);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 103;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tsHelpAbout
            // 
            this.tsHelpAbout.Name = "tsHelpAbout";
            this.tsHelpAbout.Size = new System.Drawing.Size(152, 22);
            this.tsHelpAbout.Text = "About";
            this.tsHelpAbout.Click += new System.EventHandler(this.tsHelpAbout_Click);
            // 
            // CsvToKmlConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 196);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.lblFeedback);
            this.Controls.Add(this.BtnConvert);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.BtnOutputBrowse);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.BtnInputBrowse);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CsvToKmlConverter";
            this.Text = "CSV to KML Converter";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnInputBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialogInput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Button BtnOutputBrowse;
        private System.Windows.Forms.Button BtnConvert;
        private System.Windows.Forms.Label lblFeedback;
        private System.Windows.Forms.SaveFileDialog saveFileDialogOutput;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsMenuExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOutfileNaming;
        private System.Windows.Forms.ToolStripMenuItem tsHelp;
        private System.Windows.Forms.ToolStripMenuItem tsHelpAbout;
    }
}

