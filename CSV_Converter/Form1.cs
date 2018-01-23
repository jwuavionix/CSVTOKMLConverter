﻿using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;
using System.Xml;

namespace CSV_Converter
{
    public partial class CsvToKmlConverter : Form
    {
        //kml format does not allow for colons, and this array is used after they are converted to dashes
        //please change colons to dashes if you are adding possible coordinate header names to this list
        String[] coordHeaders = { "GPS--Lat","GPS--Lon","GPS--Alt" };
        String[] keyElements = { "Name", "Description", "Timestamp", "StyleURL", "Point" };

        //error messages
        String errCapFileAcc = "Error accessing file.";
        String errMsgFileUnavailable = "Cannot access file. May be in use or no longer available. Please check and try again.\n\nError code: ";
        String errMsgFilePriv = "Cannot access file. Insufficient read-write privileges. Please check and try again.\n\nError code: ";
        String errCapFileMalformed = "Error in input file.";
        String errMsgBadHeaders = "Bad headers in input file. Aborting conversion.\n\nError code: ";
        String errCapUnknown = "Unknown error.";
        String errMsgUnknown = "Unknown error. Aborting conversion.\n\nError code: ";

        //ProgressBar tinkering
        int filesPerKB = 15; //obtained by dividing number of records by filesize in KB
        int kbPerStep = 100; //obtained arbitrarily
        int recordsRead; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy

        CsvReader csv;
        XmlWriter xmlWriter;
        String[] headers;
        String input, output, filepath;
        String name, desc, time, timestamp, style, point; //for storing data to be put in critical tags
        DateTime t1;
        bool isCoord; //replaced by dataType
        String dataType;
        int fieldCount;



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
            input = tbInput.Text;
            output = tbOutput.Text;

            if (input == "")
            {
                lblFeedback.Text = "Input file field is empty.";
                tbInput.BackColor = Color.IndianRed;
            }
            else
            {
                initializeVariables();

                if (output == "")
                {
                    filepath = Path.GetDirectoryName(input) + "\\" + Path.GetFileNameWithoutExtension(input) + ".kml";
                    output = tbOutput.Text = filepath;
                    //considering changing Clear button to a checkbox, to process multiple files in a row with auto-generated names
                }

                try
                {
                    beginDocument();

                    fieldCount = csv.FieldCount;
                    headers = csv.GetFieldHeaders();

                    for (int i = 0; i < fieldCount; i++)
                    {

                        headers[i] = XmlConvert.EncodeName(headers[i]);
                        headers[i] = headers[i].Replace(":", "-");
                    }

                    while (csv.ReadNextRecord()) //considering adding a boolean xmlOpen or xmlInError
                    {
                        processRecord();
                    }

                    finishDocument();
                }
                catch(IOException IOex)
                {
                    DialogResult result = MessageBox.Show(errMsgFileUnavailable + IOex.ToString(), errCapFileAcc, MessageBoxButtons.OK);
                    pBar.Value = 0;
                }
                catch (UnauthorizedAccessException unauthEx)
                {
                    DialogResult result = MessageBox.Show(errMsgFilePriv + unauthEx.ToString(), errCapFileAcc, MessageBoxButtons.OK);
                    pBar.Value = 0;
                }
            }
        }





        //=======
        //METHODS
        //=======

        private void beginDocument()
        {
            csv = new CsvReader(new StreamReader(input), true);
            xmlWriter = XmlWriter.Create(output);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
            xmlWriter.WriteStartElement("Folder");
        }

        private void finishDocument()
        {
            xmlWriter.WriteEndElement(); //Folder
            xmlWriter.WriteEndElement(); //kml
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            pBar.Value = pBar.Maximum;
            lblFeedback.Text = "File converted";
        }

        private void initializeVariables()
        {
            name = desc = time = point = "";
            isCoord = false;

            recordsRead = 0; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy
            pBar.Minimum = 0;
            int steps = (int)new System.IO.FileInfo(input).Length / (kbPerStep * 1000);
            pBar.Maximum = (steps > 1) ? steps : 2;
            pBar.Value = 1;
            pBar.Step = 1;
            pBar.PerformStep(); //to at least let the user know their button press was accepted
        }

        private void processRecord()
        {
            xmlWriter.WriteStartElement("Placemark");
            double seconds = 0;

            for (int i = 0; i < fieldCount; i++)
            {
                desc += headers[i] + ": " + csv[i] + "  <br>";

                //if coordinate
                foreach (String coordName in coordHeaders)
                {
                    if ( headers[i].Contains(coordName) ) {
                        dataType = "coord";
                        break;
                    }
                }
                
                //if timestamp - assumes csv value will always be a straight number
                if (!isCoord)
                {
                    if(headers[i].Contains("TimeStamp") && double.TryParse(csv[i], out seconds) ) { dataType = "time"; }
                }

                switch(dataType)
                {
                    case "coord" :
                        point += csv[i] + ", ";
                        break;
                    case "time" :
                        timestamp = new DateTime().AddSeconds(seconds).ToString("o");
                        timestamp = timestamp.Remove(timestamp.Length - 4);
                        time = timestamp.Substring(11);
                        break;
                    default : break;
                }
                dataType = "";

                /*
                DateTime dt = new DateTime().AddMilliseconds(1512597795);
                String initial = dt.ToString("o");
                String shortened = initial.Remove(23, 4);

                t1 = TimeSpan.FromMilliseconds(1512597795);
                name = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                    t1.Hours,
                    t1.Minutes,
                    t1.Seconds,
                    t1.Milliseconds);*/

                try
                {
                    xmlWriter.WriteElementString(headers[i], csv[i]);
                }
                catch (ArgumentException argEx)
                {
                    DialogResult result = MessageBox.Show(errMsgBadHeaders + argEx.ToString(), errCapFileMalformed, MessageBoxButtons.OK);
                    xmlWriter.Close();
                    return;
                }
                catch (InvalidOperationException invOpEx)
                {
                    DialogResult result = MessageBox.Show(errMsgUnknown + invOpEx.ToString(), errCapUnknown, MessageBoxButtons.OK);
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

            //name = time, no need to have 2 variables for it

            desc = time + "<br>" + desc;

            xmlWriter.WriteElementString("name", time);

            xmlWriter.WriteStartElement("description");
            xmlWriter.WriteCData(desc);
            xmlWriter.WriteEndElement(); //description

            xmlWriter.WriteStartElement("TimeStamp");
            xmlWriter.WriteElementString("when", timestamp);
            xmlWriter.WriteEndElement(); //TimeStamp

            xmlWriter.WriteElementString("styleUrl", "#pUAC");

            xmlWriter.WriteStartElement("Point");
            xmlWriter.WriteElementString("coordinates", point);
            xmlWriter.WriteEndElement(); //Point

            xmlWriter.WriteEndElement(); //Placemark
            point = desc = "";
        } //end processRecord()
    }
}
