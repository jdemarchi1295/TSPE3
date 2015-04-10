using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    //Technique from http://www.mostthingsweb.com/2011/01/hiding-tab-headers-on-a-tabcontrol-in-c/
    public class CustomTab : TabControl
    {
        protected override void WndProc(ref Message m)
        {
        // Hide tabs by trapping the TCM_ADJUSTRECT message
        if (m.Msg == 0x1328 && !DesignMode)
            m.Result = (IntPtr)1;
        else
            base.WndProc(ref m);
        }
    }
}
