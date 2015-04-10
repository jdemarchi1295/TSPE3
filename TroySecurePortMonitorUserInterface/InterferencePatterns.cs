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
    public partial class InterferencePatterns : Form
    {
        public int SelectedPattern = 0;
        public int MaxNumberOfPatterns = 0;
        public bool Cancelled = false;

        public InterferencePatterns()
        {
            InitializeComponent();
        }

        private void InterferencePatterns_Load(object sender, EventArgs e)
        {
            if (MaxNumberOfPatterns > 0)
            {
                numIntPattern.Maximum = MaxNumberOfPatterns;
            }

            numIntPattern.Value = Convert.ToDecimal(SelectedPattern);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Cancelled = false;
            SelectedPattern = Convert.ToInt32(numIntPattern.Value);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }


    }
}
