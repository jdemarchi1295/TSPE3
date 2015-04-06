using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Troy.PortMonitor.Core.XmlConfiguration;

namespace TroySecurePortMonitorUserInterface
{
    public partial class AddNewFontToList : Form
    {
        public string FontName = "";
        public string GlyphName = "";

        public AddNewFontToList()
        {
            InitializeComponent();
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFontName.Text = fontDialog1.Font.Name;
            }
        }

        private void AddNewFontToList_Load(object sender, EventArgs e)
        {
            txtFontName.Text = FontName;
            txtGlyphFile.Text = GlyphName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.InitialDirectory = Globals.appDataPath;
            openFileDialog1.Title = "Select a Glyph Map file...";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtGlyphFile.Text = openFileDialog1.SafeFileName;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((txtFontName.Text == "") || (txtGlyphFile.Text == ""))
            {
                MessageBox.Show("Must enter a font name and glyph file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                FontName = txtFontName.Text;
                GlyphName = txtGlyphFile.Text;
                bool entryFound = false;
                foreach (TroyFontGlyphMap tfgm in Globals.FontGlyphMapList.FontGlyphMapList)
                {
                    if (tfgm.FontName == FontName)
                    {
                        entryFound = true;
                        if (tfgm.FontGlyphFileName != GlyphName)
                        {
                            tfgm.FontGlyphFileName = GlyphName;
                        }
                    }
                }
                if (!entryFound)
                {
                    TroyFontGlyphMap tfgm = new TroyFontGlyphMap();
                    tfgm.FontName = FontName;
                    tfgm.FontGlyphFileName = GlyphName;
                    Globals.FontGlyphMapList.FontGlyphMapList.Add(tfgm);
                }
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FontName = "";
            GlyphName = "";
            this.Close();
        }
    }
}
