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
        //kml format does not allow for colons, and this array is used after they are converted to dashes
        //please change colons to dashes if you are adding possible coordinate header names to this list
        //String[] coordHeaders = { "gps--lat", "gps--lon", "gps--alt" };
        readonly String[] coordHeaders = { "lat", "lon", "alt" };
        readonly String[] keyElements = { "Name", "Description", "Timestamp", "StyleURL", "Point" };

        //PLACEMARK ICON STYLING
        //normal and hover
        readonly String pmColor = "ff00ffff";
        readonly String pmIcon = "http://maps.google.com/mapfiles/kml/pal2/icon26.png";
        readonly String pmBalloonStyle = "<p align=\"left\" style=\"white - space:nowrap); \"><font size=\" + 1\"><b>$[name]</b></font></p><p align=\"left\">$[description]</p>";
        //normal
        readonly String pmLabelSizeNormal = "0";
        readonly String pmStyleNameNormal = "psUAT_AirborneCompliant_Normal";
        readonly String pmIconSizeNormal = "0.275";
        readonly String pmKeyNormal = "normal";
        //on hover
        readonly String pmLabelSizeHover = "0.75";
        readonly String pmStyleNameHover = "psUAT_AirborneCompliant_Highlight";
        readonly String pmIconSizeHover = "0.4";
        readonly String pmKeyHover = "hover";
        //StyleMap, linking styles for normal and hover states
        readonly String pmStyleID = "pUAC";
        readonly String pmStyleURLNormal = "#psUAT_AirborneCompliant_Normal";
        readonly String pmStyleURLHover = "#psUAT_AirborneCompliant_Highlight";

        //MISC
        readonly String folderName = "Placemarks";
        readonly String defaultToOpen = "1"; //1 to set folder to open by default on load into Google Earth
        readonly String listItemType = "checkHideChildren";
        readonly String bgColor = "00ffffff";
        readonly String maxSnippets = "2";
        readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //ERROR MESSAGES
        readonly String errCapFileAcc = "Error accessing file.";
        readonly String errMsgFileUnavailable = "Cannot access file. May be in use or no longer available. Please check and try again.\n\nError code: ";
        readonly String errMsgFilePriv = "Cannot access file. Insufficient read-write privileges. Please check and try again.\n\nError code: ";
        readonly String errCapFileMalformed = "Error in input file.";
        readonly String errMsgBadHeaders = "Bad headers in input file. Aborting conversion.\n\nError code: ";
        readonly String errCapUnknown = "Unknown error.";
        readonly String errMsgUnknown = "Unknown error. Aborting conversion.\n\nError code: ";

        //PROGRESS BAR FINE TUNING
        readonly int filesPerKB = 15; //obtained by dividing number of records by filesize in KB
        readonly int kbPerStep = 100; //affects "resolution" of progress bar
        int recordsRead; //dirty hack to get the ProgressBar working, hopefully with some measure of accuracy

        //RUNTIME VARIABLES
        CsvReader csv;
        XmlWriter xmlWriter;
        String[] headers;
        String input, output, filepath;
        String name, desc, time, timestamp, point, allCoords; //for storing data to be put in critical tags; allCoords stores list of all coordinates to add to EOF
        String dataType;
        int fieldCount;
        bool mimicInputName = false;



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

        private void tsMenuOutfileNaming_Click(object sender, EventArgs e)
        {
            if(mimicInputName)
            {
                mimicInputName = false;
                tsMenuOutfileNaming.Image = Image.FromFile("unchecked.ico");
                btnClear.Enabled = true;
                BtnOutputBrowse.Enabled = true;
                tbOutput.Enabled = true;
            }
            else
            {
                mimicInputName = true;
                tsMenuOutfileNaming.Image = Image.FromFile("checked.ico");
                btnClear.Enabled = false;
                BtnOutputBrowse.Enabled = false;
                tbOutput.Enabled = false;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblFeedback.Text = "Working...";
            input = tbInput.Text;
            output = mimicInputName ? "" : tbOutput.Text;

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

        private void tsHelpAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }





        //=======
        //METHODS
        //=======

        private void beginDocument()
        {
            csv = new CsvReader(new StreamReader(input), true);
            xmlWriter = XmlWriter.Create(output);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");

            xmlWriter.WriteRaw("\n\n\n"); xmlWriter.WriteStartElement("Document");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteElementString("name", Path.GetFileNameWithoutExtension(input));
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteElementString("open", defaultToOpen);
            xmlWriter.WriteRaw("\n");

            writeStylingHeaders(pmStyleNameNormal, pmColor, pmIconSizeNormal, pmIcon, pmLabelSizeNormal, pmBalloonStyle);
            writeStylingHeaders(pmStyleNameHover,  pmColor, pmIconSizeHover,  pmIcon, pmLabelSizeHover,  pmBalloonStyle);

            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("StyleMap");
                                        xmlWriter.WriteAttributeString("id", pmStyleID);
                        writeStylingHeaders(pmKeyNormal, pmStyleURLNormal);
                        writeStylingHeaders(pmKeyHover,  pmStyleURLHover);
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //StyleMap
            
            xmlWriter.WriteRaw("\n\n\n"); xmlWriter.WriteStartElement("Folder");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("name");
                                        xmlWriter.WriteRaw(folderName);
                                        xmlWriter.WriteEndElement(); //Name
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("open");
                                        xmlWriter.WriteRaw(defaultToOpen);
                                        xmlWriter.WriteEndElement(); //open
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("Style");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("ListStyle");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("listItemType");
                                        xmlWriter.WriteRaw(listItemType);
                                        xmlWriter.WriteEndElement(); //listItemType
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("bgColor");
                                        xmlWriter.WriteRaw(bgColor);
                                        xmlWriter.WriteEndElement(); //bgColor
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("maxSnippetLines");
                                        xmlWriter.WriteRaw(maxSnippets);
                                        xmlWriter.WriteEndElement(); //maxSnippetLines
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //ListStyle
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //Style
        } //end beginDocument()

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
        } //end finishDocument()

        private void initializeVariables()
        {
            name = desc = time = point = "";

            recordsRead = 0;
            pBar.Minimum = 0;
            int steps = (int)new System.IO.FileInfo(input).Length / (kbPerStep * 1000);
            pBar.Maximum = (steps > 1) ? steps : 2;
            pBar.Value = 1;
            pBar.Step = 1;
            pBar.PerformStep(); //before any actual progress; meant to at least let the user know their button press was accepted
        } //end initializeVariables()

        private void processRecord()
        {
            time = desc = timestamp = point = "";

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
                        point += csv[i] + ",";
                        break;
                    case "time":
                        timestamp = epoch.AddSeconds(seconds).ToString("o");
                        timestamp = timestamp.Remove(timestamp.Length - 4);
                        time = timestamp.Substring(11);
                        break;
                    default : break;
                }

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
            recordsRead++;
            if (recordsRead >= filesPerKB * kbPerStep)
            {
                recordsRead = 0;
                pBar.PerformStep();
            }

            if (point.Length > 2)
            {
                point = point.Substring(0, point.Length - 1); //trim ending "," from point
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
        } //end writeAllCoords()

        private void writeStylingHeaders(String name, String color, String iconSize, String icon, String labelSize, String bStyle)
        {
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("Style");
                                        xmlWriter.WriteAttributeString("id", name);
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("IconStyle");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("color");
                                        xmlWriter.WriteRaw(color);
                                        xmlWriter.WriteEndElement(); //Color

            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("scale");
                                        xmlWriter.WriteRaw(iconSize);
                                        xmlWriter.WriteEndElement(); //Scale

            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("Icon");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("href");
                                        xmlWriter.WriteRaw(icon);
                                        xmlWriter.WriteEndElement(); //href

            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //Icon
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //IconStyle
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("LabelStyle");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("scale");
                                        xmlWriter.WriteRaw(labelSize);
                                        xmlWriter.WriteEndElement(); //Scale

            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //LabelStyle
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("BalloonStyle");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("text");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteCData(bStyle);
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //Text
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //BalloonStyle
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //Style
            xmlWriter.WriteRaw("\n");
        } //end writeStylingHeaders()

        private void writeStylingHeaders(String key, String url) //for the <StyleMap> element that links the <Style> elements
        {
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("Pair");
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("key");
                                        xmlWriter.WriteRaw(key);
                                        xmlWriter.WriteEndElement(); //Key
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteStartElement("styleUrl");
                                        xmlWriter.WriteRaw(url);
                                        xmlWriter.WriteEndElement(); //StyleUrl
            xmlWriter.WriteRaw("\n");   xmlWriter.WriteEndElement(); //Pair
        } //end writeStylingHeaders()

    }
}
