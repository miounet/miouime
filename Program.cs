using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
namespace MiouIME
{
    static class Program
    {
        public static MiouIME MIme=null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            int iProcessNum = 0;

            foreach (Process singleProc in Process.GetProcesses())
            {
                if (singleProc.ProcessName == Process.GetCurrentProcess().ProcessName)
                {
                    iProcessNum += 1;
                }
            }

            if (iProcessNum <= 1)
            {
                //不要重复运行程序
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MIme = new MiouIME();
                Application.Run(MIme);
            }

        }
    }
}
