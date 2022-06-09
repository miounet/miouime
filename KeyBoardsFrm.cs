using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace MiouIME
{
    public partial class KeyBoardsFrm : Form
    {
        public string _KeyBoardsType = "英文音标";
        public List<KeyBoardsPos> keyBoardsPos = null;
        KeyBoardsPos CurKey = null;
        KeyBoardsPos ShiftDown = null;
        List<KeyValueClass> keyValue = new List<KeyValueClass>();

        private Point offset;
        const int SW_SHOWNOACTIVATE = 4;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hWnd, short cmdShow);
        private const int HWND_TOPMOST = -1;
        private const int SWP_NoActiveWINDOW = 0x10;
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        public KeyBoardsFrm()
        {
            InitializeComponent();
            this.TopLevel = true;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
        }

        public string KeyBoardsType
        {
            get { return _KeyBoardsType; }
            set
            {
                _KeyBoardsType = value;
                string appPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
                string[] KeyValuearry = File.ReadAllLines(appPath + "\\KeyBoards\\" + _KeyBoardsType + ".KBD", Encoding.Unicode);
                keyValue.Clear();
                IniData();
                for (int i = 0; i < KeyValuearry.Length; i++)
                {
                    KeyValueClass item = new KeyValueClass();
                    KeyBoardsPos kitem = new KeyBoardsPos();
                    item.KeyCode=KeyValuearry[i].Split('=')[0];
                    
                    string vstr = KeyValuearry[i].Split('=')[1];
                    if (vstr.Length > 0)
                    {
                        item.KeyV1 = vstr.Split(' ')[0];
                         
                        if (vstr.Split(' ').Length > 1)
                        {
                            item.KeyV2 = vstr.Split(' ')[1];
                        }
                    }

                    keyValue.Add(item);


                }
                Thread th = new Thread(new ThreadStart(DrawFontThread));
                th.Start();
            }
        }
        
        public void DrawFont()
        {
            if (keyBoardsPos == null) return;
            Graphics e = this.CreateGraphics();
            #region 画字
            using (System.Drawing.Drawing2D.GraphicsPath path =
new System.Drawing.Drawing2D.GraphicsPath())
            {
                // 绘制文本 
                using (StringFormat f = new StringFormat())
                {
                    foreach (KeyBoardsPos item in keyBoardsPos)
                    {
                        KeyValueClass kvc = GetKeyValueClass(item.KeysCode);
                        if (kvc == null) continue;
                        // 水平居中对齐 
                        f.Alignment = System.Drawing.StringAlignment.Near;

                        // 垂直居中对齐 
                        f.LineAlignment = System.Drawing.StringAlignment.Center;
                        // 设置为单行文本 
                        f.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
                        // 绘制文本 
                        using (SolidBrush b = new SolidBrush(Color.Red))
                        {
                            if (kvc.KeyV1.Length > 0 && kvc.KeyV1.IndexOf('{') < 0)
                            {
                                e.DrawString(
                                    kvc.KeyV1,
                                    this.Font,
                                    b, item.PosX + 4, item.PosY + 20, f);
                            }
                            
                        }
                        // 绘制文本 
                        using (SolidBrush b = new SolidBrush(Color.Blue))
                        {
                            if (kvc.KeyV2.Length > 0 && kvc.KeyV2.IndexOf('{')<0)
                            {
                                e.DrawString(
                                    kvc.KeyV2,
                                    this.Font,
                                    b, item.PosX + 8, item.PosY + 8, f);
                            }

                        }
                    }
                }
            }

            #endregion
        }

        private void DrawFontThread()
        {
            Thread.Sleep(200);
            DrawFont();
        }
        /// <summary>
        /// 重新设置长宽
        /// </summary>
        public void ShowWindow()
        {
            ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NoActiveWINDOW);
            DrawFont();
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        public void HideWindow()
        {
            this.Hide();
        }
        public void IniData()
        {
            string appPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
            string[] KeyBoardsPosarry = File.ReadAllLines(appPath + "\\KeyBoards\\KeyBoardsPos.txt", Encoding.Unicode);
            keyBoardsPos = new List<KeyBoardsPos>();
            for (int i = 0; i < KeyBoardsPosarry.Length; i++)
            {
                KeyBoardsPos item = new KeyBoardsPos();
                item.KeysCode = KeyBoardsPosarry[i].Split('=')[0];
                item.PosX = Convert.ToInt32(KeyBoardsPosarry[i].Split('=')[1].Split(' ')[0]);
                item.PosY = Convert.ToInt32(KeyBoardsPosarry[i].Split('=')[1].Split(' ')[1]);
                item.Width = Convert.ToInt32(KeyBoardsPosarry[i].Split('=')[1].Split(' ')[2]);
                item.Height = Convert.ToInt32(KeyBoardsPosarry[i].Split('=')[1].Split(' ')[3]);
                keyBoardsPos.Add(item);
            }
        }
        private void KeyBoardsFrm_Load(object sender, EventArgs e)
        {
           
        }
        /// <summary>
        /// 按x y坐标查找对应的按键
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private KeyBoardsPos GetKeyBoardByXY(int x, int y)
        {
            foreach (KeyBoardsPos item in keyBoardsPos)
            {
                if (x >= item.PosX && x <= item.PosX + item.Width && y >= item.PosY && y <= item.PosY + item.Height)
                {
                    return item;
                }
            }
            return null;
        }

        private KeyBoardsPos GetKeyBoardByKeyCode(string keycode)
        {
            foreach (KeyBoardsPos item in keyBoardsPos)
            {
                if (item.KeysCode == keycode)
                {
                    return item;
                }
            }
            return null;
        }

        private KeyValueClass GetKeyValueClass(string keycode)
        {
            foreach (KeyValueClass item in keyValue)
            {
                if (item.KeyCode == keycode)
                {
                    return item;
                }
            }
            return null;
        }
        private string GetKeyValue(string keystr)
        {
            foreach (KeyValueClass item in keyValue)
            {
                if (item.KeyCode==keystr)
                {
                    if (ShiftDown == null)
                        return item.KeyV1;
                    else
                        return item.KeyV2;
                }
            }
            return "";
        }
        private void KeyBoardsFrm_MouseMove(object sender, MouseEventArgs e)
        {

            KeyBoardsPos item = GetKeyBoardByXY(e.X, e.Y);
            Graphics gh = this.CreateGraphics();
            if (CurKey==null || !CurKey.Equals(item))
            {
                #region 还原状态
                if (CurKey != null)
                {
                    this.Refresh();
                    DrawFont();
                    if (ShiftDown != null)
                    {
                        // 绘制边框 
                        using (Pen pe = new Pen(Color.Black, 2))
                        {
                            gh.DrawRectangle(pe, ShiftDown.PosX, ShiftDown.PosY, ShiftDown.Width - 1, ShiftDown.Height - 1);
                        }
                    }
                }
                #endregion

                CurKey = item;
            }
            if (item != null)
            { 
                // 绘制边框 
                using (Pen pe = new Pen(Color.Gray, 1))
                {
                    gh.DrawRectangle(pe, item.PosX, item.PosY, item.Width-1, item.Height-1);
                }
            }


            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }

        private void KeyBoardsFrm_MouseClick(object sender, MouseEventArgs e)
        {
            KeyBoardsPos item = GetKeyBoardByXY(e.X, e.Y);
            if (item != null)
            {
                if (item.KeysCode == "Shift")
                {
                    if (ShiftDown == null)
                    {
                        ShiftDown = item;
                        Graphics gh = this.CreateGraphics();
                        // 绘制边框 
                        using (Pen pe = new Pen(Color.Black, 2))
                        {
                            gh.DrawRectangle(pe, ShiftDown.PosX, ShiftDown.PosY, ShiftDown.Width - 1, ShiftDown.Height - 1);
                        }
                    }
                    else
                    {
                        ShiftDown = null;
                        this.Refresh();
                        DrawFont();
                    }
                }

                string vstr = GetKeyValue(item.KeysCode);
                if (vstr.Length > 0)
                {
                    SendKeys.Send(vstr);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }

            base.WndProc(ref m);
        }

        private void KeyBoardsFrm_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }
    }

    /// <summary>
    /// 键盘位置类
    /// </summary>
    public class KeyBoardsPos
    {
        private string _keyscode = "";
        private int _posX = 0;
        private int _posY = 0;
        private int _width = 0;
        private int _height = 0;

        /// <summary>
        /// 键盘按键
        /// </summary>
        public string KeysCode
        {
            get { return this._keyscode; }
            set { this._keyscode = value; }
        }

        /// <summary>
        /// 按键X坐标
        /// </summary>
        public int PosX
        {
            get { return this._posX; }
            set { this._posX = value; }
        }
        /// <summary>
        /// 按键Y坐标
        /// </summary>
        public int PosY
        {
            get { return this._posY; }
            set { this._posY = value; }
        }

        /// <summary>
        /// 按键宽度
        /// </summary>
        public int Width
        {
            get { return this._width; }
            set { this._width = value; }
        }

        /// <summary>
        /// 按键高度
        /// </summary>
        public int Height
        {
            get { return this._height; }
            set { this._height = value; }
        }
    }

    /// <summary>
    /// 软件键盘对应的值
    /// </summary>
    public class KeyValueClass
    {
        private string _keycode = "";
        private string _keyv1 = "";
        private string _keyv2 = "";

        public string KeyCode
        {
            get { return this._keycode; }
            set { this._keycode = value; }
        }
        public string KeyV1
        {
            get { return this._keyv1; }
            set { this._keyv1 = value; }
        }
        public string KeyV2
        {
            get { return this._keyv2; }
            set { this._keyv2 = value; }
        }
    }
}
