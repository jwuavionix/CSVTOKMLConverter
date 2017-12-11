using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;
using System.Xml;

namespace CSV_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            tbInput.BackColor = Color.White;
            DialogResult result = openFileDialogInput.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK)
            {
                tbInput.Text = openFileDialogInput.FileName;
            }
        }

        private void BtnOutputBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialogOutput.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK)
            {
                tbOutput.Text = saveFileDialogOutput.FileName;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblFeedback.Text = "Working...";
            String input = tbInput.Text;
            String output = tbOutput.Text;
            string directory;
            
            if (input == "")
            {
                lblFeedback.Text = "Input file field is empty.";
                tbInput.BackColor = Color.IndianRed;
            }
            else
            {
                if (output == "")
                {
                    directory = Path.GetDirectoryName(input) + "\\out.kml";
                    output = tbOutput.Text = directory;
                }

                CsvReader csv = new CsvReader(new StreamReader(input), true);
                var xmlWriter = XmlWriter.Create(output);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("root");
                String point = "";
                bool isCoord = false;
                bool hasCoord = false;

                int fieldCount = csv.FieldCount;
                string[] headers = csv.GetFieldHeaders();
                for (int i = 0; i < fieldCount; i++)
                {
                    headers[i] = headers[i].Replace(":", "-");
                }

                while (csv.ReadNextRecord())
                {
                    xmlWriter.WriteStartElement("Placemark");

                    for (int i = 0; i < fieldCount; i++)
                    {
                        isCoord = headers[i].Contains("GPS--Lat") ? true : isCoord;
                        isCoord = headers[i].Contains("GPS--Lon") ? true : isCoord;
                        isCoord = headers[i].Contains("GPS--Alt") ? true : isCoord;
                        if(isCoord)
                        {
                            point += csv[i] + ", ";
                            isCoord = false;
                        }
                        xmlWriter.WriteElementString(headers[i], csv[i]);
                    }
                    
                    point = point.Substring(0, point.Length - 2); //trim ending ", " from point
                    xmlWriter.WriteStartElement("Point");
                    xmlWriter.WriteElementString("Coordinates", point);
                    xmlWriter.WriteEndElement(); //Point
                    xmlWriter.WriteEndElement(); //Placemark
                    point = "";
                }

                xmlWriter.WriteEndElement(); //root...or Groot?
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                lblFeedback.Text = "File converted";
            }
        }
    }
}
