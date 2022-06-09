using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;


namespace MiouIME
{

    public partial class InputFrm : Form
    {
        private Point offset;
        public string input;
        public string lastinput = "1";
        public string inputstr = "";
        private bool tsbool = false;//用来标识是否还原状态的
        public static int shangpLen = 4;//上屏编码最大长度
        public static int wcmshangpLen = 4;//无重码时最小上屏码长
        public static string wcmspList = "";
        public string shangpingString = "";//当前需上屏的汉字
        public string oldshangpingString = "";//上次可上屏的汉字
        public string lastshangpingString = "";//最后一次并击上屏的汉字
        public string lastInputOut = "";//最后一次上屏的汉字串
        public int pageNum = 1;
        public int pageSize = 6;
        public string[] valuearry;//最近汉字数组
        public static bool OpenCodeView = true;//开启编码提示
        public static bool 错码上屏 = false;
        public bool isGoto = false;
        public double OpacityValue = 50;//输入框透明度
        public int ViewType = 0;//0为横排,1为竖排
        public bool BjInput = false;//是否是并进输出
        public bool backcall = true;//是否回调
        public static Image bgimag;
        public static bool spacexc = false;//是否进行了空格选重
        private Color BoardColor = Color.FromArgb(200, 200, 200);

        const int SW_SHOWNOACTIVATE = 4;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hWnd, short cmdShow);
        private const int HWND_TOPMOST = -1;
        private const int SWP_NoActiveWINDOW = 0x10;
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);


        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)] // 3
            public string lpData;
        }


        [DllImport("user32.dll")]
        private static extern UInt32 SendInput(UInt32 nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static Label[] inputv = null;
        public static Label[] inputc = null;

        public InputFrm()
        {
            //this.TransparencyKey = Color.White;
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.TopLevel = true;
            设置状态栏皮肤();
            UpdateSet();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            this.UpdateStyles();

        }

        public void IniLable()
        {
            this.panelinput.Controls.Clear();
            int left = 1;
            int topc = 4;
            for (int i = 0; i < pageSize; i++)
            {
                Label lbv = new Label();
                Label lbc = new Label();

                string namev = "lablevalue_" + i;
                string namec = "lablecode_" + i;

                #region
                if (this.panelinput.Controls.Find("lablecode_" + (i - 1), false).Length > 0)
                {
                    Label plab1 = (Label)this.panelinput.Controls.Find("lablecode_" + (i - 1), false)[0];
                    if (ViewType == 0) left = plab1.Left + plab1.Width + 2; else left = 1;
                    if (ViewType == 0) topc = 4; else topc = 4 + 18 * i;
                }
                lbv.BackColor = System.Drawing.Color.Transparent;
                lbv.Name = namev;
                lbv.Text = "";
                lbv.BorderStyle = BorderStyle.None;
                lbv.TabIndex = i;
                lbv.ForeColor = Color.Blue;
                lbv.AutoSize = true;
                lbv.Font = new System.Drawing.Font("宋体", Core.InputHelp.SkinFontSize);
                lbv.TextAlign = ContentAlignment.MiddleRight;
                this.panelinput.Controls.Add(lbv);
                inputv[i] = lbv;
                #endregion

                #region
                Label lab1 = (Label)this.panelinput.Controls.Find("lablevalue_" + i, false)[0];
                lbc.Left = lab1.Left + lab1.Width - 2;
                lbc.Top = lab1.Top + 2;

                lbc.BackColor = System.Drawing.Color.Transparent;
                lbc.Name = namec;
                lbc.Text = "";
                lbc.BorderStyle = BorderStyle.None;
                lbc.TabIndex = i;
                lbc.ForeColor = Color.Fuchsia;
                lbc.AutoSize = true;
                lbc.Font = new System.Drawing.Font("宋体", 11);
                lbc.TextAlign = ContentAlignment.BottomLeft;
                this.panelinput.Controls.Add(lbc);
                inputc[i] = lbc;
                #endregion
            }

        }
        public void IniLableSkin()
        {
            for (int i = 0; i < inputv.Length; i++)
            {
                inputv[i].Font = new System.Drawing.Font("宋体", Core.InputHelp.SkinFontSize);
            }
        }
        /// <summary>
        /// 清除字的位置
        /// </summary>
        private void ClearLeftTopLable()
        {

            for (int i = 0; i < pageSize; i++)
            {
                inputv[i].Text = string.Empty;
                inputc[i].Text = string.Empty;

            }
        }
        private void ReViewLable()
        {
            int left = 1;
            int topc = 4;
            for (int i = 0; i < pageSize; i++)
            {


                Label lbv = inputv[i];
                Label lbc = inputc[i];

                #region
                if ((i - 1) >= 0)
                {
                    Label plab1 = inputc[i - 1];
                    if (ViewType == 0) left = plab1.Left + plab1.Width + 2; else left = 1;
                    if (ViewType == 0) topc = 4; else topc = 4 + 18 * i;
                }

                lbv.Left = left;
                lbv.Top = topc;

                #endregion

                #region

                lbc.Left = lbv.Left + lbv.Width - 2;
                lbc.Top = lbv.Top + 2;
                #endregion
            }
        }
        /// <summary>
        /// 添加控件
        /// </summary>
        private void addLable(string strvalue, string cname, int top, Color color, int count)
        {

            int left = 1;
            int topc = 4;
            Label plab1 = null;
            Label plab2 = null;
            Label lab1 = null;
            if (cname.IndexOf("code") > 0)
            {
                //plab1 = (Label)this.panelinput.Controls.Find("lablecode_" + count, false)[0];
                //lab1 = (Label)this.panelinput.Controls.Find("lablevalue_" + count, false)[0];
                plab1 = inputc[count];
                lab1 = inputv[count];

                left = lab1.Left + lab1.Width - 2;
                topc = lab1.Top + 2;
            }
            if (cname.IndexOf("value") > 0)
            {
                plab2 = inputv[count];
                if ((count - 1) >= 0)
                {
                    lab1 = inputc[count - 1];
                    if (ViewType == 0) left = lab1.Left + lab1.Width + 2; else left = 1;
                    if (ViewType == 0) topc = 4; else topc = 4 + 18 * count;
                }

            }

            string headstr = "";
            string formatstr = "";
            if (strvalue.Length > 0)
            {
                if (cname.IndexOf("value") > 0)
                {
                    #region
                    headstr = strvalue.Substring(0, strvalue.IndexOf('.') + 1);
                    if (strvalue.IndexOf('z') > 0)
                    {
                        if (plab2 != null)
                        {
                            plab2.Tag = "z" + strvalue.Split('z')[0].Substring(2) + "_" + strvalue.Split('z')[1];
                            formatstr = strvalue.Split('z')[2];
                            strvalue = CutString(strvalue.Split('z')[2], 30);//叛断字符串长度,太长用...代替
                            if (strvalue.EndsWith("...")) strvalue = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                            plab2.Text = (headstr == "10." ? headstr.Replace("10.", "0.") : headstr) + strvalue;
                            plab2.Top = topc;
                            plab2.Left = left;
                            //plab2.ForeColor = Color.Blue;
                        }
                    }
                    else
                    {
                        if (plab2 != null)
                        {
                            plab2.Tag = "p" + strvalue.Split('p')[0].Substring(2) + "_" + strvalue.Split('p')[1];
                            formatstr = strvalue.Split('p')[2];
                            strvalue = CutString(strvalue.Split('p')[2], 30);//叛断字符串长度,太长用...代替
                            if (strvalue.EndsWith("...")) strvalue = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                            plab2.Text = (headstr == "10." ? headstr.Replace("10.", "0.") : headstr) + strvalue;
                            plab2.Top = topc;
                            plab2.Left = left;
                            //plab2.ForeColor = Color.Black;
                        }
                    }
                    #endregion
                }
            }


            if (cname.IndexOf("code") > 0)
            {
                plab1.Text = strvalue;
                plab1.Top = topc;
                plab1.Left = left;

            }

        }

        /// <summary>
        /// 手工调频
        /// </summary>
        public void MoveValue(int pos)
        {
            if (pos == 1) return;
            if (Core.InputHelp.输入模式 == 1) return;

            if (pos > shangpingString.Split('|').Length)
                return;
            pos--;
            for (int i = 0; i <= pos; i++)
            {
                MoveValue(pos, i);
            }
            ReViewLable();
            int count = shangpingString.Split('|').Length;
            shangpingString = "";
            for (int i = 0; i < count; i++)
            {
                shangpingString += ((Label)this.panelinput.Controls.Find("lablevalue_" + i, false)[0]).Text.Split('.')[1] + "|";
            }
            shangpingString = shangpingString.TrimEnd('|');
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Core.InputHelp.SaveMove));
            th.Start((object)inputstr);
        }

        public void MoveValue(int posy, int posm)
        {
            Label lbvy = (Label)this.panelinput.Controls.Find("lablevalue_" + (posy), false)[0];
            Label lbcy = (Label)this.panelinput.Controls.Find("lablecode_" + (posy), false)[0];
            Label lbvm = (Label)this.panelinput.Controls.Find("lablevalue_" + (posm), false)[0];
            Label lbcm = (Label)this.panelinput.Controls.Find("lablecode_" + (posm), false)[0];

            string tempy, tempm;
            if (lbvy.Tag.ToString().IndexOf('z') >= 0)
            {
                //主码

                tempy = lbvy.Tag.ToString().Substring(1);
                tempm = lbvm.Tag.ToString().Substring(1);
                int posint = int.Parse(tempm.Split('_')[0]);
                int yposint = int.Parse(tempy.Split('_')[0]);

                #region
                string mvs = Core.InputHelp.GetInputMB(inputstr)[yposint];
                Core.InputHelp.GetInputMB(inputstr)[yposint] = Core.InputHelp.GetInputMB(inputstr)[posint];
                Core.InputHelp.GetInputMB(inputstr)[posint] = mvs;

                #endregion

            }
            else if (lbvy.Tag.ToString().IndexOf('p') >= 0)
            {
                //拼音
                tempy = lbvy.Tag.ToString().Substring(1);
                tempm = lbvm.Tag.ToString().Substring(1);
                int posint = int.Parse(tempm.Split('_')[0]);
                int yposint = int.Parse(tempy.Split('_')[0]);

                #region
                string mvs = Core.InputHelp.pydtary[yposint];
                Core.InputHelp.pydtary[yposint] = Core.InputHelp.pydtary[posint];
                Core.InputHelp.pydtary[posint] = mvs;


                #endregion

            }


            string temstr = lbvm.Text;
            lbvm.Text = (posm + 1) + "." + lbvy.Text.Split('.')[1];
            lbvy.Text = (posy + 1) + "." + temstr.Split('.')[1];
            temstr = lbcm.Text;
            lbcm.Text = lbcy.Text;
            lbcy.Text = temstr;


        }


        /// <summary>
        /// 设置信息更新
        /// </summary>
        public void UpdateSet()
        {
            shangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长"));
            wcmshangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "无重码上屏码长"));
            wcmspList = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "wcmspList");
            pageSize = Convert.ToInt32(Core.InputHelp.iniobj.IniReadValue("功能设置", "候选项数"));
            OpenCodeView = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenCodeView") == "0" ? false : true;
            错码上屏 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "错码上屏") == "0" ? false : true;
            ViewType = Core.InputHelp.iniobj.IniReadValue("功能设置", "输入框显示方式") == "0" ? 0 : 1;
            inputv = new Label[pageSize];
            inputc = new Label[pageSize];
            if (ViewType != 0)
            {
                this.Height = pageSize * Core.InputHelp.SkinFontH + 37;
                //this.Width = 156;
            }
            IniLable();
        }

        public void 设置状态栏皮肤()
        {
            string skinexname = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinExName");
            if (!string.IsNullOrEmpty(skinexname)) Core.InputHelp.SkinExName = skinexname;
            string skinFontName = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinFontName");
            if (!string.IsNullOrEmpty(skinFontName)) Core.InputHelp.SkinFontName = skinFontName;
            string Skinbordpen = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "Skinbordpen");
            if (!string.IsNullOrEmpty(Skinbordpen)) Core.InputHelp.Skinbordpen = Color.FromName(Skinbordpen);
            if (Core.InputHelp.Skinbordpen.ToArgb() == 0)
            {
                try { Core.InputHelp.Skinbordpen = Color.FromArgb(int.Parse(Skinbordpen)); }
                catch { }
            }
            string Skinbstring = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "Skinbstring");
            if (!string.IsNullOrEmpty(Skinbstring)) Core.InputHelp.Skinbstring = Color.FromName(Skinbstring);
            if (Core.InputHelp.Skinbstring.ToArgb() == 0)
            {
                try { Core.InputHelp.Skinbstring = Color.FromArgb(int.Parse(Skinbstring)); }
                catch { }
            }
            string Skinbcstring = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "Skinbcstring");
            if (!string.IsNullOrEmpty(Skinbcstring)) Core.InputHelp.Skinbcstring = Color.FromName(Skinbcstring);

            if (Core.InputHelp.Skinbcstring.ToArgb() == 0)
            {
                try { Core.InputHelp.Skinbcstring = Color.FromArgb(int.Parse(Skinbcstring)); }
                catch { }
            }

            string Skinfbcstring = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "Skinfbcstring");
            if (!string.IsNullOrEmpty(Skinfbcstring)) Core.InputHelp.Skinfbcstring = Color.FromName(Skinfbcstring);

            if (Core.InputHelp.Skinfbcstring.ToArgb() == 0)
            {
                try { Core.InputHelp.Skinfbcstring = Color.FromArgb(int.Parse(Skinfbcstring)); }
                catch { }
            }

            string SkinFontSize = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinFontSize");
            if (!string.IsNullOrEmpty(SkinFontSize)) Core.InputHelp.SkinFontSize = int.Parse(SkinFontSize);

            string SkinFontH = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinFontH");
            if (!string.IsNullOrEmpty(SkinFontH)) Core.InputHelp.SkinFontH = int.Parse(SkinFontH);

            string SkinFontJG = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinFontJG");
            if (!string.IsNullOrEmpty(SkinFontJG)) Core.InputHelp.SkinFontJG = int.Parse(SkinFontJG);

            string SkinWidth = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinWidth");
            if (!string.IsNullOrEmpty(SkinWidth)) Core.InputHelp.SkinWidth = int.Parse(SkinWidth);

            string SkinHeith = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinHeith");
            if (!string.IsNullOrEmpty(SkinHeith)) Core.InputHelp.SkinHeith = int.Parse(SkinHeith);

            string SkinStateWidth = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateWidth");
            if (!string.IsNullOrEmpty(SkinStateWidth)) Core.InputHelp.SkinStateWidth = int.Parse(SkinStateWidth);

            string SkinStateHeith = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateHeith");
            if (!string.IsNullOrEmpty(SkinStateHeith)) Core.InputHelp.SkinStateHeith = int.Parse(SkinStateHeith);
            string SkinStateStringWidth = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateStringWidth");
            if (!string.IsNullOrEmpty(SkinStateStringWidth)) Core.InputHelp.SkinStateStringWidth = int.Parse(SkinStateStringWidth);

            string SkinStateFontColor = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateFontColor");
            if (!string.IsNullOrEmpty(SkinStateFontColor)) Core.InputHelp.SkinStateFontColor = Color.FromName(SkinStateFontColor);
            if (Core.InputHelp.SkinStateFontColor.ToArgb() == 0)
            {
                try { Core.InputHelp.SkinStateFontColor = Color.FromArgb(int.Parse(SkinStateFontColor)); }
                catch { }
            }
            string SkinStateFontName = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateFontName");
            if (!string.IsNullOrEmpty(SkinStateFontName)) Core.InputHelp.SkinStateFontName = SkinStateFontName;
            string SkinStateStringView = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinStateStringView");
            if (!string.IsNullOrEmpty(SkinStateStringView)) Core.InputHelp.SkinStateStringView = SkinStateStringView == "1" ? true : false;
            shangpLen = int.Parse(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长"));
            ViewType = Core.InputHelp.iniobj.IniReadValue("功能设置", "输入框显示方式") == "0" ? 0 : 1;
            bgimag = Image.FromFile(Core.InputHelp.编码输出框背景图片());
            if (ViewType != 0)
                this.Height = pageSize * Core.InputHelp.SkinFontH + 37;
            else
                this.Height = Core.InputHelp.编码输出框高();

            this.Width = Core.InputHelp.编码输出框宽();


        }

        public void 保存状态栏皮肤()
        {
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinExName", Core.InputHelp.SkinExName);
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinFontName", Core.InputHelp.SkinFontName);
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "Skinbordpen", Core.InputHelp.Skinbordpen.ToArgb().ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "Skinbstring", Core.InputHelp.Skinbstring.ToArgb().ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "Skinbcstring", Core.InputHelp.Skinbcstring.ToArgb().ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "Skinfbcstring", Core.InputHelp.Skinfbcstring.ToArgb().ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinFontSize", Core.InputHelp.SkinFontSize.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinFontH", Core.InputHelp.SkinFontH.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinFontJG", Core.InputHelp.SkinFontJG.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinWidth", Core.InputHelp.SkinWidth.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinHeith", Core.InputHelp.SkinHeith.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateWidth", Core.InputHelp.SkinStateWidth.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateHeith", Core.InputHelp.SkinStateHeith.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateStringWidth", Core.InputHelp.SkinStateStringWidth.ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateFontColor", Core.InputHelp.SkinStateFontColor.ToArgb().ToString());
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateFontName", Core.InputHelp.SkinStateFontName);
            Core.InputHelp.pfiniobj.IniWriteValue("皮肤信息", "SkinStateStringView", Core.InputHelp.SkinStateStringView ? "1" : "0");

        }

        public void UpdateSkin()
        {

            if (ViewType != 0)
                this.Height = pageSize * Core.InputHelp.SkinFontH + 37;
            else
                this.Height = Core.InputHelp.编码输出框高();

            this.Width = Core.InputHelp.编码输出框宽();

            IniLableSkin();
        }

        public string Input
        {
            get { return this.input; }
            set
            {
                this.input = value;
            }
        }
        private void SetOpacity()
        {
            this.Opacity = 100 - OpacityValue;
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


        /// <summary>
        /// 显示编码对应的汉字
        /// </summary>
        /// <param name="flag">是否检查输入编码的长度</param>
        /// <param name="nobj">是否是并击</param>
        public bool ShowInput(bool flag, bool isbj, bool isbf, Keys _key, string preinput = null)
        {
            bool spdic = false;
            if (this.inputstr == " ")
            {
                this.inputstr = string.Empty;
            }
            curDream = false;
            //lastDate = DateTime.Now;
            if (tsbool)
            {
                this.inputstr = "";
                tsbool = false;
            }

            isGoto = false;
            backcall = true;
            pageNum = 1;
            if (this.inputstr.Length >= shangpLen && !MiouIME.OpenSR)
            {
                if (shangpingString.Length > 0)
                {
                    SendText(shangpingString.Split('|')[0]);
                    spdic = true;
                    MiouIME.justxm = false;
                }
                else
                {
                    this.HideWindow();
                }
                this.inputstr = "";
                if (input.Length == 1 && !Core.InputHelp.IsLowerLetter(input))
                {

                    if (MiouIME.justxm == false)
                        SendText(Core.InputHelp.CheckKeysString(_key));
                    MiouIME.justxm = false;
                    EnterDown(false);
                    return false;
                }
            }
            else if (this.inputstr.Length > shangpLen && MiouIME.OpenSR)
            {
                if (shangpingString.Length > 0)
                {
                    SendText(shangpingString.Split('|')[0]);
                    MiouIME.justxm = false;
                    this.inputstr = this.inputstr.Substring(shangpLen);
                    spdic = true;
                }
                else
                {
                    this.HideWindow();
                }
            }
            if (this.inputstr.Length == 0 && input.Length == 1 && !Core.InputHelp.IsLowerLetter(input))
            {
                if (MiouIME.justxm == false)
                {
                    SendText(Core.InputHelp.CheckKeysString(_key));
                    if (this.Visible) EnterDown(false);
                    spdic = true;
                }
                MiouIME.justxm = false;
                return spdic;
            }
            shangpingString = "";
            this.lbinputstr.Text = "";
            bool notcodesp = false;
            ClearLeftTopLable();
            //判断编码位数
            if (flag)
            {
                if (this.input.Length > 0)
                {
                    this.input = this.input.ToLower();
                    if (this.input.Length > 1 && ",./;'".IndexOf(this.input.Substring(0, 1)) >= 0)
                    {

                        if (this.input.Substring(0, 1) == ",")
                            SendText(Core.InputHelp.CheckKeysString(Keys.Oemcomma));// SendKeys.Send(Core.InputHelp.CheckKeysString(Keys.Oemcomma));
                        else if (this.input.Substring(0, 1) == ".")
                            SendText(Core.InputHelp.CheckKeysString(Keys.OemPeriod));//  SendKeys.Send(Core.InputHelp.CheckKeysString(Keys.OemPeriod));
                        else if (this.input.Substring(0, 1) == "/")
                            SendText(Core.InputHelp.CheckKeysString(Keys.OemQuestion));// SendKeys.Send(Core.InputHelp.CheckKeysString(Keys.OemQuestion));
                        else if (this.input.Substring(0, 1) == ";")
                            SendText(Core.InputHelp.CheckKeysString(Keys.Oem1));// SendKeys.Send(Core.InputHelp.CheckKeysString(Keys.Oem1));
                        input = this.input.Substring(1);
                        notcodesp = false;
                        this.inputstr += this.input;

                    }
                    else
                    {
                        if (this.input.IndexOf("~") >= 0 || (Core.InputHelp.CheckCode(this.input.Substring(this.input.Length - 1, 1))))
                        {
                            notcodesp = false;
                            this.inputstr += this.input;
                        }
                        else
                        {
                            this.inputstr += this.input.Substring(0, this.input.Length - 1);
                            notcodesp = true;
                        }
                    }
                }
            }
            if (this.inputstr.Length == 0)
            {
                return spdic;
            }
        gototemp1:
            if (this.inputstr.Length < shangpLen)
            {

                #region 输入编码小于最大编码长度
                this.lbinputstr.Text = this.inputstr;
                valuearry = Core.InputHelp.GetInputValue(this.inputstr);

                if (valuearry != null)
                {
                    string shangpStr = "";
                    for (int i = 0; i < pageSize && i < valuearry.Length; i++)
                    {
                        string endstr = valuearry[i].Split('|')[1];
                        if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('z')[2].Split('|')[0]) + "|";
                        else
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('p')[2].Split('|')[0]) + "|";

                        string formatstr = Core.InputHelp.ReplaceSystem(valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0]);
                        string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                        if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                        string wencode = valuearry[i].Split('|')[1];
                        shangpStr += (i + 1) + "." + wenzi + " " + wencode + " ";

                        addLable((i + 1) + "." + Core.InputHelp.ReplaceSystem(valuearry[i].Split('|')[0]), "lablevalue_" + i, 30, Color.Blue, i);
                        addLable(wencode, "lablecode_" + i, 30, Color.Red, i);
                        oldshangpingString = shangpingString;
                    }
                    shangpingString = shangpingString.TrimEnd('|');

                    if (notcodesp)
                    {
                        ShangPing(1);
                        SendText(Core.InputHelp.CheckKeysString(_key));
                        spdic = true;
                        return spdic;
                    }
                    else if (valuearry.Length == 1 && (inputstr.Length == wcmshangpLen || wcmspList.IndexOf(inputstr.Length.ToString()) >= 0 || Core.InputHelp.CheckYJSP(inputstr.Substring(0, 1)) || isStopCode(inputstr) || Core.InputHelp.AutoShangPing()))
                    {
                        ShangPing(1);
                        spdic = true;
                        return spdic;
                    }
                    else if (valuearry.Length > 0 && isStopCode(inputstr))
                    {
                        ShangPing(1);
                        spdic = true;
                        return spdic;
                    }
                    else
                    {
                        SetInputByWH(shangpStr);
                        ShowWindow();
                    }
                }
                else if ((错码上屏 || inputstr.Length == wcmshangpLen || wcmspList.IndexOf(inputstr.Length.ToString()) >= 0))
                {
                    if (this.oldshangpingString.Length > 0)
                    {
                        SendText(this.oldshangpingString.Split('|')[0]);// SendKeys.Send(this.oldshangpingString.Split('|')[0]);
                        this.oldshangpingString = "";
                    }
                    if (this.input.Length > 0)
                    {
                        this.inputstr = this.input.Substring(this.input.Length - this.input.Length, this.input.Length);
                        if (!isGoto)
                        {
                            isGoto = true;
                            goto gototemp1;
                        }
                    }
                    else
                    {
                        EnterDown(false);

                        return spdic;
                    }
                }
                else if (",./;'".IndexOf(input) >= 0 && ",./;'".IndexOf(inputstr) >= 0 && inputstr.Length == 1)
                {
                    if (MiouIME.justxm == false)
                        SendText(Core.InputHelp.CheckKeysString(_key));// SendKeys.Send(Core.InputHelp.CheckKeysString(_key));
                    MiouIME.justxm = false;
                    EnterDown(false);
                }
                else
                {
                    if (inputstr.Length >= wcmshangpLen)
                    {
                        EnterDown(false);
                    }
                    else
                    {
                        this.shangpingString = "";
                        ShowWindow();
                    }

                    return spdic;
                }


                #endregion

            }
            else if (this.inputstr.Length == shangpLen)
            {
                #region 输入编码等于最大编码长度
                bool isSP = true;//是否上屏
                this.lbinputstr.Text = this.inputstr;
                valuearry = Core.InputHelp.GetInputValue(this.inputstr);
                if (valuearry != null)
                {
                    string shangpStr = "";
                    for (int i = 0; i < pageSize && i < valuearry.Length; i++)
                    {
                        string endstr = valuearry[i].Split('|')[1];
                        if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('z')[2].Split('|')[0]) + "|";
                        else
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('p')[2].Split('|')[0]) + "|";

                        string formatstr = Core.InputHelp.ReplaceSystem(valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0]);
                        string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                        if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                        string wencode = valuearry[i].Split('|')[1];
                        shangpStr += (i + 1) + "." + wenzi + " " + wencode + " ";

                        addLable((i + 1) + "." + Core.InputHelp.ReplaceSystem(valuearry[i].Split('|')[0]), "lablevalue_" + i, 30, Color.Blue, i);
                        addLable(wencode, "lablecode_" + i, 30, Color.Red, i);

                    }
                    shangpingString = shangpingString.TrimEnd('|');


                    if (valuearry.Length > 0 && isStopCode(inputstr))
                    {
                        ShangPing(1);
                        spdic = true;
                        return spdic;
                    }
                    else
                    {
                        SetInputByWH(shangpStr);
                        ShowWindow();

                    }
                    if (valuearry.Length > 1) isSP = false;

                }
                else if (错码上屏)
                {
                    if (this.oldshangpingString.Length > 0)
                    {
                        SendText(this.oldshangpingString.Split('|')[0]);// SendKeys.Send(this.oldshangpingString.Split('|')[0]);
                        this.oldshangpingString = "";
                    }
                    if (this.input.Length > 0)
                    {
                        this.inputstr = this.input.Substring(this.input.Length - 1, 1);
                        if (!isGoto)
                        {
                            isGoto = true;
                            goto gototemp1;
                        }
                    }
                }
                else
                {
                    this.shangpingString = "";
                    ShowWindow();
                }
                if (isSP)
                {
                    this.ShangPing(1);
                    spdic = true;
                    return spdic;
                }
                #endregion

            }
            else if (this.inputstr.Length > shangpLen)
            {
                if (this.inputstr.Length > 8)
                {
                    EnterDown(false);

                    return spdic;
                }
                #region 输入编码大于最大编码长度
                bool isSP = true;//是否上屏
                this.lbinputstr.Text = this.inputstr;
                valuearry = Core.InputHelp.GetInputValue(this.inputstr);

                if (valuearry != null)
                {
                    string shangpStr = "";
                    for (int i = 0; i < pageSize && i < valuearry.Length; i++)
                    {
                        string endstr = valuearry[i].Split('|')[1];
                        if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('z')[2].Split('|')[0]) + "|";
                        else
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('p')[2].Split('|')[0]) + "|";

                        string formatstr = Core.InputHelp.ReplaceSystem(valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0]);
                        string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                        if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                        string wencode = valuearry[i].Split('|')[1];
                        shangpStr += (i + 1) + "." + wenzi + " " + wencode + " ";

                        addLable((i + 1) + "." + Core.InputHelp.ReplaceSystem(valuearry[i].Split('|')[0]), "lablevalue_" + i, 30, Color.Blue, i);
                        addLable(wencode, "lablecode_" + i, 30, Color.Red, i);

                    }
                    shangpingString = shangpingString.TrimEnd('|');


                    if (valuearry.Length > 0 && isStopCode(inputstr))
                    {
                        ShangPing(1);
                        spdic = true;
                        return spdic;
                    }
                    else
                    {
                        SetInputByWH(shangpStr);
                        ShowWindow();
                    }
                    if (valuearry.Length > 1) isSP = false;
                }

                else
                {
                    this.shangpingString = "";
                    ShowWindow();
                }
                if (isSP)
                {
                    this.ShangPing(1);
                    spdic = true;
                    return spdic;
                }
                #endregion


            }
            return spdic;
        }
        public bool curDream = false;
        /// <summary>
        /// 上屏
        /// </summary>
        public void ShangPing(int page)
        {
            this.lastinput = string.Empty;

            if (this.shangpingString.Length > 0)
            {
                if (page > 0 && page - 1 < this.shangpingString.Split('|').Length)
                {
                    if (curDream)
                        lastInputOut = this.shangpingString.Split('|')[page - 1].Substring(lastInputOut.Length);
                    else
                        lastInputOut = this.shangpingString.Split('|')[page - 1];

                    SendText(@lastInputOut);
                }
                else
                {
                    if (curDream)
                        lastInputOut = this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1].Substring(lastInputOut.Length);
                    else
                        lastInputOut = this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1];

                    SendText(lastInputOut);

                }

            }
        inputGo:
            pageNum = 1;

            BjInput = false;
            this.inputstr = "";
            this.input = "";
            valuearry = null;
            this.shangpingString = "";
            this.oldshangpingString = "";
            this.lastshangpingString = "";
            ClearLeftTopLable();
            this.lbinputstr.Text = "";
            curDream = false;
            if (MiouIME.CZDream && !MiouIME.IsPressShift)
            {
                this.inputstr = " ";
                this.lbinputstr.Text = "联想:";
                valuearry = Core.InputHelp.GetInputValue(this.lastInputOut, true);

                if (valuearry != null)
                {
                    string shangpStr = "";
                    for (int i = 0; i < pageSize && i < valuearry.Length; i++)
                    {
                        string endstr = valuearry[i].Split('|')[1];
                        if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('z')[2].Split('|')[0]) + "|";
                        else
                            shangpingString += Core.InputHelp.ReplaceSystem(valuearry[i].Split('p')[2].Split('|')[0]) + "|";

                        string formatstr = Core.InputHelp.ReplaceSystem(valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0]);
                        string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                        if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                        string wencode = valuearry[i].Split('|')[1];
                        shangpStr += (i + 1) + "." + wenzi + " " + wencode + " ";

                        addLable((i + 1) + "." + Core.InputHelp.ReplaceSystem(valuearry[i].Split('|')[0]), "lablevalue_" + i, 30, Color.Blue, i);
                        addLable(wencode, "lablecode_" + i, 30, Color.Red, i);
                        oldshangpingString = shangpingString;
                    }
                    shangpingString = shangpingString.TrimEnd('|');
                    curDream = true;
                }
                else
                {
                    curDream = false;
                    this.inputstr = string.Empty;
                    this.HideWindow();
                }
            }
            else
                this.HideWindow();
            MiouIME.justxm = false;

        }




        /// <summary>
        /// 并击上屏
        /// </summary>
        public void BJShangPing(int page)
        {
            this.lastinput = string.Empty;
            backcall = false;

            if (this.shangpingString.Length > 0)
            {

                if (page > 0 && page - 1 < this.shangpingString.Split('|').Length)
                {

                    if (this.shangpingString.Split('|')[page - 1].Trim().Length == 1)
                        SendText(this.shangpingString.Split('|')[page - 1]);
                    else
                        SendText(this.shangpingString.Split('|')[page - 1]);
                    lastshangpingString = this.shangpingString.Split('|')[page - 1];
                    lastInputOut = lastshangpingString;
                }
                else
                {
                    if (this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1].Trim().Length == 1)
                        SendText(this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1]);
                    else
                        SendText(this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1]);
                    lastshangpingString = this.shangpingString.Split('|')[this.shangpingString.Split('|').Length - 1];
                    lastInputOut = lastshangpingString;


                }

            }
            else
            {
                BjInput = false;
            }
            this.input = "";
            pageNum = 1;
            if (BjInput)
            {
                BjInput = false;
                return;
            }
            this.inputstr = "";
            valuearry = null;
            this.lbinputstr.Text = "";
            this.shangpingString = "";
            this.oldshangpingString = "";
            this.lastshangpingString = "";
            ClearLeftTopLable();

            this.HideWindow();

            BjInput = false;
            MiouIME.justxm = false;

        }
        /// <summary>
        /// 清除上屏信息
        /// </summary>
        public void EnterDown(bool isSP)
        {
            backcall = true;
            if (isSP && inputstr.Trim().Length > 0)
                SendText(inputstr);
            this.lastinput = string.Empty;

            BjInput = false;
            pageNum = 1;
            valuearry = null;
            this.inputstr = "";
            this.input = "";
            this.shangpingString = "";
            this.oldshangpingString = "";
            ClearLeftTopLable();
            this.lbinputstr.Text = "";

            this.HideWindow();
            MiouIME.justxm = false;
        }

        public void UpPage()
        {
            #region
            if (valuearry != null)
            {
                if (this.inputstr != " ")
                    this.lbinputstr.Text = this.inputstr;
                string shangpStr = "";
                shangpingString = "";
                ClearLeftTopLable();
                int count = 0;
                if (pageNum > 1) pageNum--;
                int pagetol = pageSize * pageNum - pageSize;
                if (pagetol < 0) pagetol = 0;

                for (int i = pagetol; i < pageSize * pageNum && i < valuearry.Length; i++)
                {
                    string endstr = valuearry[i].Split('|')[1];
                    if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                        shangpingString += valuearry[i].Split('z')[2].Split('|')[0] + "|";
                    else
                        shangpingString += valuearry[i].Split('p')[2].Split('|')[0] + "|";

                    string formatstr = valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0];
                    string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                    if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                    string wencode = valuearry[i].Split('|')[1];
                    shangpStr += (count + 1) + "." + wenzi + " " + wencode + " ";

                    addLable((count + 1) + "." + valuearry[i].Split('|')[0], "lablevalue_" + count, 30, Color.Blue, count);
                    addLable(wencode, "lablecode_" + count, 30, Color.Red, count);
                    count++;
                }
                shangpingString = shangpingString.TrimEnd('|');

                SetInputByWH(shangpStr);
                ShowWindow();
            }
            #endregion
        }
        /// <summary>
        /// 捡测是否停止键
        /// </summary>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public bool isStopCode(string endstr)
        {

            for (int i = 0; i < Core.InputHelp.停止键码.Length; i++)
            {
                if (endstr.EndsWith(Core.InputHelp.停止键码.Substring(i, 1))) return true;
            }
            return false;
        }
        public void NextPage()
        {
            #region
            if (valuearry != null)
            {
                if (pageNum * pageSize < valuearry.Length)
                {
                    if (this.inputstr != " ")
                        this.lbinputstr.Text = this.inputstr;
                    string shangpStr = "";
                    shangpingString = "";
                    ClearLeftTopLable();
                    int count = 0;
                    for (int i = pageSize * pageNum; i <= pageSize * pageNum + pageSize - 1 && i < valuearry.Length; i++)
                    {
                        string endstr = valuearry[i].Split('|')[1];
                        if (valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0)
                            shangpingString += valuearry[i].Split('z')[2].Split('|')[0] + "|";
                        else
                            shangpingString += valuearry[i].Split('p')[2].Split('|')[0] + "|";

                        string formatstr = valuearry[i].Substring(0, valuearry[i].Length - endstr.Length).IndexOf('z') > 0 ? valuearry[i].Split('z')[2].Split('|')[0] : valuearry[i].Split('p')[2].Split('|')[0];
                        string wenzi = CutString(formatstr, 30);//叛断字符串长度,太长用...代替
                        if (wenzi.EndsWith("...")) wenzi = formatstr.Substring(0, 3) + "..." + formatstr.Substring(formatstr.Length - 2, 2);
                        string wencode = valuearry[i].Split('|')[1];
                        shangpStr += (count + 1) + "." + wenzi + " " + wencode + " ";

                        addLable((count + 1) + "." + valuearry[i].Split('|')[0], "lablevalue_" + count, 30, Color.Blue, count);
                        addLable(wencode, "lablecode_" + count, 30, Color.Red, count);
                        count++;
                    }

                    shangpingString = shangpingString.TrimEnd('|');
                    pageNum++;
                    SetInputByWH(shangpStr);
                    ShowWindow();

                }
            }
            #endregion
        }
        /// <summary>
        /// 按一定的高度和宽度设置输出框
        /// </summary>
        /// <param name="str"></param>
        public void SetInputByWH(string shangpStr)
        {
            if (ViewType != 0 && MiouIME.OSWin8) return;
            int w1 = 0, w2 = 0, w3 = 0, w4 = 0;
            for (int i = 0; i < this.panelinput.Controls.Count; i++)
            {
                if (this.panelinput.Controls[i].GetType().ToString() == "System.Windows.Forms.Label")
                {
                    Label lable = (Label)this.panelinput.Controls[i];
                    if (lable.Name.IndexOf("lablecode_") >= 0)
                    {
                        w1 = lable.Width;
                        if (ViewType == 0)
                        {
                            w3 += w1;
                        }
                    }
                    else
                    {
                        w2 = lable.Width;
                        if (ViewType == 0)
                        {
                            w4 += w2;
                        }
                    }
                    if (ViewType != 0)
                    {
                        if (w3 + w4 < w1 + w2)
                        {
                            w3 = w1;
                            w4 = w2;
                        }
                    }
                }
            }

            this.Width = w3 + w4;
            if (ViewType == 0) { if (this.Width < 260) this.Width = 260; }
            else if (this.Width < 150) this.Width = 150;
            else this.Width += 4;
            if (ViewType != 0)
            {
                this.Height = pageSize * 18 + 37;
            }
            if ((this.Left + this.Width + 5) > Screen.PrimaryScreen.WorkingArea.Width)
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 5;
            if (this.Left < 4) this.Left = 4;

            if (this.Top > Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10)
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10;
        }
        /// <summary>
        /// 按字符串实际长度截取定长字符窜
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="length">要截取的长度</param>
        /// <returns>string型字符串</returns>
        protected string CutString(string str, int length)
        {
            //return str;
            int i = 0, j = 0;
            foreach (char chr in str)
            {
                if ((int)chr > 127)
                {
                    i += 2;
                }
                else
                {
                    i++;
                }
                if (i > length)
                {
                    str = str.Substring(0, j) + "...";
                    break;
                }
                j++;
            }
            return str;
        }

        /// <summary>   
        /// 计算文本长度，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="Text">需计算长度的字符串</param>
        /// <returns>int</returns>
        public int TextLength(string Text)
        {
            int len = 0;

            for (int i = 0; i < Text.Length; i++)
            {
                byte[] byte_len = System.Text.Encoding.Unicode.GetBytes(Text.Substring(i, 1));
                if (byte_len.Length > 1)
                    len += 2;  //如果长度大于1，是中文，占两个字节，+2
                else
                    len += 1;  //如果长度等于1，是英文，占一个字节，+1
            }

            return len;
        }

        /// <summary>
        /// view input
        /// </summary>
        public void ShowWindow()
        {
            ShowWindow(true);

        }

        /// <summary>
        /// view input
        /// </summary>
        public void ShowWindow(bool f)
        {
            Core.InputForm.Left = this.Left;
            Core.InputForm.Top = this.Top;

            MiouIME.InputForm.ShowInput(f);

            if (!this.Visible && f)
            {
                ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
                SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NoActiveWINDOW);
            }
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        public void HideWindow()
        {
            Core.InputForm.ClearShow();

        }

        private void lbinputstr_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }

        private void lbinputstr_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }

        private void InputFrm_Load(object sender, EventArgs e)
        {

        }

        public void DelLastInput()
        {
            for (int j = 0; j < this.lastInputOut.Length; j++)
            {
                SendText("{BACKSPACE}", true);

            }
        }


        public static void SendText(string message, bool keysend = false)
        {

            if (keysend)
            {
                SendKeys.Send(message);
                return;
            }
            if (message.Length > 1 && MiouIME.OpenSR) MiouIME.SRDelLast = message.Length;
            else MiouIME.SRDelLast = 0;

            if (MiouIME.outtype == 0)
            {
                INPUT[] input_down = new INPUT[message.Length];
                INPUT[] input_up = new INPUT[message.Length];
                //bool upinput = false;
                for (int i = 0; i < message.Length; i++)
                {

                    input_down[i].type = 1;// (int)InputType.INPUT_KEYBOARD;
                    input_down[i].ki.dwFlags = 0x0004;// (int)KEYEVENTF.UNICODE;
                    input_down[i].ki.wScan = (ushort)message[i];
                    input_down[i].ki.wVk = 0;
                    input_up[i].type = input_down[i].type;
                    input_up[i].ki.wScan = input_down[i].ki.wScan;
                    input_up[i].ki.wVk = 0;
                    input_up[i].ki.dwFlags = (int)(KEYEVENTF.KEYUP | KEYEVENTF.UNICODE);

                }

                for (int i = 0; i < input_down.Length; i++)
                {
                    SendInput(1, ref input_down[i], Marshal.SizeOf(input_down[i]));//keydown 
                    SendInput(1, ref input_up[i], Marshal.SizeOf(input_up[i]));//keyup    
                }

            }
            else
            {
                Clipboard.SetText(message);
                //发送ctrl+v 进行粘贴
                keybd_event((byte)Keys.ControlKey, 0, 0, 0);//按下
                keybd_event((byte)Keys.V, 0, 0, 0);
                keybd_event((byte)Keys.ControlKey, 0, 0x2, 0);//松开
                keybd_event((byte)Keys.V, 0, 0x2, 0);
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {

            BufferedGraphicsContext context = BufferedGraphicsManager.Current;

            BufferedGraphics grafx = context.Allocate(e.Graphics, e.ClipRectangle);
            grafx.Graphics.Clear(this.BackColor);
            grafx.Graphics.DrawImage(bgimag, new Rectangle(0, 0, Width, Height));

            Pen bordpen = new Pen(Core.InputHelp.Skinbordpen);
            SolidBrush bstring = new SolidBrush(Core.InputHelp.Skinbstring);
            SolidBrush bcstring = new SolidBrush(Core.InputHelp.Skinbcstring);
            SolidBrush fbcstring = new SolidBrush(Core.InputHelp.Skinfbcstring);

            Rectangle hzrec = new Rectangle(0, 0, Width - 1, Height - 1);

            grafx.Graphics.DrawRectangle(bordpen, hzrec);
            int inputy = Core.InputHelp.SkinFontJG;

            string ins = MiouIME.inputFrm.lbinputstr.Text;// +" " + MiouIME.ForegroundWindow.ToString();
            grafx.Graphics.DrawString(ins, new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize, FontStyle.Bold), bstring, new Point(0 + 3, 0 + 4));
            if (valuearry != null && valuearry.Length > 0)
                grafx.Graphics.DrawString(string.Format("{0}/{1}", pageNum, (valuearry.Length % pageSize == 0 ? valuearry.Length / pageSize : valuearry.Length / pageSize + 1)), new Font("", 10F), bstring, new Point(Width - 40, 0 + 4));

            if (MiouIME.inputFrm.ViewType == 0)
            {
                //横排显示 
                int len = 3;
                for (int i = 0; i < inputv.Length; i++)
                {
                    if (string.IsNullOrEmpty(inputv[i].Text)) break; ;
                    string v = inputv[i].Text.Substring(2);
                    int vw = InputFrm.inputv[i].Width;
                    string pos = i == 9 ? "0." : (i + 1).ToString() + ".";
                    len = 3 + InputFrm.inputv[i].Left;
                    if (i == 0)
                        grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize), fbcstring, new Point(len, inputy));
                    else
                        grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize), bstring, new Point(len, inputy));
                    len = 1 + InputFrm.inputc[i].Left;
                    grafx.Graphics.DrawString(inputc[i].Text, new Font("宋体", Core.InputHelp.SkinFontSize - 1), bcstring, new Point(len, inputy - 1));
                }
            }
            else
            {
                //竖排显示 
                for (int i = 0; i < inputv.Length; i++)
                {
                    if (string.IsNullOrEmpty(inputv[i].Text)) break; ;
                    string v = inputv[i].Text.Substring(2);
                    int vw = InputFrm.inputv[i].Width; //int vw = v.Length * Core.InputHelp.SkinFontW + Core.InputHelp.SkinFontW + 16;
                    string pos = i == 9 ? "0." : (i + 1).ToString() + ".";
                    if (i == 0)
                        grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize), fbcstring, new Point(3, inputy + i * Core.InputHelp.SkinFontH));
                    else
                        grafx.Graphics.DrawString(pos + v, new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize), bstring, new Point(3, inputy + i * Core.InputHelp.SkinFontH));

                    grafx.Graphics.DrawString(inputc[i].Text, new Font("宋体", Core.InputHelp.SkinFontSize - 1), bcstring, new Point(3 + vw, (inputy + i * Core.InputHelp.SkinFontH) - 1));
                }
            }

            grafx.Render(e.Graphics);
            grafx.Graphics.Dispose();
            grafx.Dispose();

        }
    }

    [Flags()]
    public enum KEYEVENTF
    {
        EXTENDEDKEY = 0x0001,
        KEYUP = 0x0002,
        UNICODE = 0x0004,
        SCANCODE = 0x0008,
    }
    public enum InputType
    {
        INPUT_MOUSE = 0,
        INPUT_KEYBOARD = 1,
        INPUT_HARDWARE = 2,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public Int16 wVk;
        public ushort wScan;
        public Int32 dwFlags;
        public Int32 time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public Int32 dx;
        public Int32 dy;
        public Int32 mouseData;
        public Int32 dwFlags;
        public Int32 time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public Int32 type;//0-MOUSEINPUT;1-KEYBDINPUT;2-HARDWAREINPUT   
        [FieldOffset(4)]
        public KEYBDINPUT ki;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public Int32 uMsg;
        public Int16 wParamL;
        public Int16 wParamH;
    }
}
