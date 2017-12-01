namespace CSV_Converter
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
            this.BtnInputBrowse = new System.Windows.Forms.Button();
            this.openFileDialogInput = new System.Windows.Forms.OpenFileDialog();
            this.lblInput = new System.Windows.Forms.Label();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.BtnOutputBrowse = new System.Windows.Forms.Button();
            this.BtnConvert = new System.Windows.Forms.Button();
            this.openFileDialogOutput = new System.Windows.Forms.OpenFileDialog();
            this.lblFeedback = new System.Windows.Forms.Label();
            this.saveFileDialogOutput = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // BtnInputBrowse
            // 
            this.BtnInputBrowse.Location = new System.Drawing.Point(326, 24);
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
            this.lblInput.Location = new System.Drawing.Point(12, 9);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(50, 13);
            this.lblInput.TabIndex = 100;
            this.lblInput.Text = "Input File";
            // 
            // tbInput
            // 
            this.tbInput.Location = new System.Drawing.Point(15, 26);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(305, 20);
            this.tbInput.TabIndex = 1;
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(15, 81);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(305, 20);
            this.tbOutput.TabIndex = 3;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(12, 64);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(58, 13);
            this.lblOutput.TabIndex = 40;
            this.lblOutput.Text = "Output File";
            // 
            // BtnOutputBrowse
            // 
            this.BtnOutputBrowse.Location = new System.Drawing.Point(326, 79);
            this.BtnOutputBrowse.Name = "BtnOutputBrowse";
            this.BtnOutputBrowse.Size = new System.Drawing.Size(75, 23);
            this.BtnOutputBrowse.TabIndex = 4;
            this.BtnOutputBrowse.Text = "Browse";
            this.BtnOutputBrowse.UseVisualStyleBackColor = true;
            this.BtnOutputBrowse.Click += new System.EventHandler(this.BtnOutputBrowse_Click);
            // 
            // BtnConvert
            // 
            this.BtnConvert.Location = new System.Drawing.Point(324, 132);
            this.BtnConvert.Name = "BtnConvert";
            this.BtnConvert.Size = new System.Drawing.Size(75, 23);
            this.BtnConvert.TabIndex = 5;
            this.BtnConvert.Text = "Convert";
            this.BtnConvert.UseVisualStyleBackColor = true;
            this.BtnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // openFileDialogOutput
            // 
            this.openFileDialogOutput.FileName = "out.kml";
            this.openFileDialogOutput.Filter = "KML Files|*.kml|All files|*.*";
            // 
            // lblFeedback
            // 
            this.lblFeedback.AutoSize = true;
            this.lblFeedback.Location = new System.Drawing.Point(12, 132);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(0, 13);
            this.lblFeedback.TabIndex = 7;
            // 
            // saveFileDialogOutput
            // 
            this.saveFileDialogOutput.Filter = "KML Files|*.kml|All files|*.*";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 167);
            this.Controls.Add(this.lblFeedback);
            this.Controls.Add(this.BtnConvert);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.BtnOutputBrowse);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.BtnInputBrowse);
            this.Name = "Form1";
            this.Text = "Form1";
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
        private System.Windows.Forms.OpenFileDialog openFileDialogOutput;
        private System.Windows.Forms.Label lblFeedback;
        private System.Windows.Forms.SaveFileDialog saveFileDialogOutput;
    }
}

