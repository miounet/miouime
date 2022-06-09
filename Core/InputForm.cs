using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace MiouIME.Core
{
    public class InputForm
    {
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDCEx", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool RedrawWindow(IntPtr hwnd, ref RECT rcUpdate, IntPtr hrgnUpdate, int flags);

        /// 函数释放设备上下文环境（DC）供其他应用程序使用。      
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        //该函数向指定的窗体添加一个矩形，然后窗口客户区域的这一部分将被重新绘制。 
        [DllImport("user32.dll")]
        public extern static int InvalidateRect(IntPtr hWnd, ref RECT rcUpdate, int bErase);


        public static int Top = 100;
        public static int Left = 100;
        public static int Width = 170;
        public static int Height = 100;

        //public static IntPtr desk = GetDesktopWindow();
        //public static IntPtr deskDC = GetDCEx(desk, IntPtr.Zero, 0x403);
        public static IntPtr deskDC = GetDC(IntPtr.Zero);
        //public static Graphics MyScreen = Graphics.FromHdc(deskDC);

        static string[] inputv = { "" };
        static string[] inputc = { "" };

        public static bool curIsBj = false;
        public static string inputstr = string.Empty;
        public static bool Show = false;

        public static System.Threading.Thread workth = null;
        public static System.Threading.Thread sworkth = null;
        //public static System.Threading.Thread monitoractive = null;
        private static BufferedGraphicsContext context;
        private static BufferedGraphics grafx = null;
        private static BufferedGraphics bjgrafx = null;
        private static Rectangle old_inputg = new Rectangle(-99, -99, 1, 1);
        private static int inT = 15;

        public static bool Break = false;
        public static void DealView()
        {
            Width = MiouIME.inputFrm.Width;
            Height = MiouIME.inputFrm.Height;

            if (InputFrm.inputv != null)
            {
                int count = 0;
                foreach (Control c in MiouIME.inputFrm.panelinput.Controls)
                {
                    if (c.Name.IndexOf("lablevalue_") >= 0) count++;
                }
                inputv = new string[count];
                inputc = new string[count];
                for (int i = 0; i < count; i++)
                {
                    inputv[i] = ((Label)MiouIME.inputFrm.panelinput.Controls.Find("lablevalue_" + i, false)[0]).Text;
                    inputc[i] = ((Label)MiouIME.inputFrm.panelinput.Controls.Find("lablecode_" + i, false)[0]).Text;
                }
            }
        }

        public static IntPtr GetTopDc()
        {
            //return MiouIME.inputFrm.Handle;
            return Core.InputForm.GetDC(IntPtr.Zero);
        }
        public void MonitorActive()
        {
            while (!Break)
            {
                try
                {
                    if (MiouIME.GetForegroundWindow() != MiouIME.ForegroundWindow)
                    {
                        Core.InputForm.deskDC = Core.InputForm.GetTopDc();
                        MiouIME.ForegroundWindow = MiouIME.GetForegroundWindow();
                    }
                }
                catch { }
                System.Threading.Thread.Sleep(400);
            }
        }
        public void ShowWork()
        {
            while (!Break)
            {
                System.Threading.Thread.Sleep(inT);

                try
                {
                    int inputy = InputHelp.SkinFontJG;

                    if (!Show)
                    {
                        if (MiouIME.inputFrm.inputstr.Length > 0 && !MiouIME.inputFrm.Visible) { Show = true; MiouIME.inputFrm.Visible = true; }
                       MiouIME.inputFrm.Hide();
                        //grafx.Graphics.Clear(Color.Transparent);
                        continue;
                    }
                    if (MiouIME.inputFrm.inputstr.Length > 0 && !MiouIME.inputFrm.Visible) { Show = true; MiouIME.inputFrm.Visible = true; }
                    if (!MiouIME.OSWin8)
                    {
                        System.Threading.Thread.Sleep(50);
                        MiouIME.inputFrm.Refresh();
                        continue;
                    }
                    context = BufferedGraphicsManager.Current;
                    context.MaximumBuffer = new Size(Width, Height);

                    DealView();

                    Rectangle inputg = new Rectangle(Left, Top, Width, Height);

                    grafx = context.Allocate(deskDC, inputg);

                    grafx.Graphics.DrawImage(InputFrm.bgimag, new Rectangle(Left, Top, Width, Height));

                    Pen bordpen = new Pen(Color.Red);
                    
                    Rectangle hzrec = new Rectangle(Left, Top, Width - 1, Height - 1);

                    grafx.Graphics.DrawRectangle(bordpen, hzrec);

                    if (MiouIME.inputFrm.ViewType == 0)
                    {
                        横排显示(inputy,false);
                    }
                    else
                    {

                        竖排显示(inputy,false);
                    }
                }
                catch
                {

                }

            }
        }
        public void ShowInput(bool show)
        {
            //deskDC = GetDCEx(desk, IntPtr.Zero, 0x403);

            Show = show;

            if (workth == null)
            {
                workth = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWork));

                workth.Start();
            }
            if (sworkth == null)
            {
                sworkth = new System.Threading.Thread(new System.Threading.ThreadStart(ShowLog));

                sworkth.Start();
            }

            //if (monitoractive == null)
            //{
            //    monitoractive = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorActive));

            //    monitoractive.Start();
            //}
        }

        private static void 横排显示(int inputy,bool empty)
        {
  
            SolidBrush bstring = new SolidBrush(Core.InputHelp.Skinbstring);
            SolidBrush bcstring = new SolidBrush(Core.InputHelp.Skinbcstring);
            SolidBrush fbcstring = new SolidBrush(Core.InputHelp.Skinfbcstring);

            string ins = MiouIME.inputFrm.lbinputstr.Text;
            int len = Left + 3;
            grafx.Graphics.DrawString(ins, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize, FontStyle.Bold), bstring, new Point(Left + 3, Top + 4));
            if (MiouIME.inputFrm.valuearry != null && MiouIME.inputFrm.valuearry.Length > 0)
                grafx.Graphics.DrawString(string.Format("{0}/{1}", MiouIME.inputFrm.pageNum
                   , MiouIME.inputFrm.valuearry.Length / MiouIME.inputFrm.pageSize + 1), new Font("", 10F), bstring, new Point(Left + Width - 40, Top + 4));
            
            for (int i = 0; i < inputv.Length; i++)
            {
                if (string.IsNullOrEmpty(inputv[i])) break; ;
                string v = inputv[i].Substring(2);
                int vw = InputFrm.inputv[i].Width;
                string pos = i == 9 ? "0." : (i + 1).ToString() + ".";
                len = Left + 3 + InputFrm.inputv[i].Left;
                if (i == 0)
                    grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize), fbcstring, new Point(len, Top + inputy));
                else
                    grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize), bstring, new Point(len, Top + inputy));

                len = Left + 1 + InputFrm.inputc[i].Left;
                grafx.Graphics.DrawString(inputc[i], new Font("宋体", InputHelp.SkinFontSize - 1), bcstring, new Point(len, (Top + inputy) - 1));
            }

            grafx.Render(deskDC);
        }

    
        private static void 竖排显示(int inputy, bool empty)
        {
            SolidBrush bstring = new SolidBrush(Core.InputHelp.Skinbstring);
            SolidBrush bcstring = new SolidBrush(Core.InputHelp.Skinbcstring);
            SolidBrush fbcstring = new SolidBrush(Core.InputHelp.Skinfbcstring);

            string ins = MiouIME.inputFrm.lbinputstr.Text;// +" " + MiouIME.ForegroundWindow.ToString();
            grafx.Graphics.DrawString(ins, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize, FontStyle.Bold), bstring, new Point(Left + 3, Top + 4));
            if (MiouIME.inputFrm.valuearry != null && MiouIME.inputFrm.valuearry.Length > 0)
                grafx.Graphics.DrawString(string.Format("{0}/{1}", MiouIME.inputFrm.pageNum
                   , (MiouIME.inputFrm.valuearry.Length % MiouIME.inputFrm.pageSize == 0 ? MiouIME.inputFrm.valuearry.Length / MiouIME.inputFrm.pageSize : MiouIME.inputFrm.valuearry.Length / MiouIME.inputFrm.pageSize + 1)
                   ), new Font("", 10F), bstring, new Point(Left + Width - 40, Top + 4));
            for (int i = 0; i < inputv.Length; i++)
            {
                if (string.IsNullOrEmpty(inputv[i])) break; ;
                string v = inputv[i].Substring(2);
                int vw = InputFrm.inputv[i].Width;
                string pos = i == 9 ? "0." : (i + 1).ToString() + ".";
                if (i == 0)
                    grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize), fbcstring, new Point(Left + 3, Top + inputy + i * InputHelp.SkinFontH));
                else
                    grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, InputHelp.SkinFontSize), bstring, new Point(Left + 3, Top + inputy + i * InputHelp.SkinFontH));

                grafx.Graphics.DrawString(inputc[i], new Font("宋体", InputHelp.SkinFontSize - 1), bcstring, new Point(Left + 3 + vw, (Top + inputy + i * InputHelp.SkinFontH) - 1));
            }

            grafx.Render(deskDC);

        }

        public static void ClearShow()
        {
            Show = false;
        }

        static Image bglogimag = Image.FromFile(Core.InputHelp.状态栏Log());
        private void ShowLog()
        {

            while (!Break)
            {
                System.Threading.Thread.Sleep(inT);
               
                try
                {
                    if (!MiouIME.isActiveInput) { if(Program.MIme.Visible) Program.MIme.Hide(); continue; }
                    if (!MiouIME.OSWin8)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    context = BufferedGraphicsManager.Current;

                    Rectangle inputg = new Rectangle(Program.MIme.Left + 2, Program.MIme.Top + 3, 20, 19);
                    bjgrafx = context.Allocate(deskDC, inputg);
                    bjgrafx.Graphics.DrawImage(bglogimag, new Rectangle(Program.MIme.Left + 2, Program.MIme.Top + 3, 20, 19));

                    bjgrafx.Render(deskDC);
                }
                catch { }
            }

        }
    }
}
