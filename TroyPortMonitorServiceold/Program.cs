using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TroyPortMonitorService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if (!DEBUG)
            {            
                ServiceBase[] ServicesToRun = new ServiceBase[] 
                {
                    new TroyPortMonService() 
                };
                ServiceBase.Run(ServicesToRun);
            }
#else
            TroyPortMonService service = new TroyPortMonService();
            service.StartService(args);
#endif

        }
    }
}
