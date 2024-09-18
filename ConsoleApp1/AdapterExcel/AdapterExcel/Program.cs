using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Windows.Forms;

namespace AdapterExcel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MediaTypeNames.Application.EnableVisualStyles();
            MediaTypeNames.Application.SetCompatibleTextRenderingDefault(false);
            MediaTypeNames.Application.Run(new Form1());
        }
    }
}
