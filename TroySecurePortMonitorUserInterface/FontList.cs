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
    public partial class FontList : Form
    {
        public List<string> SelectedFontName = new List<string>();

        public FontList()
        {
            InitializeComponent();
        }

        private void FontList_Load(object sender, EventArgs e)
        {
            foreach (TroyFontGlyphMap tfgm in Globals.FontGlyphMapList.FontGlyphMapList)
            {
                string[] rowStr = {tfgm.FontName,
                                   tfgm.FontGlyphFileName};
                dgvFontList.Rows.Add(rowStr);
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (dgvFontList.SelectedRows.Count > 0)
            {
                for (int cntr = 0; cntr < dgvFontList.SelectedRows.Count; cntr++)
                {
                    SelectedFontName.Add(dgvFontList.SelectedRows[cntr].Cells[0].Value.ToString());
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("No rows selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedFontName.Clear();
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNewFontToList anftl = new AddNewFontToList();
            anftl.ShowDialog();
            if (anftl.FontName != "")
            {
                string[] rowStr = { anftl.FontName,
                                    anftl.GlyphName};
                dgvFontList.Rows.Add(rowStr);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvFontList.SelectedRows.Count > 0)
            {
                AddNewFontToList anftl = new AddNewFontToList();
                anftl.FontName = dgvFontList.SelectedRows[0].Cells[0].Value.ToString();
                anftl.GlyphName = dgvFontList.SelectedRows[0].Cells[1].Value.ToString();
                anftl.ShowDialog();
                dgvFontList.SelectedRows[0].Cells[0].Value = anftl.FontName;
                dgvFontList.SelectedRows[0].Cells[1].Value = anftl.GlyphName;
            }
        }
    }
}
