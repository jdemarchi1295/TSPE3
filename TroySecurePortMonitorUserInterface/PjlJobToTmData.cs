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
    public partial class PjlJobToTmData : Form
    {
        public List<string> PjlToTmList = new List<string>();

        public PjlJobToTmData()
        {
            InitializeComponent();
        }

        private void PjlJobToTmData_Load(object sender, EventArgs e)
        {
            dgvPjlToTmMap.Rows.Clear();
            if (PjlToTmList.Count > 0)
            {
                UpdateGrid();
            }
            else
            {
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddTextMap_Click(object sender, EventArgs e)
        {
            TmTextMap ttm = new TmTextMap();
            ttm.PjlJobName = "";
            ttm.TmString = "";
            ttm.ShowDialog();
            if (ttm.ReturnOk)
            {
                if ((ttm.PjlJobName != "") && (ttm.TmString != ""))
                {
                    string[] strRow = { ttm.PjlJobName, ttm.TmString };
                    dgvPjlToTmMap.Rows.Add(strRow);
                }
            }
        }

        private void UpdateGrid()
        {
            foreach (string str in PjlToTmList)
            {
                int cntr = 0;
                string col1 = "", col2 = "";
                foreach (string spl in str.Split(','))
                {
                    if (cntr == 0)
                    {
                        col1 = spl;
                    }
                    else
                    {
                        col2 = spl;
                    }
                    cntr++;
                }
                string[] strRow = { col1, col2 };
                dgvPjlToTmMap.Rows.Add(strRow);
            }

        }

        private void btnDelTextMap_Click(object sender, EventArgs e)
        {
            if (dgvPjlToTmMap.SelectedRows.Count > 0)
            {
                dgvPjlToTmMap.Rows.Remove(dgvPjlToTmMap.SelectedRows[0]);
            }
        }

        private void btnEditTextMap_Click(object sender, EventArgs e)
        {
            if (dgvPjlToTmMap.SelectedRows.Count > 0)
            {
                string Pjl, TmText;
                Pjl = dgvPjlToTmMap.SelectedRows[0].Cells[0].Value.ToString();
                TmText = dgvPjlToTmMap.SelectedRows[0].Cells[1].Value.ToString();
                TmTextMap ttm = new TmTextMap();
                ttm.PjlJobName = Pjl;
                ttm.TmString = TmText;
                ttm.ShowDialog();
                if (ttm.ReturnOk)
                {
                    if ((ttm.PjlJobName != "") && (ttm.TmString != ""))
                    {
                        dgvPjlToTmMap.SelectedRows[0].Cells[0].Value = ttm.PjlJobName;
                        dgvPjlToTmMap.SelectedRows[0].Cells[1].Value = ttm.TmString;
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            PjlToTmList.Clear();
            for (int cntr = 0; cntr < dgvPjlToTmMap.Rows.Count; cntr++)
            {
                string listStr = "";
                listStr = dgvPjlToTmMap.Rows[cntr].Cells[0].Value.ToString();
                listStr += ",";
                listStr += dgvPjlToTmMap.Rows[cntr].Cells[1].Value.ToString();
                PjlToTmList.Add(listStr);
            }
            this.Close();
        }

       

        
    }
}
