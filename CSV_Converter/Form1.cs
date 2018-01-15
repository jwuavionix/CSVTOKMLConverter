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
        int filesPerKB = 15; //obtained by dividing number of records by filesize in KB
        int kbPerStep = 100; //obtained arbitrarily

        int recordsRead; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy
        CsvReader csv;
        XmlWriter xmlWriter;
        String point;
        bool isCoord;

        int fieldCount;
        String[] headers;

        //kml format does not allow for colons, and this array is used after they are converted to dashes
        //please change colons to dashes if you are adding possible coordinate header names to this list
        String[] coordHeaders = {"GPS--Lat","GPS--Lon","GPS--Alt"};

        public CsvToKmlConverter()
        {
            InitializeComponent();
            lblFeedback.Text = "";
            pBar.Visible = true;
        }

        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbOutput.Text = "";
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialogInput.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbInput.Text = openFileDialogInput.FileName;
                tbInput.BackColor = Color.WhiteSmoke;
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
            String filepath;
            recordsRead = 0; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy

            if (input == "")
            {
                lblFeedback.Text = "Input file field is empty.";
                tbInput.BackColor = Color.IndianRed;
            }
            else
            {
                pBar.Minimum = 0;
                int steps = (int) new System.IO.FileInfo(input).Length / (kbPerStep * 1000);
                pBar.Maximum = (steps>1) ? steps : 2;
                pBar.Value = 1;
                pBar.Step = 1;
                pBar.PerformStep();

                if (output == "")
                {
                    filepath = Path.GetDirectoryName(input) + "\\" + Path.GetFileNameWithoutExtension(input) + ".kml";
                    output = tbOutput.Text = filepath;
                }

                try
                {
                    csv = new CsvReader(new StreamReader(input), true);
                    xmlWriter = XmlWriter.Create(output);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
                    xmlWriter.WriteStartElement("Folder");
                    point = "";
                    isCoord = false;

                    fieldCount = csv.FieldCount;
                    headers = csv.GetFieldHeaders();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        headers[i] = headers[i].Replace(":", "-");
                    }

                    while (csv.ReadNextRecord())
                    {
                        processRecord();
                    }

                    xmlWriter.WriteEndElement(); //Folder
                    xmlWriter.WriteEndElement(); //kml
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
                    lblFeedback.Text = "File converted";

                    xmlWriter.Close();
                }
                catch(IOException IOex)
                {
                    String message = "Cannot access file. May be in use or no longer available. Please check and try again.\n\nError code: " + IOex.ToString();
                    String caption = "Error accessing file.";
                    DisplayError(message, caption);
                    pBar.Value = 0;
                }
                catch(UnauthorizedAccessException unauthEx)
                {
                    String message = "Cannot access file. Insufficient read-write privileges. Please check and try again.\n\nError code: " + unauthEx.ToString();
                    String caption = "Error accessing file.";
                    DisplayError(message, caption);
                    pBar.Value = 0;
                }
            }
        }

        //
        //METHODS
        //
        private void DisplayError(String msg, String cap)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(msg, cap, buttons);
        }

        private void processRecord()
        {
            xmlWriter.WriteStartElement("Placemark");

            for (int i = 0; i < fieldCount; i++)
            {
                foreach (String header in coordHeaders)
                {
                    isCoord = headers[i].Contains(header) ? true : isCoord;
                    //considering breaking if match is found; especially if list becomes long
                }

                if (isCoord)
                {
                    point += csv[i] + ", ";
                    isCoord = false;
                }

                try
                {
                    xmlWriter.WriteElementString(headers[i], csv[i]);
                }
                catch (ArgumentException argEx)
                {
                    string message = "Bad headers in input file. Aborting conversion.\n\nError code: " + argEx.ToString();
                    string caption = "Error in input file.";
                    DisplayError(message, caption);
                    xmlWriter.Close();
                    return;
                }
                catch (InvalidOperationException invOpEx)
                {
                    string message = "Unknown error. Aborting conversion.\n\nError code: " + invOpEx.ToString();
                    string caption = "Unknown error.";
                    DisplayError(message, caption);
                    xmlWriter.Close();
                    return;
                }
            }

            //dirty hack to hopefully give ProgressBar some measure of accuracy
            //moved from inside for loop 1/14/18 due to logic error (was running for every field, not every record)
            recordsRead++;
            if (recordsRead >= filesPerKB * kbPerStep)
            {
                recordsRead = 0;
                pBar.PerformStep();
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
        }
    }
}
