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
        //String[] coordHeaders = { "gps--lat", "gps--lon", "gps--alt" };
        String[] coordHeaders = { "lat", "lon", "alt" };
        String[] keyElements = { "Name", "Description", "Timestamp", "StyleURL", "Point" };

        //styling of icon used by Google Earth to display Placemarks
        String pmStyleNameNormal = "psUAT_AirborneCompliant_Normal";
        String pmStyleNameHover = "psUAT_AirborneCompliant_Highlight";
        String pmColor = "ff00ffff";
        String pmIconSizeNormal = "0.275";
        String pmIconSizeHover = "0.4";
        String pmIcon = "http://maps.google.com/mapfiles/kml/pal2/icon26.png";
        String pmLabelSizeNormal = "0";
        String pmLabelSizeHover = "0.75";
        String pmBalloonStyle = "<p align=\"left\" style=\"white - space:nowrap); \"><font size=\" + 1\"><b>$[name]</b></font></p><p align=\"left\">$[description]</p>";
        String pmStyleID = "pUAC";
        String pmStyleURLNormal = "#psUAT_AirborneCompliant_Normal";
        String pmStyleURLHover = "#psUAT_AirborneCompliant_Highlight";

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
        int kbPerStep = 100; //affects "resolution" of progress bar
        int recordsRead; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy

        CsvReader csv;
        XmlWriter xmlWriter;
        String[] headers;
        String input, output, filepath;
        String name, desc, time, timestamp, style, point, allCoords; //for storing data to be put in critical tags; allCoords stores list of all coordinates to add to EOF
        //int timeIndex, latIndex, lonIndex, altIndex;  //planning a change from how field types are currently determined (i.e., coordinates, timestamp)
        //DateTime t1;
        //bool isCoord; //replaced by dataType
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
                        headers[i] = headers[i].ToLower();
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

        private void writeStylingHeaders(String name, String color, String iconSize, String icon, String labelSize, String bStyle)
        {
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Style", "id=" + name);
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("IconStyle");
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Color");
            xmlWriter.WriteRaw(color); xmlWriter.WriteEndElement(); //Color

            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Scale");
            xmlWriter.WriteRaw(iconSize); xmlWriter.WriteEndElement(); //Scale

            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Icon");
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("href");
            xmlWriter.WriteRaw(icon); xmlWriter.WriteEndElement(); //href

            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //Icon
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //IconStyle
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("LabelStyle");
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Scale");
            xmlWriter.WriteRaw(labelSize); xmlWriter.WriteEndElement(); //Scale

            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //LabelStyle
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("BalloonStyle");
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("Text");
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteCData(bStyle);
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //Text
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //BalloonStyle
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteEndElement(); //Style
            xmlWriter.WriteRaw("\n");
        }

        private void writeStylingHeaders(String id, String urlNormal, String urlHover)
        {
            xmlWriter.WriteRaw("\n"); xmlWriter.WriteStartElement("StyleMap", "id=" + pmStyleID);
        }
        private void beginDocument()
        {
            csv = new CsvReader(new StreamReader(input), true);
            xmlWriter = XmlWriter.Create(output);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("Document");
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteElementString("name", Path.GetFileNameWithoutExtension(input));
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteElementString("open", "1");
            xmlWriter.WriteRaw("\n");

            writeStylingHeaders(pmStyleNameNormal, pmColor, pmIconSizeNormal, pmIcon, pmLabelSizeNormal, pmBalloonStyle);
            writeStylingHeaders(pmStyleNameHover,  pmColor, pmIconSizeHover,  pmIcon, pmLabelSizeHover,  pmBalloonStyle);

            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("Folder");
            xmlWriter.WriteRaw("\n");
        }
        
        private void finishDocument()
        {
            xmlWriter.WriteEndElement(); //Folder
            writeAllCoords();
            xmlWriter.WriteEndElement(); //Document
            xmlWriter.WriteEndElement(); //kml
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            pBar.Value = pBar.Maximum;
            lblFeedback.Text = "File converted";
        }

        private void initializeVariables()
        {
            name = desc = time = point = "";
            //isCoord = false;

            recordsRead = 0; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy
            pBar.Minimum = 0;
            int steps = (int)new System.IO.FileInfo(input).Length / (kbPerStep * 1000);
            pBar.Maximum = (steps > 1) ? steps : 2;
            pBar.Value = 1;
            pBar.Step = 1;
            pBar.PerformStep(); //before any actual progress; meant to at least let the user know their button press was accepted
        }

        private void processRecord()
        {
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("Placemark");
            xmlWriter.WriteRaw("\n");
            double seconds = 0;

            for (int i = 0; i < fieldCount; i++)
            {
                dataType = "";
                desc += headers[i] + ": " + csv[i] + "  <br>";

                //if coordinate
                foreach (String coordName in coordHeaders)
                {
                    if ( headers[i].Contains(coordName) ) { dataType = "coord"; break; }
                }
                
                //if timestamp
                if (dataType != "coord"
                    && headers[i].Contains("timestamp")
                    && double.TryParse(csv[i], out seconds) )
                { dataType = "time"; }

                switch(dataType)
                {
                    case "coord" :
                        point += csv[i] + ", ";
                        break;
                    case "time" :
                        timestamp = new DateTime().AddSeconds(seconds).ToString("o");
                        //timestamp = new DateTime((long)seconds).ToString("o");
                        timestamp = timestamp.Remove(timestamp.Length - 4);
                        time = timestamp.Substring(11);
                        break;
                    default : break;
                }

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
                    if( dataType == "" ) { xmlWriter.WriteElementString(headers[i], csv[i]); }
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
                allCoords += point + " ";
            }

            //name = time, no need to have 2 variables for it

            desc = time + "<br>" + desc;

            if(time != "")
            {
                xmlWriter.WriteRaw("\n");
                xmlWriter.WriteElementString("name", time);
            }

            if(desc != "")
            {
                xmlWriter.WriteRaw("\n");
                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteCData(desc);
                xmlWriter.WriteEndElement(); //description
            }

            if (timestamp != "")
            {
                xmlWriter.WriteRaw("\n");
                xmlWriter.WriteStartElement("TimeStamp");
                xmlWriter.WriteElementString("when", timestamp);
                xmlWriter.WriteEndElement(); //TimeStamp
                xmlWriter.WriteRaw("\n");
            }
            
            xmlWriter.WriteElementString("styleUrl", "#pUAC");
            xmlWriter.WriteRaw("\n");

            if(point != "")
            {
                xmlWriter.WriteStartElement("Point");
                xmlWriter.WriteElementString("coordinates", point);
                xmlWriter.WriteEndElement(); //Point
                xmlWriter.WriteRaw("\n");
            }

            xmlWriter.WriteEndElement(); //Placemark
            xmlWriter.WriteRaw("\n");
            point = desc = "";
        } //end processRecord()


        private void writeAllCoords()
        {
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("Placemark");
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteElementString("name", "UAT 3D GeoAlt [100ft Offset]");
            xmlWriter.WriteElementString("styleUrl", "Style");

            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteStartElement("LineString");
            xmlWriter.WriteElementString("extrude", "1");
            xmlWriter.WriteElementString("altitudeMode", "absolute");
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteElementString("coordinates", allCoords);
            xmlWriter.WriteRaw("\n");
            xmlWriter.WriteElementString("name", "3D");
            xmlWriter.WriteEndElement(); //LineString
            xmlWriter.WriteEndElement(); //Placemark
            xmlWriter.WriteRaw("\n");
        }

    }
}
