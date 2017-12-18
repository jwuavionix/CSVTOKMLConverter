using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;
using System.Xml;

namespace CSV_Converter
{
    public partial class CsvToKmlConverter : Form
    {
        //ProgressBar tinkering
        int filesPerKB = 15;
        int kbPerStep = 100;

        String[] coordHeaders = {"GPS--Lat","GPS--Lon","GPS--Alt"};

        public CsvToKmlConverter()
        {
            InitializeComponent();
            lblFeedback.Text = "";
            pBar.Visible = true;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            tbInput.BackColor = Color.White;
            DialogResult result = openFileDialogInput.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbInput.Text = openFileDialogInput.FileName;
            }
        }

        private void BtnOutputBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialogOutput.ShowDialog();
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
            int recordsRead = 0; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy

            pBar.Minimum = 1;
            pBar.Maximum = (int) new System.IO.FileInfo(input).Length / kbPerStep * 1000;
            pBar.Value = 1;
            pBar.Step = 1;

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

                try
                {
                    CsvReader csv = new CsvReader(new StreamReader(input), true);
                    var xmlWriter = XmlWriter.Create(output);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("root");
                    String point = "";
                    bool isCoord = false;
                    //bool firstCoords; //flag to prevent multiple <Point> elements

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
                            //firstCoords = true;

                            foreach(String header in coordHeaders)
                            {
                                isCoord = headers[i].Contains(header) ? true : isCoord;
                            }

                            if (isCoord)
                            {
                                point += csv[i] + ", ";
                                isCoord = false;
                            }
                            xmlWriter.WriteElementString(headers[i], csv[i]);

                            //dirty hack to hopefully give ProgressBar some measure of accuracy
                            recordsRead++;
                            if (recordsRead >= filesPerKB * kbPerStep)
                            {
                                recordsRead = 0;
                                pBar.PerformStep();
                            }
                        }

                        if (point.Length > 2)
                        {
                            point = point.Substring(0, point.Length - 2); //trim ending ", " from point
                            xmlWriter.WriteStartElement("Point");
                            xmlWriter.WriteElementString("Coordinates", point);
                            xmlWriter.WriteEndElement(); //Point
                        }

                        xmlWriter.WriteEndElement(); //Placemark
                        point = "";
                        //firstCoords = true;
                    } //end while

                    xmlWriter.WriteEndElement(); //root...or Groot?
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
                    lblFeedback.Text = "File converted";
                }
                catch(IOException IOex)
                {
                    string message = "Cannot access file. May be in use or no longer available. Please check and try again.\n\nError code: " + IOex.ToString();
                    string caption = "Error accessing input file.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result = MessageBox.Show(message, caption, buttons);
                }
                catch(UnauthorizedAccessException unauthEx)
                {
                    string message = "Cannot access file. Insufficient read-write privileges. Please check and try again.\n\nError code: " + unauthEx.ToString();
                    string caption = "Error accessing input file.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result = MessageBox.Show(message, caption, buttons);
                }
            }
        }
    }
}
