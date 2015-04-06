using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TroySecurePortMonitorUserInterface
{
    public partial class PrinterMap : Form
    {
        public string PrinterName = "";
        public string MapString = "";
        public bool ReturnOk = false;

        public PrinterMap()
        {
            InitializeComponent();
        }

        private void PrinterMap_Load(object sender, EventArgs e)
        {
            txtMapString.Text = MapString;
            cboPrinterName.Text = PrinterName;

            foreach (string strPrinter in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cboPrinterName.Items.Add(strPrinter);
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            MapString = txtMapString.Text;
            PrinterName = cboPrinterName.Text;
            ReturnOk = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReturnOk = false;
            this.Close();
        }


    }
}
