using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DialogResult result = openFileDialogInput.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                tbInput.Text = openFileDialogInput.FileName;
                //convenience: use regex to keep directory and change file name for default output

                /*
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    //List<String> lines = (List<String>) File.ReadLines(file);
                    int size = text.Length;
                    label1.Text = "text: " + text + " size: " + size;
                }
                catch (IOException)
                {
                }
                */
            }
            //Console.WriteLine(size); // <-- Shows file size in debugging mode.
            //Console.WriteLine(result); // <-- For debugging use.
        }

        private void BtnOutputBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialogOutput.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                tbOutput.Text = saveFileDialogOutput.FileName;
            }

            /*
            String temp = "";
            // open the file "data.csv" which is a CSV file with headers
            using (CsvReader csv =
                   new CsvReader(new StreamReader("in.csv"), true))
            {
                int fieldCount = csv.FieldCount;
                lblFeedback.Text = fieldCount.ToString();
                
                string[] headers = csv.GetFieldHeaders();
                while (csv.ReadNextRecord())
                {
                    for (int i = 0; i < fieldCount; i++)
                    temp = string.Format("{0} = {1};", headers[i], csv[i]);
                    lblFeedback.Text += " " + temp; // string.Format("{0} = {1};", csv[i], csv[i]);
                    lblFeedback.Text += " done";

                    //writer.Write("stuff");
                }
            }
            StreamWriter writer = new StreamWriter("out.kml", false);
            writer.Write("stuff");
            */
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblFeedback.Text = "Working...";
            String input = tbInput.Text;
            String output = tbOutput.Text;

            if (input != "" && output != "")
            {
                CsvReader csv = new CsvReader(new StreamReader(input), true);
                var xmlWriter = XmlWriter.Create(output);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("root");
                //lblFeedback.Text = "root";
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
                        //xmlWriter.WriteStartElement(headers[i]);
                        //xmlWriter.WriteAttributeString(headers[i], csv[i]);
                        xmlWriter.WriteElementString(headers[i], csv[i]);
                    }
                    //trim ending ", " from point
                    point = point.Substring(0, point.Length - 2);
                    xmlWriter.WriteStartElement("Point");
                    xmlWriter.WriteElementString("Coordinates", point);
                    xmlWriter.WriteEndElement(); //Point
                    xmlWriter.WriteEndElement(); //Placemark
                    point = "";
                }

                xmlWriter.WriteEndElement(); // root
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                lblFeedback.Text = "File converted";
            }
        }
    }
}


/*
                xmlWriter.WriteStartElement("root");
                xmlWriter.WriteStartElement("data");
                xmlWriter.WriteStartElement("entry");
                xmlWriter.WriteAttributeString("attrib1", "value1");
                xmlWriter.WriteAttributeString("attrib2", "value2");
                xmlWriter.WriteEndElement(); // entry
                xmlWriter.WriteStartElement("entry");
                xmlWriter.WriteAttributeString("attrib1", "value1");
                xmlWriter.WriteAttributeString("attrib2", "value2");
                xmlWriter.WriteEndElement(); // entry
                xmlWriter.WriteEndElement(); // data
                xmlWriter.WriteEndElement(); // root
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();


create xml-object having as properties the CSV column headings
create xml-object having as properties the current row's values

determine relevant bits according to predetermined list (TBD by Jeff/Tucker)
read in CSV, line by line, and write relevant parts to KML
*/
