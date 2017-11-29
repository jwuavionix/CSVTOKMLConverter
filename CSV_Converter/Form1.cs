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
            //label1.Text = "testing";
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                tbInput.Text = openFileDialog1.FileName;

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
            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                tbOutput.Text = openFileDialog1.FileName;
            }

            /*
            String temp = "";
            // open the file "data.csv" which is a CSV file with headers
            using (CsvReader csv =
                   new CsvReader(new StreamReader("in.csv"), true))
            {
                int fieldCount = csv.FieldCount;
                label2.Text = fieldCount.ToString();
                
                string[] headers = csv.GetFieldHeaders();
                while (csv.ReadNextRecord())
                {
                    for (int i = 0; i < fieldCount; i++)
                    temp = string.Format("{0} = {1};", headers[i], csv[i]);
                    label1.Text = temp; // string.Format("{0} = {1};", csv[i], csv[i]);

                    //writer.Write("stuff");
                }
            }
            StreamWriter writer = new StreamWriter("out.kml", false);
            writer.Write("stuff");
            */
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            String input = tbInput.Text;
            String output = tbOutput.Text;

            if (input != "" && output != "")
            {
                var xmlWriter = XmlWriter.Create(output);
                xmlWriter.WriteStartDocument();
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
            }
        }
    }
}


/*
create xml-object having as properties the CSV column headings
create xml-object having as properties the current row's values

determine relevant bits according to predetermined list (TBD by Jeff/Tucker)
read in CSV, line by line, and write relevant parts to KML
*/
