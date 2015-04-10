using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace TroySecurePortMonitorUserInterface
{
    public partial class TestDataGrid : Form
    {
        public string filePath = "";

        public TestDataGrid()
        {
            InitializeComponent();
        }

        private void TestDataGrid_Load(object sender, EventArgs e)
        {
            /*  //Test using anonymous type, worked but not what I'm looking for 
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + "DataCaptureConfiguration.xml");

            XmlNodeReader nr = new XmlNodeReader(xDoc);
            nr.MoveToContent();
            XElement xRoot = XElement.Load(nr);

            var xmlquery = from dc in xRoot.Descendants("DataCaptureConfiguration")
                           select new
                           {
                               CaptureType = dc.Element("DataCapture").Value,
                               RmoveData = dc.Element("RemoveData").Value,
                               PlainText = dc.Element("PlainText").Value
                           };

            dataGridView1.DataSource = xmlquery.ToList();
             */
            //foreach (DataCaptureConfiguration dcc in dcaplist.DataCaptureConfigurationList)
            //{

            //}
        }


    }
}
