using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Net;
namespace MiouIME
{
    public partial class MiouIME : Form
    {
        private static DateTime startDate = DateTime.Now;
 
        public static InputFrm inputFrm = new InputFrm();//输入编码显框
        public static IntPtr ForegroundWindow = IntPtr.Zero;//活动窗口句柄
        IntPtr targetThreadID = IntPtr.Zero;//活动窗口线程号
        IntPtr localThreadID = IntPtr.Zero;//输入法线程号
        private Point offset;
        private Point curPoint;//当前光标位置
        private bool curTrac = true;//光标跟随
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        MenuItem[] mnuItms = null;
        public static int hKeyboardHook = 0;   //键盘钩子句柄 
        public static bool IsCallNextHookEx = false;
        public static bool isActiveInput = true;
        public static bool IsPressShift = false;
        public static bool IsPressAlt = false;
        public static bool IsPressCtrl = false;
        public static bool IsPressWin = false;
        public static int Pressnum = 0;
        public static bool OSWin8 = false;
        KeyBoardsFrm KeyBoardfrm = new KeyBoardsFrm();//软键盘
 
    
        Thread updateth = null;
        private bool spFirstCode = false;
        private bool spSencedCode = false;
        private bool pressShiftOther = false;
        public static bool justxm = false;
        private bool SRAndNo = false;//速录与正常录入一起使用
        public static Core.InputForm InputForm = new Core.InputForm();
    
        #region 配置属性
        public static bool EnterCodeSP = false;
        public static bool IsViewSta = true;//隐藏状态栏
        public static bool OpenSR = false;//开启速录
        public static bool OpenOneSR = false;//开启速录单击结尾直接上屏
        public static bool BDOneOut = true;//速录时当这打一个标点时输出标点符号，不参与编码
        public static bool SROneOut = true;//速录时一码直接上屏
        public static bool CZDream = false;//词组联想功能
        public static string GBCode = "unicode";//汉字集
        public static int SRDelLast = 0;
        public static int outtype = 0;//上屏方式 ，0默认，1剪贴板
        #endregion
        #region 输入法状态
        public static bool Is简体 = true;
        public static bool Is全角 = true;
        public static bool Is中文句号 = true;
        public static bool Is中文 = true;
        public static bool IsOpenSoftKeyboard = false;
        public static bool Is自动启动 = false;
        #endregion
        #region 快捷键
        public static bool Press激活关闭输入快捷键1 = false;
        public static bool Press中英文切换快捷键1 = false;
        public static bool Press简体繁体切换1 = false;
        public static bool Press中英文标点切换1 = false;
        public static bool Press全角半角切换1 = false;
        public static bool Press临拼切换键1 = false;
        #endregion
        //鼠标常量 
        public const int WH_KEYBOARD_LL = 13;   //keyboard   hook   constant   
        public const int WM_CHAR = 0x0102;
        private string updateurl = string.Empty;
        private string lastupdate = "0";
        private bool autoupdate = false;
        public static HookProc KeyboardHookProcedure;   //声明键盘钩子事件类型.

        Queue<Core.KeysQueue> KeyQueue = new Queue<Core.KeysQueue>(); //键盘输入的消息队列

        #region API
        //装置钩子的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸下钩子的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("user32 ")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32 ")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool UpdateWindow(IntPtr hWnd);


        const int SW_SHOWNOACTIVATE = 4;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hWnd, short cmdShow);
        private const int HWND_TOPMOST = -1;
        private const int SWP_NoActiveWINDOW = 0x10;
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;
        private string ModuleName = "";

        //Win32 Calls
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        };

        /// <summary>
        /// 可获取窗口进程信息
        /// </summary>
        /// <param name="dwthreadid"></param>
        /// <param name="lpguithreadinfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetGUIThreadInfo")]
        public static extern uint GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);

 

        #endregion


        public MiouIME()
        {
           // this.TransparencyKey = Color.White;
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            //状态栏位于屏幕左下角
            ShowWindow();
            设置状态栏皮肤();
            UpdateSet();
            localThreadID = GetCurrentThreadId();//获取当前线程号
            MiouIME.InputForm.ShowInput(false);

        }
     　
        /// <summary>
        /// 设置信息更新
        /// </summary>
        public void UpdateSet()
        {
            OpenSR = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenSR") == "1" ? true : false;
            OpenOneSR = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenOneSR") == "1" ? true : false;
            curTrac = Core.InputHelp.iniobj.IniReadValue("功能设置", "光标跟随") == "1" ? true : false;
            BDOneOut = Core.InputHelp.iniobj.IniReadValue("功能设置", "标点一码输出") == "1" ? true : false;
            SROneOut = Core.InputHelp.iniobj.IniReadValue("功能设置", "SROneOut") == "1" ? true : false;
            OSWin8 = Core.InputHelp.iniobj.IniReadValue("功能设置", "Metro") == "1" ? true : false;
            CZDream = Core.InputHelp.iniobj.IniReadValue("功能设置", "CZDream") == "1" ? true : false;
            SRAndNo = Core.InputHelp.iniobj.IniReadValue("功能设置", "SRAndNo") == "1" ? true : false;
            outtype = Core.InputHelp.iniobj.IniReadValue("功能设置", "outtype") == "1" ? 1 : 0;
            if (OpenSR)
            {
                Core.InputHelp.bjdb一码上屏 = true;
                Core.InputHelp.只用主码并击 = false;
                Core.InputHelp.自动搜索主词库 = true;
            }
            GBCode = Core.InputHelp.iniobj.IniReadValue("功能设置", "GBCode");
            if(GBCode.Trim().Length==0) GBCode="unicode";

            IsViewSta = Core.InputHelp.iniobj.IniReadValue("功能设置", "IsViewSta") == "0" ? true : false;
            EnterCodeSP = Core.InputHelp.iniobj.IniReadValue("功能设置", "回车运用") == "清空编码" ? false : true;
            Is自动启动 = Core.InputHelp.iniobj.IniReadValue("功能设置", "自动启动") == "0" ? false : true;
            updateurl = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "updateurl");
            lastupdate = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "lastupdate");
            autoupdate = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "autoupdate") == "1" ? true : false;

            Core.InputHelp.RunWhenStart(Is自动启动);

        }
        //析构函数. 
        ~MiouIME()
        {
            Stop();
       
        }

        public void 设置状态栏皮肤()
        {
            Image imag = Image.FromFile(Core.InputHelp.状态栏背景框());
            this.BackgroundImage = imag;
            this.Height = Core.InputHelp.状态栏高();
            this.Width = Core.InputHelp.状态栏宽();
            imag = Image.FromFile(Is中文 == true ? Core.InputHelp.中() : Core.InputHelp.英());
            this.piczyw.BackgroundImage = imag;
            imag = Image.FromFile(Is全角 == true ? Core.InputHelp.全角() : Core.InputHelp.半角());
            this.picqbj.BackgroundImage = imag;
            imag = Image.FromFile(Is中文句号 == true ? Core.InputHelp.句号() : Core.InputHelp.逗号());
            this.picjdh.BackgroundImage = imag;
            imag = Image.FromFile(Is简体 == true ? Core.InputHelp.简体() : Core.InputHelp.繁体());
            this.picjft.BackgroundImage = imag;
            imag = Image.FromFile(Core.InputHelp.软键盘());
            this.picrjp.BackgroundImage = imag;
            if (Core.InputHelp.SkinStateStringView)
            {
                this.lbinputname.Width = Core.InputHelp.SkinStateStringWidth;
                this.lbinputname.Visible = true;
                this.lbinputname.ForeColor = Core.InputHelp.SkinStateFontColor;
                this.lbinputname.Font = new Font(Core.InputHelp.SkinStateFontName, 9);
                this.lbinputname.Text = Core.InputHelp.输入法名();
            }
            else
            {
                this.lbinputname.Visible = false;
            }
         
        }

        public void Start()
        {
            //安装键盘钩子   
            if (hKeyboardHook == 0)
            {
                ModuleName = Process.GetCurrentProcess().MainModule.ModuleName;
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(ModuleName), 0);
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx   ist   failed. ");
                }

            }
        }

        public void ReStart()
        {
            //重新安装键盘钩子   
            int temhk = hKeyboardHook;
            hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(ModuleName), 0);
            if (temhk != 0)
            {
                UnhookWindowsHookEx(temhk);
            }
        }

        public void Stop()
        {

            bool retKeyboard = true;

            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            //如果卸下钩子失败 
            if (!(retKeyboard)) throw new Exception("UnhookWindowsHookEx   failed. ");


        }
        bool lscloseinput = false;

        public static DateTime lastUpKey = DateTime.Now;
    
   
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
           
            Core.KeyboardHookStruct MyKeyboardHookStruct = (Core.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Core.KeyboardHookStruct));
            if (MyKeyboardHookStruct.vkCode == 231)
            {
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            }
            Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;

            if (inputFrm.inputstr.Length > 0)
            {
                if (keyData == Keys.D1)
                {
                    inputFrm.ShangPing(1);
                    return 1;
                }
                else if (keyData == Keys.D2)
                {
                    inputFrm.ShangPing(2);
                    return 1;
                }
                else if (keyData == Keys.D3)
                {
                    inputFrm.ShangPing(3);
                    return 1;
                }
                else if (keyData == Keys.D4)
                {
                    inputFrm.ShangPing(4);
                    return 1;
                }
                else if (keyData == Keys.D5)
                {
                    inputFrm.ShangPing(5);
                    return 1;
                }
                else if (keyData == Keys.D6)
                {
                    inputFrm.ShangPing(6);
                    return 1;
                }
                else if (keyData == Keys.D7)
                {
                    inputFrm.ShangPing(7);
                    return 1;
                }
                else if (keyData == Keys.D8)
                {
                    inputFrm.ShangPing(8);
                    return 1;
                }
                else if (keyData == Keys.D9)
                {
                    inputFrm.ShangPing(9);
                    return 1;
                }
                else if (keyData == Keys.D0)
                {
                    inputFrm.ShangPing(0);
                    return 1;
                }
            }
            string revstr = "";
            if (!IsPressCtrl && !OpenSR && KeyQueue.Count == 0 && inputFrm.inputstr.Length == 0 && !MiouIME.Is全角)
                if (keyData == Keys.Space && !IsPressShift)
                    return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            if ((IsPressCtrl || IsPressAlt || IsPressWin || IsPressShift) && keyData!=Keys.Delete && CheckFunKey(keyData) && inputFrm.inputstr.Length == 0)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

            #region deal
           // LastKeyTime = DateTime.Now;
            #region OnKeyDownEvent
            //引发OnKeyDownEvent 
            if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
            {
               
                if (IsPressShift && keyData != Keys.LShiftKey && keyData != Keys.RShiftKey)
                {
                    spFirstCode = true;
                    pressShiftOther = true;
                    if (inputFrm.inputstr == " ")
                        inputFrm.EnterDown(false);
                    if (!Is中文句号 && !Is全角 && keyData!=Keys.Space)
                    {
                        spFirstCode = false;
                        spSencedCode = false;
                        if (inputFrm.inputstr.Length > 0 && inputFrm.inputstr != " ") inputFrm.ShangPing(1);
                        return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
                    }
                }
  
 
                if (KeyQueue.Count == 0 && (inputFrm.inputstr.Length == 0 || inputFrm.inputstr == " ") && keyData == Keys.Back)
                {
 
                    if (inputFrm.inputstr == " ") inputFrm.EnterDown(false);
                    if (MiouIME.OpenSR && MiouIME.SRDelLast>1)
                    {
                        int abct = MiouIME.SRDelLast;
                        MiouIME.SRDelLast = 0;
                        for (int abc = 0; abc < abct; abc++)
                        { InputFrm.SendText("{BACKSPACE}", true); }
                        return 1;
                    }
                    else
                        return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
                }
                //输入入队
                Core.KeysQueue keyques = new Core.KeysQueue();
                keyques.KeyData = keyData;
                keyques.MyKeyboardHookStruct = MyKeyboardHookStruct;
 

                #region shift/ctrl/alt/win 按下判断
                if (keyData == Keys.RShiftKey || keyData == Keys.LShiftKey)
                {
                    IsPressShift = true;
                    if (isActiveInput && inputFrm.inputstr.Length == 0 && Is中文)
                    {
                        isActiveInput = false;
                        lscloseinput = true;
                    }
                }

                if (IsPressShift && lscloseinput)
                {
                    revstr = Core.InputHelp.CheckKeysString(keyData);
                    if (revstr.Length > 0 && ",.'\\;，。、；‘“”".IndexOf(revstr) >= 0)
                    {
                        InputFrm.SendText(revstr);
                        return 1;
                    }
                }

                if (Console.CapsLock)
                {
                    //english input
                    Is中文 = false;
                    inputFrm.EnterDown(false);
                    this.设置状态栏皮肤();
                }

                if (keyData == Keys.RControlKey || keyData == Keys.LControlKey)
                    IsPressCtrl = true;
                if (keyData == Keys.RMenu || keyData == Keys.LMenu)
                    IsPressAlt = true;
                if (keyData == Keys.RWin || keyData == Keys.LWin)
                    IsPressWin = true;

                if (IsPressAlt && IsPressCtrl && keyData == Keys.Delete)
                {
                    IsPressAlt = false;
                    IsPressWin = false;
                    IsPressCtrl = false;
                    IsPressShift = false;
                    isActiveInput = false;

                    inputFrm.EnterDown(false);
                    TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\logh32.ico");
                }
                if (IsPressWin && keyData == Keys.L)
                {
                    IsPressWin = false;
                    IsCallNextHookEx = true;
                    goto gototem;
                }
                #endregion
                #region 快捷键功能键按下
                if (keyData.ToString() == Core.InputHelp.Get激活关闭输入快捷键().Split('+')[0])
                    Press激活关闭输入快捷键1 = true;
                if (keyData.ToString() == Core.InputHelp.Get中英文切换快捷键().Split('+')[0])
                    Press中英文切换快捷键1 = true;
                if (keyData.ToString() == Core.InputHelp.Get简体繁体切换().Split('+')[0])
                    Press简体繁体切换1 = true;
                if (keyData.ToString() == Core.InputHelp.Get中英文标点切换().Split('+')[0])
                    Press中英文标点切换1 = true;
                if (keyData.ToString() == Core.InputHelp.Get全角半角切换().Split('+')[0])
                    Press全角半角切换1 = true;
                if (keyData.ToString() == Core.InputHelp.Get临拼切换键().Split('+')[0])
                    Press临拼切换键1 = true;
                #endregion

             
                if (keyData == Keys.Escape && !IsPressCtrl)
                {
                    if (KeyBoardfrm.Visible == true)
                    {
                        KeyBoardfrm.HideWindow();
                        IsOpenSoftKeyboard = false;
                        IsCallNextHookEx = false;
                        goto gototem;
                    }
                    if (inputFrm.Visible)
                    {
                        //有输入,功能键阻止
                        inputFrm.EnterDown(false);
                        return 1;
                    }
                    else 
                        IsCallNextHookEx = true;//无输入,功能键发送

                    inputFrm.EnterDown(false);

                    goto gototem;
                }
                else if (keyData == Keys.Escape && IsPressCtrl)
                {
                    IsCallNextHookEx = false;
                    goto gototem;
                }
                #region 激活关闭输入法
                if ((((keyData.ToString() == Core.InputHelp.Get激活关闭输入快捷键().Split('+')[1] || Core.InputHelp.Get激活关闭输入快捷键().Split('+')[1] == "") && Press激活关闭输入快捷键1 == true)) && isActiveInput == true)
                {
                    isActiveInput = false;
                    MiouIME.SRDelLast = 0;
                    //if (IsViewSta)
                    //    this.Hide();
                    inputFrm.EnterDown(false);
                    TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\logh32.ico");
                }
                else
                {
                    if ((((keyData.ToString() == Core.InputHelp.Get激活关闭输入快捷键().Split('+')[1] || Core.InputHelp.Get激活关闭输入快捷键().Split('+')[1] == "") && Press激活关闭输入快捷键1 == true)) && isActiveInput == false)
                    {
                        isActiveInput = true;
                        if (IsViewSta)
                            ShowWindow();
                        TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\log32.ico");

                    }
                }
                #endregion
                if (keyData == Keys.Space && IsPressCtrl)
                {
                    IsCallNextHookEx = false;
                    goto gototem;
                }
                else if (IsPressShift && IsPressCtrl)
                {
                    IsCallNextHookEx = false;
                    goto gototem;
                }
                if (isActiveInput || lscloseinput)
                {
                    
                    #region 中英切换
                    if (Press中英文切换快捷键1 && (keyData.ToString() == Core.InputHelp.Get中英文切换快捷键().Split('+')[1] || Core.InputHelp.Get中英文切换快捷键().Split('+')[1] == ""))
                    {
                        piczyw_Click(null, null);
                        IsCallNextHookEx = false;
                        goto gototem;
                    }

                    #endregion
                    #region 全半角切换
                    if (Press全角半角切换1 && (keyData.ToString() == Core.InputHelp.Get全角半角切换().Split('+')[1] || Core.InputHelp.Get全角半角切换().Split('+')[1] == ""))
                    {
                        picqbj_Click(null, null);
                        IsCallNextHookEx = false;
                        goto gototem;
                    }
                    #endregion
                    #region 简繁体切换
                    if (Press简体繁体切换1 && (keyData.ToString() == Core.InputHelp.Get简体繁体切换().Split('+')[1] || Core.InputHelp.Get简体繁体切换().Split('+')[1] == ""))
                    {
                        picjft_Click(null, null);
                        IsCallNextHookEx = false;
                        goto gototem;
                    }
                    #endregion
                    #region 中英文标点切换
                    if (Press中英文标点切换1 && (keyData.ToString() == Core.InputHelp.Get中英文标点切换().Split('+')[1] || Core.InputHelp.Get中英文标点切换().Split('+')[1] == ""))
                    {
                        picjdh_Click(null, null);
                        IsCallNextHookEx = false;
                        goto gototem;
                    }
                    else if (Press中英文标点切换1 && !(keyData.ToString() == Core.InputHelp.Get中英文标点切换().Split('+')[1] || Core.InputHelp.Get中英文标点切换().Split('+')[1] == ""))
                    {
                        IsCallNextHookEx = true;
                    }
                    #endregion
                    #region 快捷键选择第2位重码
                    if (keyData.ToString() == Core.InputHelp.Get第二重码选择键().Split('+')[0] && inputFrm.shangpingString != "")
                    {
                        spFirstCode = true;
                        justxm = true;
                        if (keyData == Keys.Space) InputFrm.spacexc = true;
                    }
                    #endregion
                    #region 快捷键选择第3位重码
                    if (keyData.ToString() == Core.InputHelp.Get第三重码选择键().Split('+')[0] && inputFrm.shangpingString != "")
                    {
                        spSencedCode = true;
                        justxm = true;
                        if (keyData == Keys.Space) InputFrm.spacexc = true;
                    }
                    #endregion
                    if (IsPressShift && keyData != Keys.LShiftKey && keyData != Keys.RShiftKey)
                    {
                        spFirstCode = true;
                        pressShiftOther = true;
                        if (inputFrm.inputstr == " ")
                            inputFrm.EnterDown(false);
                    }
                    else
                        pressShiftOther = false;

                    #region 临拼切换
                    if (Press临拼切换键1 && (keyData.ToString() == Core.InputHelp.Get临拼切换键().Split('+')[1] || Core.InputHelp.Get临拼切换键().Split('+')[1] == ""))
                    {
                        if (Core.InputHelp.输入模式 >= 2)
                        {
                            Core.InputHelp.IniCode();
                            Core.InputHelp.输入模式 = 0;
                        }
                        else
                        {
                            Core.InputHelp.IniPYCode();
                            Core.InputHelp.输入模式++;
                        }
                        Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "输入模式", Core.InputHelp.输入模式.ToString());
                        this.lbinputname.Text = Core.InputHelp.输入法名();
                        IsCallNextHookEx = false;
                        goto gototem;
                    }
                    #endregion

                    try
                    {

                        #region 手工调频
                        if (IsPressCtrl && (keyData == Keys.D2 || keyData == Keys.D3 || keyData == Keys.D4 || keyData == Keys.D5
                            || keyData == Keys.D6 || keyData == Keys.D7 || keyData == Keys.D8 || keyData == Keys.D9))
                        {
                            if (inputFrm.shangpingString.Length > 0)
                            {
                                inputFrm.MoveValue(int.Parse(keyData.ToString().Substring(keyData.ToString().Length - 1, 1)));
                                IsCallNextHookEx = false;
                                goto gototem;
                            }
                        }
                        #endregion

                        #region 汉字反查
                        if (IsPressCtrl && keyData == Keys.OemQuestion)
                        {
                            if (inputFrm.lastInputOut.Length > 0)
                            {

                                QueryCodeFrm qfrm = new QueryCodeFrm();
                                qfrm.QueryValue = inputFrm.lastInputOut;
                                qfrm.TopMost = true;
                                qfrm.Show();

                            }
                            IsCallNextHookEx = true;
                            goto gototem;

                        }
                        #endregion

                        #region 激活状态
                        if (!Is中文)
                        {
                            IsCallNextHookEx = true;
                            goto gototem;
                        }

                        if (!IsPressAlt && !IsPressWin && !IsPressCtrl)
                        {
                            revstr = Core.InputHelp.CheckKeysString(keyData);
                            if ("1234567890-='\\][,./`;".IndexOf(revstr) >= 0 && revstr.Length == 1 && inputFrm.inputstr.Length == 0)
                                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

                            if (!IsPressShift)
                            {
                                lock (KeyQueue)
                                    KeyQueue.Enqueue(keyques);

                                if (!OpenSR)
                                {
                                    Pressnum = 0;
                                    KeyEventArgs e = new KeyEventArgs(keyData);
                                    UserOnKeyUp(this, e, ref IsCallNextHookEx);
                                }
                            }
                            else
                            {
                                #region 快捷键选择重码

                                if ((spFirstCode || spSencedCode) && pressShiftOther)
                                {
                                    if (inputFrm.inputstr == " ") inputFrm.EnterDown(false);
                                    spFirstCode = false;
                                    pressShiftOther = false;
                                  
                                        inputFrm.ShangPing(1);
                                }

                                #endregion
                            }
                            if (OpenSR)
                                Pressnum++;

                        }
                        else
                        {
                            IsCallNextHookEx = true;
                            goto gototem;
                        }
                        #endregion

                    }
                    catch { }
                }
                else
                {
                    IsCallNextHookEx = true;
                    goto gototem;
                }
            }
            #endregion


            #region OnKeyUpEvent
            //引发OnKeyUpEvent 
            if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
            {

                if ( OpenSR)
                    Pressnum--;
                if (Pressnum < 0) Pressnum = 0;

 
                #region shift/ctrl/alt/win 按下判断 释放
                if (keyData == Keys.RShiftKey || keyData == Keys.LShiftKey)
                {
                    IsPressShift = false;
                    if (inputFrm.inputstr.Length == 0 && Is中文 && lscloseinput)
                    {
                        isActiveInput = true;
                        lscloseinput = false;
                        this.ShowWindow();
                    }
                    else if (inputFrm.inputstr.Length == 0 && !Is中文 && lscloseinput)
                    {
                        isActiveInput = true;
                        lscloseinput = false;
                        this.ShowWindow();
                    }
                }
                if (keyData == Keys.CapsLock && !Console.CapsLock)
                {
                    //english input
                    Is中文 = true;
                    this.设置状态栏皮肤();
                }
                if (keyData == Keys.RControlKey || keyData == Keys.LControlKey)
                    IsPressCtrl = false;
                if (keyData == Keys.RMenu || keyData == Keys.LMenu)
                    IsPressAlt = false;
                if (keyData == Keys.RWin || keyData == Keys.LWin)
                    IsPressWin = false;
                #endregion

                #region 快捷键功能键 释放
                if (keyData.ToString() == Core.InputHelp.Get激活关闭输入快捷键().Split('+')[0])
                    Press激活关闭输入快捷键1 = false;
                if (keyData.ToString() == Core.InputHelp.Get中英文切换快捷键().Split('+')[0])
                    Press中英文切换快捷键1 = false;
                if (keyData.ToString() == Core.InputHelp.Get简体繁体切换().Split('+')[0])
                    Press简体繁体切换1 = false;
                if (keyData.ToString() == Core.InputHelp.Get中英文标点切换().Split('+')[0])
                    Press中英文标点切换1 = false;
                if (keyData.ToString() == Core.InputHelp.Get全角半角切换().Split('+')[0])
                    Press全角半角切换1 = false;
                if (keyData.ToString() == Core.InputHelp.Get临拼切换键().Split('+')[0])
                    Press临拼切换键1 = false;
                #endregion


                if (isActiveInput)
                {
                    try
                    {
                        #region 快捷键选择第2位重码

                        if (spFirstCode && !pressShiftOther)
                        {
                            spFirstCode = false;
                            inputFrm.ShangPing(2);
                            if (Core.InputHelp.CheckKeysString(keyData) != "")
                            {
                                justxm = false;
                            }

                        }

                        #endregion
                        #region 快捷键选择第3位重码

                        if (spSencedCode && !pressShiftOther)
                        {
                            spSencedCode = false;
                            inputFrm.ShangPing(3);
                            if (Core.InputHelp.CheckKeysString(keyData) != "")
                            {
                          
                                    justxm = false;
                            }

                        }

                        #endregion

                        KeyEventArgs e = new KeyEventArgs(keyData);
                        UserOnKeyUp(this, e, ref IsCallNextHookEx);
 
                    }
                    catch { }
                }
                else
                {
                    IsCallNextHookEx = true;
                    goto gototem;
                }

                if ( !OpenSR)
                    IsCallNextHookEx = UseOnKeyPress(this, GetKeyPressEventArgs(MyKeyboardHookStruct), false, keyData,(wParam == WM_KEYUP || wParam == WM_SYSKEYUP));
            }
            #endregion

            if (OpenSR)
            {
                if (isActiveInput && !OpenSR)
                    IsCallNextHookEx = UseOnKeyPress(this, GetKeyPressEventArgs(MyKeyboardHookStruct), false, keyData, (wParam == WM_KEYUP || wParam == WM_SYSKEYUP));
                else if (isActiveInput && OpenSR && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && keyData != Keys.Space)
                    IsCallNextHookEx = UseOnKeyPress(this, GetKeyPressEventArgs(MyKeyboardHookStruct), false, keyData, (wParam == WM_KEYUP || wParam == WM_SYSKEYUP));
                else if (isActiveInput && OpenSR && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && keyData == Keys.Space)
                    IsCallNextHookEx = UseOnKeyPress(this, GetKeyPressEventArgs(MyKeyboardHookStruct), false, keyData, (wParam == WM_KEYUP || wParam == WM_SYSKEYUP));
            }


        gototem:
            if ((IsPressAlt || IsPressWin || IsPressShift) && (CheckFunKey(keyData) || keyData == Keys.Space) && inputFrm.inputstr.Length == 0)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            else if(IsPressShift && IsPressCtrl)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            else if (IsPressShift && IsPressAlt)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            else if (IsPressCtrl && IsPressAlt)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            else if (IsPressShift && IsPressAlt && IsPressCtrl)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

            if (IsCallNextHookEx || IsPressShift)
            {

                if (isActiveInput && (  OpenSR) && keyData == Keys.Space && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    int queuecount = KeyQueue.Count;

                    return 1;
                }
                if (( OpenSR) && keyData == Keys.Space && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    if (isActiveInput && !(IsPressAlt || IsPressCtrl || IsPressShift || IsPressWin))
                    {
                        revstr = Core.InputHelp.CheckKeysString(keyData);

                        //处理空格
                        if (revstr.Length == 0 && inputFrm.backcall)
                        {

                            InputFrm.SendText(" ");
                            int queuecount = KeyQueue.Count;
                            for (int i = 0; i < queuecount; i++)
                            {
                                KeyQueue.Dequeue();
                            }

                            return 1;
                        }
                    }
                }

                #region

                if (revstr == "")
                {
                    inputFrm.backcall = true;
                    int queuecount = KeyQueue.Count;
                    for (int i = 0; i < queuecount; i++)
                    {
                        KeyQueue.Dequeue();
                    }
                    if (!lscloseinput)
                        return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
                }
                else if (inputFrm.backcall ||  !OpenSR)
                {
                    if ((KeyQueue.Count > 0 && GetKeyPressEventArgs(KeyQueue.Peek().MyKeyboardHookStruct) != null) && IsPressShift == false)
                    {
                        if (IsPressShift == true || ((KeyQueue.Count <= 2 && KeyQueue.Count > 0) && (Core.InputHelp.CheckCode(GetKeyPressEventArgs(KeyQueue.Peek().MyKeyboardHookStruct).KeyChar.ToString()) == false)))
                        {

                            InputFrm.SendText(revstr);
                            justxm = false;
                            int queuecount = KeyQueue.Count;
                            for (int i = 0; i < queuecount; i++)
                            {
                                KeyQueue.Dequeue();
                            }
                        }
                        else if (KeyQueue.Count == 1)
                        {

                            InputFrm.SendText(revstr);
                            justxm = false;
                            int queuecount = KeyQueue.Count;
                            for (int i = 0; i < queuecount; i++)
                            {
                                KeyQueue.Dequeue();
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!lsbjbsp)
                                InputFrm.SendText(revstr);
                            else lsbjbsp = false;
                        }
                        catch { return 1; }
                        justxm = false;
                        int queuecount = KeyQueue.Count;
                        for (int i = 0; i < queuecount; i++)
                        {
                            KeyQueue.Dequeue();
                        }
                    }

                }
                else
                {

                    inputFrm.backcall = true;
                    int queuecount = KeyQueue.Count;
                    for (int i = 0; i < queuecount; i++)
                    {
                        KeyQueue.Dequeue();
                    }
                }
                #endregion
            }

            if (keyData == Keys.ShiftKey || keyData == Keys.RShiftKey || keyData == Keys.LShiftKey
               || keyData == Keys.ControlKey || keyData == Keys.RControlKey || keyData == Keys.LControlKey
           || keyData == Keys.LWin || keyData == Keys.RWin || keyData == Keys.Escape
               || keyData == Keys.LMenu || keyData == Keys.RMenu || keyData == Keys.Alt)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

            return 1;
            #endregion

        }

        private bool CheckFunKey(Keys _key)
        {
            switch (_key)
            {
                case Keys.End: return true;
                case Keys.Home: return true;
                case Keys.PageUp: return true;
                case Keys.PageDown: return true;
                case Keys.Delete: return true;
                case Keys.Enter: return true;
                case Keys.Up: return true;
                case Keys.Down: return true;
                case Keys.Left: return true;
                case Keys.Right: return true;
                case Keys.Escape: return true;
                case Keys.F1: return true;
                case Keys.F2: return true;
                case Keys.F3: return true;
                case Keys.F4: return true;
                case Keys.F5: return true;
                case Keys.F6: return true;
                case Keys.F7: return true;
                case Keys.F8: return true;
                case Keys.F9: return true;
                case Keys.F10: return true;
                case Keys.F11: return true;
                case Keys.F12: return true;
                default: return false;
            }
        }
        private bool lsbjbsp = false;
 
        protected override void OnActivated(EventArgs e)
        {
            //if (IsViewSta)
            //    ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
            //else
            //    this.Hide();
        }
        //public static bool RunOSWin8=false;
        private void MiouIME_Load(object sender, EventArgs e)
        {
  
            inputFrm.Left = this.Left + this.Width + 10;
            inputFrm.Top = this.Top;
            //状态栏位于屏幕左下角
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - 55;
            this.Left = 5;

            Core.InputHelp.iniobj.IniReadValue("", "");

            inputFrm.Show();
            inputFrm.Hide();

            //初始化托盘程序的各个要素 
            Initializenotifyicon();
            this.ContextMenu = notifyiconMnu;
      
            picqbj_Click(null, null);
            this.Start();
    
            updateth = new Thread(new ThreadStart(this.GetNewVer));
            updateth.Start();

        }

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 用户按下任意键发生,在UseOnKeyDown事件后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool UseOnKeyPress(object sender, KeyPressEventArgs e, bool flag, Keys keye,bool downpress)
        {
            
            if (e == null)
            {
                //IsCallNextHookEx = true;
                return IsCallNextHookEx;
            }

            IsCallNextHookEx = false;
            if (e.KeyChar.GetHashCode() == (int)Core.KeysHashCode.空格)
            {
                if (MiouIME.OpenSR) flag = true;
                if (inputFrm.inputstr == "" && (KeyQueue.Count == 1 || KeyQueue.Count == 0))
                {
                    if (!MiouIME.OpenSR)
                        IsCallNextHookEx = true;
                    else if (MiouIME.OpenSR && inputFrm.backcall)
                        inputFrm.backcall = false;
                }
                else
                {
                    if (flag)
                    {
                        if (inputFrm.shangpingString == "")
                        {
                            inputFrm.EnterDown(false);
                            inputFrm.backcall = true;
                        }
                        else
                        {
                            if (MiouIME.OpenSR && inputFrm.inputstr != " " && KeyQueue.Count <2)
                                inputFrm.ShangPing(1);
                            else if (MiouIME.OpenSR && (inputFrm.inputstr == " " || KeyQueue.Count>1)) { }
                            else
                                inputFrm.ShangPing(1);

                            inputFrm.backcall = false;
                            if ((  OpenSR) && Is全角) lsbjbsp = true;
                        }

                    }
                }
            }
            else if (keye == Keys.Enter)
            {
                if (MiouIME.OpenSR) flag = true;
                if (inputFrm.inputstr == "" || inputFrm.inputstr == " ")
                {
                    IsCallNextHookEx = true;
                }
                if (flag)
                {
                    inputFrm.EnterDown(EnterCodeSP);
                }
            }
            else if (keye == Keys.Back)
            {
                if (MiouIME.OpenSR) flag = true;
                if (inputFrm.inputstr.Length - 1 > 0 || MiouIME.OpenSR)
                {
                    if (flag)
                    {
                        if (inputFrm.inputstr.Length <= 3)
                        {
                            inputFrm.EnterDown(false);
                        }
                        else
                        {
                            inputFrm.Input = "";
                            try
                            {
                                if (OpenSR)
                                {
                                    if (inputFrm.inputstr.Length > 0)
                                    {
                                        inputFrm.lastinput = inputFrm.lastinput.TrimEnd('|');
                                        int lastinput = int.Parse(inputFrm.lastinput.Split('|')[inputFrm.lastinput.Split('|').Length-1]);
                                        inputFrm.lastinput = inputFrm.lastinput.Substring(0, inputFrm.lastinput.Length - 1);
                                        if (inputFrm.inputstr.Length >= lastinput)
                                            inputFrm.inputstr = inputFrm.inputstr.Substring(0, inputFrm.inputstr.Length - lastinput);
                                        else
                                        {
                                            inputFrm.inputstr = inputFrm.inputstr.Substring(0, inputFrm.inputstr.Length - 1);
                                        }
                                        
                                    }
                                    else IsCallNextHookEx = true;
                                }
                                else
                                {
                                    if (inputFrm.inputstr.Length > 0)
                                    {
                                        inputFrm.inputstr = inputFrm.inputstr.Substring(0, inputFrm.inputstr.Length - 1);
                                    }
                                    else
                                        IsCallNextHookEx = true;
                                }
                            }
                            catch { inputFrm.inputstr = string.Empty; }
                            if (inputFrm.inputstr.Length > 0)
                                inputFrm.ShowInput(true, false, false, keye);
                            else
                            {
                                inputFrm.EnterDown(false);
                            }
                        }
                    }
                }
                else if (inputFrm.inputstr.Length - 1 == 0 && inputFrm.shangpingString != "")
                {
                    if (flag && inputFrm.shangpingString != "")
                    {
                        inputFrm.EnterDown(false);

                    }
                }
                else if (inputFrm.inputstr.Length - 1 <= 0 && inputFrm.shangpingString == "")
                {
                    IsCallNextHookEx = true;
                    if (flag)
                    {
                        inputFrm.EnterDown(false);

                    }
                }

            }
            else if (e.KeyChar.ToString() == "=" && inputFrm.inputstr != "")
            {
                IsCallNextHookEx = false;

                if (flag || MiouIME.OpenSR)
                {
                    inputFrm.NextPage();
                }
            }
            else if (e.KeyChar.ToString() == "-" && inputFrm.inputstr != "")
            {
                IsCallNextHookEx = false;
                if (flag || MiouIME.OpenSR)
                {
                    inputFrm.UpPage();
                }
            }
            else if (Core.InputHelp.CheckCode(e.KeyChar.ToString()) || (Core.InputHelp.输入模式 > 0 && Core.InputHelp.IsLowerLetter(e.KeyChar.ToString())))
            {
                //if (MiouIME.OpenSR) flag = true;
                IsCallNextHookEx = false;
                if (flag)
                {

                    inputFrm.Input = e.KeyChar.ToString();
                    inputFrm.ShowInput(true, false, false, keye);
                }
            }
            else if ((!flag  ) && KeyQueue.Count > 1 && Core.InputHelp.CheckCode(e.KeyChar.ToString()))
            {
                IsCallNextHookEx = false;
            }
            else if (inputFrm.shangpingString.Length > 0 && Core.InputHelp.IsNumber(e.KeyChar.ToString()))
            {
                IsCallNextHookEx = false;
                if (downpress ||   MiouIME.OpenSR)
                    inputFrm.ShangPing(Convert.ToInt32(e.KeyChar.ToString()));
    
            }
            else if (inputFrm.inputstr != "")
            {
                if (flag)
                {
                    if (justxm == false)
                    {
                        if (inputFrm.inputstr != " ")
                            inputFrm.ShangPing(1);
                        InputFrm.SendText(Core.InputHelp.CheckKeysString(keye));
                        IsCallNextHookEx = false;
                        inputFrm.EnterDown(false);
                    }

                }
            }
            else
            {
                IsCallNextHookEx = true;
            }

            return IsCallNextHookEx;
        }

        /// <summary>
        /// 用户释放任意键发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserOnKeyUp(object sender, KeyEventArgs e, ref bool isback)
        {

 
            lock (KeyQueue)
            {
                int queuecount = KeyQueue.Count;
                if (queuecount == 0)
                {
                    return;
                }
                SetCurPos();
                IsCallNextHookEx = true;
                if (OpenSR)
                {
                    #region 速录
                   
                    string srinput = string.Empty;
                    string psrinput = string.Empty;
                    string mustsrinput = string.Empty;
                    Core.KeysQueue _lkey = null;
                    //Core.KeysQueue _lskey = null;
                    bool srspace = false;
                    for (int i = 0; i < queuecount; i++) //出队
                    {
                        //if (_lkey != null) _lskey = _lkey;
                        _lkey=KeyQueue.Dequeue();
                        if (_lkey.KeyData == Keys.Space)
                        {
                            srspace = true;
                      
                        }
                        
                        srinput += Core.InputHelp.CheckKeysString(_lkey.KeyData);
                       
                    }
                    if (srinput.Length>0 && "-=＝－".IndexOf(srinput) >= 0) return;
                    int sppos = 0;
                    bool hleft = false;
                    bool hright = false;
                    if (srinput.IndexOf("　") >= 0 && srinput.Length>1)
                        srinput = srinput.Replace("　", "");
                    Core.InputHelp.CheckLetRight(srinput, out hleft, out hright);
                    psrinput = srinput;
                    if (srinput.Length > 0 && srinput.IndexOf("~") < 0 && Core.InputHelp.CovertStr(srinput).IndexOf(">") < 0 && inputFrm.inputstr.Length == InputFrm.shangpLen)
                    {
                        if (_lkey.KeyData == Keys.Space) InputFrm.spacexc = true;
                        inputFrm.ShangPing(1);
                    }
                    if (inputFrm.inputstr == " " && (_lkey.KeyData == Keys.Oemplus || _lkey.KeyData == Keys.OemMinus)) { InputFrm.spacexc = false; return; }
                    if (_lkey.KeyData != Keys.Space && inputFrm.inputstr == " ") { inputFrm.EnterDown(false); InputFrm.spacexc = false; }
                    if (BDOneOut && srinput.Length == 1
                        && inputFrm.inputstr.Length==0
                        && ",./;，。、；，．／；＇‘’".IndexOf(srinput)>=0)
                    {
                        InputFrm.spacexc = false;
                        InputFrm.SendText(srinput);
                    }
                    else if (inputFrm.inputstr.Length == 0 && srinput.Length == 0 && srspace && !InputFrm.spacexc) { InputFrm.SendText(" "); InputFrm.spacexc = false; }
                    else if (inputFrm.inputstr.Length == 0 && srinput == "　" && srspace && !InputFrm.spacexc) { InputFrm.SendText("　"); InputFrm.spacexc = false; }
                    else
                    {
                        InputFrm.spacexc = false;
                        if (!SROneOut || srinput.Length > 1 || (inputFrm.inputstr.Length > 0 && inputFrm.inputstr != " "))
                        {
                            if (inputFrm.inputstr.Length == InputFrm.shangpLen && SROneOut && srinput.Length == 1)
                            {
                                srinput = Core.InputHelp.CovertNoStr(srinput);
                                mustsrinput = Core.InputHelp.CovertStr(srinput);
                            }
                            else if (!SRAndNo || (SRAndNo && srinput.Length > 1))
                            {
                                srinput = Core.InputHelp.CovertStr(srinput);
                                mustsrinput = srinput;
                            }
                            else
                            {
                                srinput = Core.InputHelp.CovertNoStr(srinput);
                                mustsrinput = Core.InputHelp.CovertStr(srinput);
                            }
                            sppos = 0;
                            if (srinput.IndexOf(">") >= 0)
                            {
                                sppos = int.Parse(srinput.Substring(srinput.IndexOf(">") + 1, 1));
                                if (sppos > 0) srinput = srinput.Replace(">" + sppos, "");
                            }
                            if (srinput.Replace("~", "").Length + inputFrm.inputstr.Length > InputFrm.shangpLen)
                            {
                                inputFrm.ShangPing(1);
                            }
                            else if (srinput.Length == 1 && inputFrm.inputstr.Length % 2 == 0 && hright)
                            {
                                if (",./;，。、；，．／；＇‘’".IndexOf(psrinput) >= 0)
                                {
                                    inputFrm.ShangPing(1);
                                    InputFrm.spacexc = false;
                                    InputFrm.SendText(psrinput);
                                    return;
                                }
                                else
                                {
                                    if (psrinput.Length == 1) srinput = psrinput;
                                    inputFrm.ShangPing(1);
                                }
                            }
                            else if (srinput.Length == 1 && inputFrm.inputstr.Length % 2 == 0 
                                && inputFrm.inputstr.Length>3 && hleft)
                            {
                                inputFrm.ShangPing(1);
                            }
                      
                        }
                        else 
                            mustsrinput = Core.InputHelp.CovertStr(srinput);

                        if (srinput.IndexOf("{") >= 0)
                        {
                            SendKeys.Send(srinput);
                            srinput = string.Empty;
                            return;
                        }
                        int inputlen = 0;
                        if (sppos > 0) srinput = srinput.Replace(">" + sppos, "");
                        string nostr = string.Empty;
                        //for (int i = 0; i < srinput.Length; i++)
                        //{
                        //    if (Core.InputHelp.CheckCode(srinput.Substring(i, 1)))
                        //    {
                        //        inputFrm.inputstr += srinput.Substring(i, 1);
                        //        inputFrm.input = srinput.Substring(i, 1);

                        //        inputFrm.ShowInput(false, false, false, _lkey.KeyData);
                        //        inputlen++;
                        //    }
                        //    else
                        //        nostr += srinput.Substring(i, 1);
                        //}
                        if (srinput.Length > 0)
                        {
                            string inputss = string.Empty;
                            for (int i = 0; i < srinput.Length; i++)
                            {
                                if (Core.InputHelp.CheckCode(srinput.Substring(i, 1)))
                                {
                                    inputss += srinput.Substring(i, 1);
                                    inputlen++;
                                }
                                else
                                    nostr += srinput.Substring(i, 1);
                            }
                            inputFrm.inputstr += inputss;
                            inputFrm.input = inputss;

                            inputFrm.ShowInput(false, false, false, _lkey.KeyData,psrinput);
                        }
                        if (sppos > 0)
                        {
                            inputFrm.ShangPing(sppos);
                            sppos = 0;
                            return;
                        }
                        if (inputlen > 0)
                        {
                            inputFrm.lastinput += inputlen.ToString() + "|";
                            if (inputFrm.shangpingString.Length == 0)
                                inputFrm.EnterDown(false);
                        }
                        if (_lkey.KeyData != Keys.OemMinus && _lkey.KeyData != Keys.Oemplus && _lkey.KeyData != Keys.Back && (srspace || (SROneOut && inputFrm.inputstr.Length == 1 && inputFrm.inputstr != " ")))
                        {
                            string outstr = string.Empty;
                            if (hright)
                            {
                                foreach (var outstr1 in Core.InputHelp.srrightdict)
                                {
                                    if (outstr1.Split(' ').Length > 1 && outstr1.Split(' ')[0] == mustsrinput)
                                    {
                                        outstr = outstr1.Split(' ')[1];
                                        break;
                                    }
                                }
                            }
                            else if (hleft)
                            {
                                foreach (var outstr1 in Core.InputHelp.srleftdict)
                                {
                                    if (outstr1.Split(' ').Length > 1 && outstr1.Split(' ')[0] == mustsrinput)
                                    {
                                        outstr = outstr1.Split(' ')[1];
                                        break;
                                    }
                                }
                            }
                            if(_lkey.KeyData.ToString() == Core.InputHelp.Get第二重码选择键().Split('+')[0])
                            {
                                inputFrm.ShangPing(2);
                            }
                            else if (_lkey.KeyData.ToString() == Core.InputHelp.Get第三重码选择键().Split('+')[0])
                            {
                                inputFrm.ShangPing(3);
                            }
                            else
                            {
                                //psrinput=Core.InputHelp.SRSort(psrinput);
                                if (!string.IsNullOrEmpty(outstr))
                                {
                                    inputFrm.EnterDown(false);
                                    if (!MiouIME.Is简体)
                                    {
                                        outstr = Microsoft.VisualBasic.Strings.StrConv(outstr, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
                                    }
                                    InputFrm.SendText(outstr);
                                }
                                //else if (",./;，。、；，．／；＇‘’".IndexOf(psrinput.Substring(psrinput.Length-1)) >= 0)
                                //{
                                //    InputFrm.spacexc = false;
                                //    inputFrm.EnterDown(false);
                                //    InputFrm.SendText(psrinput.Substring(psrinput.Length - 1));
                                //}
                                else
                                    inputFrm.ShangPing(1);
                            }
                        }
                        else if (OpenOneSR && inputFrm.inputstr.Length > 2 && inputFrm.inputstr.Length % 2 != 0)
                        {
                            if (sppos == 0) sppos = 1;
                            if (inputFrm.shangpingString.Length > 0 && inputFrm.shangpingString.Split('|')[0].Length == 1)
                                inputFrm.ShangPing(sppos);
                        }
                        else if (OpenSR && inputFrm.inputstr.Length > 0 && nostr.Length > 0 && "－＝-=".IndexOf(nostr)<0)
                        {
                            if (inputFrm.shangpingString.Length > 0 && inputFrm.shangpingString.Split('|')[0].Length == 1)
                                inputFrm.ShangPing(1);
                            InputFrm.SendText(nostr);
                        }
                    }
                    if (srspace)
                        IsCallNextHookEx = false;
                    
                    #endregion
                }
                else
                {
                    for (int i = 0; i < queuecount; i++)
                    {
                        Core.KeysQueue keyqueuearry = KeyQueue.Dequeue();
                        isback = UseOnKeyPress(this, GetKeyPressEventArgs(keyqueuearry.MyKeyboardHookStruct), true, keyqueuearry.KeyData, false);

                    }
                }
            }

        }

        private KeyPressEventArgs GetKeyPressEventArgs(Core.KeyboardHookStruct MyKeyboardHookStruct)
        {

            byte[] keyState = new byte[256];
            GetKeyboardState(keyState);
            byte[] inBuffer = new byte[2];
            if (ToAscii(MyKeyboardHookStruct.vkCode,
              MyKeyboardHookStruct.scanCode,
              keyState,
              inBuffer,
              MyKeyboardHookStruct.flags) == 1)
            {
                return (new KeyPressEventArgs((char)inBuffer[0]));
            }

            return null;
        }

        private void MiouIME_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }

        private void MiouIME_MouseMove(object sender, MouseEventArgs e)
        {

            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }

        private void ShowWindow()
        {
            ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NoActiveWINDOW);
        }

        #region 得到光标在屏幕上的位置
        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(out Point lpPoint);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        private static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern void ClientToScreen(IntPtr hWnd, ref Point p);


 
        /// <summary>
        /// 设置光标位置
        /// </summary>
        private void GetCurPos()
        {
            #region 光标所在位置
            IntPtr ForegroundWindow = GetForegroundWindow();

            Core.InputForm.deskDC = Core.InputForm.GetTopDc();
            //lbinputname.Text = Core.InputForm.deskDC.ToString();
            MiouIME.ForegroundWindow = ForegroundWindow;
            curPoint = new Point();


            //得到Caret在屏幕上的位置  
            if (ForegroundWindow.ToInt32() != 0)
            {
                
                targetThreadID = GetWindowThreadProcessId(ForegroundWindow, IntPtr.Zero);



                if (localThreadID != targetThreadID)
                {
                    AttachThreadInput(localThreadID, targetThreadID, 1);
                    ForegroundWindow = GetFocus();

                    if (ForegroundWindow.ToInt32() != 0)
                    {
                        if (GetCaretPos(out  curPoint))
                        {
                            ClientToScreen(ForegroundWindow, ref  curPoint);
                        }

                        GUITHREADINFO gInfo = new GUITHREADINFO();
                        gInfo.cbSize = (uint)Marshal.SizeOf(gInfo);
                        bool bl = System.Convert.ToBoolean(GetGUIThreadInfo((uint)targetThreadID, ref gInfo));

                        if (bl)
                        {
                            if (gInfo.hwndCaret != ForegroundWindow)
                            {
                                if (inputFrm.Top != this.Top - 25)
                                    inputFrm.Top = this.Top - 25;
                                curPoint.Y = inputFrm.Top;
                                if (inputFrm.ViewType == 1)
                                {

                                    if (curPoint.Y > Screen.PrimaryScreen.WorkingArea.Height - inputFrm.Height - 10 - 25)
                                        curPoint.Y = Screen.PrimaryScreen.WorkingArea.Height - inputFrm.Height - 10 - 25;
                                }


                                if (curPoint.X < this.Left + this.Width + 10)
                                    curPoint.X = this.Left + this.Width + 10;

                            }
                            //else
                            //{
                            //    curPoint.X = (int)gInfo.rcCaret.Right;
                            //    curPoint.Y = (int)gInfo.rcCaret.Bottom;
                            //    ClientToScreen(gInfo.hwndCaret, ref  curPoint);
                            //}
                        }
                    }
                    else
                    {

                        curPoint.Y = Screen.PrimaryScreen.WorkingArea.Height - inputFrm.Height - 10 - 25;

                        if (curPoint.X < this.Left + this.Width + 10)
                            curPoint.X = this.Left + this.Width + 10;
                    }

                }
            }

            #endregion


        }
        /// <summary>
        /// 设置输入框光标的位置
        /// </summary>
        private void SetCurPos()
        {
            if (curTrac)
            {
                if (inputFrm.inputstr == "")
                {
                    GetCurPos();
                    if (curPoint.Y + 25 != inputFrm.Top)
                        inputFrm.Top = curPoint.Y + 25;

                    inputFrm.Left = curPoint.X + 10;
                }
            }
            else
            {
                inputFrm.Left = Screen.PrimaryScreen.WorkingArea.Width / 2-170;

                inputFrm.Top = Screen.PrimaryScreen.WorkingArea.Height - inputFrm.Height - 8;
            }
        }
        #endregion

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {

            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }

        private void piczyw_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (Is中文)
                    {
                        MiouIME.SRDelLast = 0;
                        Is中文 = false;
                        inputFrm.EnterDown(false);
                    }
                    else
                        Is中文 = true;

                    this.设置状态栏皮肤();
                }
            }
            catch { }
        }

        private void picqbj_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (Is全角)
                        Is全角 = false;
                    else
                        Is全角 = true;

                    this.设置状态栏皮肤();
                }
            }
            catch { }
        }

        private void picjdh_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (Is中文句号)
                        Is中文句号 = false;
                    else
                        Is中文句号 = true;

                    this.设置状态栏皮肤();
                }
            }
            catch { }
        }

        private void picjft_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (Is简体)
                        Is简体 = false;
                    else
                        Is简体 = true;

                    this.设置状态栏皮肤();
                }
            }
            catch { }
        }

        private void picrjp_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    //开启软件键盘
                    if (IsOpenSoftKeyboard)
                    {
                        //关闭软键盘
                        IsOpenSoftKeyboard = false;
                        KeyBoardfrm.HideWindow();
                    }
                    else
                    {
                        //打开软键盘
                        IsOpenSoftKeyboard = true;
                        KeyBoardfrm.ShowWindow();
                        KeyBoardfrm.KeyBoardsType = KeyBoardfrm.KeyBoardsType;
                    }
                }
            }
            catch { }

        }


        private NotifyIcon TrayIcon;
        public static ContextMenu notifyiconMnu;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, keymodifiers fsModifiers, Keys vk);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [Flags()]
        public enum keymodifiers
        {
            None = 0, Alt = 1, Control = 2, Shift = 4, Windows = 8
        }
        //访问官网
        private void AccWMUrl(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://srkmm.ysepan.com/");
        }

        //通过托盘的'退出'按钮关闭程序
        private void ExitSelect(object sender, System.EventArgs e)
        {
 
            try
            {
                Stop();
            }
            catch { }
            Core.InputForm.Break = true;
            UnregisterHotKey(Handle, 100);//释放快捷键注册
            //隐藏托盘程序中的图标 
            TrayIcon.Visible = false;
            try
            {
    
                try
                {
                    if (updateth != null)
                        updateth.Abort();
                }
                catch { }
            }
            catch { }
            //关闭系统 
            this.Dispose();
            this.Close();

        }

        /// <summary>
        /// 更新词库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateMB(object sender, System.EventArgs e)
        {
            for (int i = 0; i < mnuItms[8].MenuItems.Count; i++)
                mnuItms[8].MenuItems[i].Checked = false;
            MenuItem mite = (MenuItem)sender;
            mite.Checked = true;
            Core.InputHelp.iniobj.IniWriteValue("词库设置", "词库目录", mite.Tag.ToString());
            Core.InputHelp.UpdateMBThread();
            inputFrm.设置状态栏皮肤();
            this.设置状态栏皮肤();
            this.UpdateSet();
            Core.InputHelp.UpdateSet();
            inputFrm.UpdateSet();
        }

        //换服
        public void UpdateSkin(object sender, System.EventArgs e)
        {
            for (int i = 0; i < mnuItms[3].MenuItems.Count; i++)
                mnuItms[3].MenuItems[i].Checked = false;
            MenuItem mite = (MenuItem)sender;
            mite.Checked = true;
            Core.InputHelp.iniobj.IniWriteValue("皮肤设置", "皮肤目录", mite.Tag.ToString());

            Core.InputHelp.皮肤目录 = System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() +"\\skin\\" + mite.Tag.ToString();
            Core.InputHelp.pfiniobj = new Core.IniClass(Core.InputHelp.皮肤目录 + "\\skin.ini");
            Core.InputHelp.SkinExName = Core.InputHelp.pfiniobj.IniReadValue("皮肤信息", "SkinExName");
            if (string.IsNullOrEmpty(Core.InputHelp.SkinExName)) Core.InputHelp.SkinExName = "bmp";
            inputFrm.设置状态栏皮肤();
            inputFrm.IniLableSkin();
            this.设置状态栏皮肤();
        }
        /// <summary>
        /// 打开程序目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAppDir(object sender, System.EventArgs e)
        {
            string path = System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString();
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// 输入法设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImeSet(object sender, System.EventArgs e)
        {
            InputSetFrm isf = new InputSetFrm();
            if (isf.ShowDialog() == DialogResult.OK)
            {
                MiouIME.inputFrm.设置状态栏皮肤();
                inputFrm.EnterDown(false);
                inputFrm.HideWindow();
                this.UpdateSet();
                Core.InputHelp.UpdateSet();
                inputFrm.UpdateSet();
                if (IsViewSta)
                    ShowWindow();
                else
                    this.Hide();

                if (OpenSR)
                {
                    Core.InputHelp.bjdb一码上屏 = true;
                    Core.InputHelp.只用主码并击 = false;
                    Core.InputHelp.自动搜索主词库 = true;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutInfo(object sender, System.EventArgs e)
        {
            AboutFrm afrm = new AboutFrm();
            afrm.ShowDialog();
        }

        /// <summary>
        /// 打开软键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRJP(object sender, System.EventArgs e)
        {
            for (int i = 0; i < mnuItms[5].MenuItems.Count; i++)
                mnuItms[5].MenuItems[i].Checked = false;

            MenuItem mite = (MenuItem)sender;
            mite.Checked = true;
            //开启软件键盘
            IsOpenSoftKeyboard = true;
            KeyBoardfrm.ShowWindow();
            KeyBoardfrm.KeyBoardsType = mite.Tag.ToString();

        }
        /// <summary>
        /// 打开用户字词频统计窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenUWordCount(object sender, System.EventArgs e)
        {
            UWordCountFrm uwfrm = new UWordCountFrm();
            uwfrm.Show();
        }

        /// <summary>
        /// 字词频批量统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSWordCount(object sender, System.EventArgs e)
        {
            SWordCountfrm swfrm = new SWordCountfrm();
            swfrm.Show();
        }
        /// <summary>
        /// 单字合并器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordHB(object sender, System.EventArgs e)
        {
            WordJoinFrm wjfrm = new WordJoinFrm();
            wjfrm.Show();
        }
        private void ReStart(object sender, System.EventArgs e)
        {
            hKeyboardHook=SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(ModuleName), 0);
            //this.ReStart();
        }

        private void ImportWord(object sender, System.EventArgs e)
        {
            AddWordFrm objfrm = new AddWordFrm();
            objfrm.Show();
        }
        /// <summary>
        /// 词库导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordExp(object sender, System.EventArgs e)
        {
            GetWordFrm gwfrm = new GetWordFrm();
            gwfrm.Show();
        }
        /// <summary>
        /// 打开帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHelp(object sender, System.EventArgs e)
        {
            try
            {
                System.Diagnostics.Process Proc = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                Info.WorkingDirectory = Application.StartupPath + "\\";
                Info.FileName = "help.chm";
                if (File.Exists(Application.StartupPath + "\\help.chm"))
                {
                    Info.FileName = "help.chm";
                }
                else if (File.Exists(Application.StartupPath + "\\readme.txt"))
                {
                    Info.FileName = "readme.txt";
                }
                Proc = System.Diagnostics.Process.Start(Info);
            }
            catch { 
                
                MessageBox.Show("没找到平台帮助文件!"); 
            
            }
        }
        /// <summary>
        /// 打开输入法教程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenInputHelp(object sender, System.EventArgs e)
        {
            try
            {
                System.Diagnostics.Process Proc = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                Info.WorkingDirectory = Core.InputHelp.appPath + "\\MB\\" + Core.InputHelp.mbdir + "\\";
                Info.FileName = "help.chm";
                if (File.Exists(Info.WorkingDirectory + "help.chm"))
                {
                    Info.FileName = "help.chm";
                    Proc = System.Diagnostics.Process.Start(Info);
                }
                else if (File.Exists(Info.WorkingDirectory + "help.pdf"))
                {
                    Info.FileName = "help.pdf";
                    Proc = System.Diagnostics.Process.Start(Info);
                }
                else if (File.Exists(Info.WorkingDirectory + "help.doc"))
                {
                    Info.FileName = "help.doc";
                    Proc = System.Diagnostics.Process.Start(Info);
                }
                else if (Directory.Exists(Info.WorkingDirectory + "help\\"))
                {
                    System.Diagnostics.Process.Start(Info.WorkingDirectory + "help\\");
                }
                else
                {
                    MessageBox.Show("当前输入法目录没提供教程[help.chm/pdf/doc]!");
                }
            }
            catch { MessageBox.Show("当前输入法目录没提供教程[help.chm/pdf/doc]!"); }
        }

        /// <summary>
        /// 检查新版词库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdateVer(object sender, System.EventArgs e)
        {
            Thread uth = new Thread(new ParameterizedThreadStart(this.UpdateDit));
            uth.Start("true");
        }
 

        private void Initializenotifyicon()
        {
            //设定托盘程序的各个属性 
            TrayIcon = new NotifyIcon();
            try
            {
                TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\log32.ico");
            }
            catch
            {
                MessageBox.Show("图标文件未找到!", "装载图标出错");
                return;
            }
            TrayIcon.Text = "完美输入平台3.1";//鼠标移至托盘的提示文本
            TrayIcon.Visible = true;

            //定义一个MenuItem数组，并把此数组同时赋值给ContextMenu对象 
            mnuItms = new MenuItem[11];
            mnuItms[mnuItms.Length - 11] = new MenuItem();
            mnuItms[mnuItms.Length - 11].Text = "关于";
            mnuItms[mnuItms.Length - 11].Click += new System.EventHandler(this.AboutInfo);

            mnuItms[mnuItms.Length - 10] = new MenuItem();
            mnuItms[mnuItms.Length - 10].Text = "平台帮助";
            mnuItms[mnuItms.Length - 10].Click += new System.EventHandler(this.OpenHelp);

            mnuItms[mnuItms.Length - 9] = new MenuItem();
            mnuItms[mnuItms.Length - 9].Text = "输入法教程";
            mnuItms[mnuItms.Length - 9].Click += new System.EventHandler(this.OpenInputHelp);

            mnuItms[mnuItms.Length - 8] = new MenuItem();
            mnuItms[mnuItms.Length - 8].Text = "皮肤";
           DirectoryInfo dirinfo = new DirectoryInfo(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\Skin\\");
            DirectoryInfo[] dirlist = dirinfo.GetDirectories();
            Core.IniClass inimiou = new Core.IniClass(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\MiouIME.ini");
            for (int i = 0; i < dirlist.Length; i++)
            {
                MenuItem itme = new MenuItem();
                itme.Text = dirlist[i].Name;
                itme.Name = "pfmenu_" + dirlist[i].Name;
                itme.Tag = dirlist[i].Name;

                itme.Checked = dirlist[i].Name == inimiou.IniReadValue("皮肤设置", "皮肤目录");
                itme.Click += new System.EventHandler(this.UpdateSkin);
                mnuItms[mnuItms.Length - 8].MenuItems.Add(itme);
 
            }
            #region 工具

            mnuItms[mnuItms.Length - 7] = new MenuItem();
            mnuItms[mnuItms.Length - 7].Text = "工具";
            MenuItem itmetools = new MenuItem();
            itmetools.Text = "用户字词频统计";
            itmetools.Click += new System.EventHandler(this.OpenUWordCount);
            //mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "字词频批量统计";
            itmetools.Click += new System.EventHandler(this.OpenSWordCount);
            mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "单字合并替换";
            itmetools.Click += new System.EventHandler(this.WordHB);
            mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "词库导出";
            itmetools.Click += new System.EventHandler(this.WordExp);
            if (!Core.InputHelp.openJM)
                mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "添加/导入词库";
            itmetools.Click += new System.EventHandler(this.ImportWord);
            mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "平台修复";
            itmetools.Click += new System.EventHandler(this.ReStart);
            mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);
            itmetools = new MenuItem();
            itmetools.Text = "检查新版词库";
            itmetools.Click += new System.EventHandler(this.CheckUpdateVer);
            mnuItms[mnuItms.Length - 7].MenuItems.Add(itmetools);

 
            #endregion

            mnuItms[mnuItms.Length - 6] = new MenuItem();
            mnuItms[mnuItms.Length - 6].Text = "设置";
            mnuItms[mnuItms.Length - 6].Click += new System.EventHandler(this.ImeSet);

            #region 软键盘
            DirectoryInfo dirinfo1 = new DirectoryInfo(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\KeyBoards\\");
            FileInfo[] filelist = dirinfo1.GetFiles();
            for (int i = 0; i < filelist.Length; i++)
            {
                if (i == 0) mnuItms[mnuItms.Length - 5] = new MenuItem("软键盘");
                if (filelist[i].Name.Split('.')[1].ToUpper() != "KBD") continue;
                MenuItem itme = new MenuItem();
                itme.Text = filelist[i].Name.Split('.')[0];
                itme.Name = "rjpmenu_" + filelist[i].Name;
                itme.Tag = filelist[i].Name.Split('.')[0];
                itme.Click += new System.EventHandler(this.OpenRJP);
                mnuItms[mnuItms.Length - 5].MenuItems.Add(itme);
            }
            #endregion

            #region 程序目录
            mnuItms[mnuItms.Length - 4] = new MenuItem();
            mnuItms[mnuItms.Length - 4].Text = "程序目录";
            mnuItms[mnuItms.Length - 4].Click += new System.EventHandler(this.OpenAppDir);
            #endregion

            #region 词库列表
              dirinfo = new DirectoryInfo(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\MB\\");
             dirlist = dirinfo.GetDirectories();
              
            for (int i = 0; i < dirlist.Length; i++)
            {
                if (i == 0) mnuItms[mnuItms.Length - 3] = new MenuItem("输入法");
                Core.IniClass iniobj = new Core.IniClass(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\MB\\" + dirlist[i].Name + "\\MB.ini");
                MenuItem itme = new MenuItem();
                itme.Text = iniobj.IniReadValue("词库设置", "词库名");
                itme.Name = "srfmenu_" + dirlist[i].Name;
                itme.Tag = dirlist[i].Name;
                itme.Checked = dirlist[i].Name == inimiou.IniReadValue("词库设置", "词库目录");
                itme.Click += new System.EventHandler(this.UpdateMB);
                mnuItms[mnuItms.Length - 3].MenuItems.Add(itme);
            }

            #endregion

            mnuItms[mnuItms.Length - 2] = new MenuItem("-");
            mnuItms[mnuItms.Length - 2].Text = "进入官网";
            mnuItms[mnuItms.Length - 2].Click += new System.EventHandler(this.AccWMUrl);

            mnuItms[mnuItms.Length - 1] = new MenuItem();
            mnuItms[mnuItms.Length - 1].Text = "退出";
            mnuItms[mnuItms.Length - 1].Click += new System.EventHandler(this.ExitSelect);

            notifyiconMnu = new ContextMenu(mnuItms);
            TrayIcon.ContextMenu = notifyiconMnu;
            TrayIcon.MouseDoubleClick += new MouseEventHandler(TrayIcon_MouseDoubleClick);
            //为托盘程序加入设定好的ContextMenu对象 
        }
        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            #region 激活关闭输入法
            if (isActiveInput == true)
            {
                isActiveInput = false;
                MiouIME.SRDelLast = 0;
                if (IsViewSta)
                    this.Hide();
                inputFrm.EnterDown(false);
                TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\logh32.ico");
            }
            else
            {

                isActiveInput = true;
                if (IsViewSta)
                    ShowWindow();
                TrayIcon.Icon = new Icon(Application.StartupPath + @"\ico\log32.ico");


            }
            #endregion
        }
        private void MiouIME_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ExitSelect(null, null);

        }

        private void lbinputname_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                bool ok = false;
                if (e == null) ok = true;
                else
                {
                    MouseEventArgs ee = (MouseEventArgs)e;
                    if (ee.Button == MouseButtons.Left)
                    {
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (Core.InputHelp.输入模式 >= 2)
                    {
                        Core.InputHelp.IniCode();
                        Core.InputHelp.输入模式 = 0;
                    }
                    else
                    {
                        Core.InputHelp.IniPYCode();
                        Core.InputHelp.输入模式++;
                    }

                    Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "输入模式", Core.InputHelp.输入模式.ToString());
                    this.lbinputname.Text = Core.InputHelp.输入法名();

                }
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Interval < 5000) timer1.Interval = 5000;
            this.ReStart();
        }

        private void GetNewVer()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(3));
                if (autoupdate)
                {
                    UpdateDit(false);
                }
                Thread.Sleep(TimeSpan.FromHours(3));
            }
        }

        private void UpdateDit(object obj)
        {
            bool flag = bool.Parse(obj.ToString());
            #region update
            if (updateurl.Length > 0)
            {

                HttpWebRequest request = null;
                HttpWebResponse response = null;

                StreamReader reader = null;

                try
                {
                    request = (HttpWebRequest)WebRequest.Create(updateurl);
                    //request.KeepAlive = true;
                    request.AllowAutoRedirect = true;
                    //request.UserAgent = "Mozilla/4.0+(compatible;+MSIE+6.0;+Windows+NT+5.2;+SV1;+.NET+CLR+1.1.4322;+.NET+CLR+2.0.50727;+Alexa+Toolbar)";
                    request.Timeout = 1000 * 15;
                    request.Method = "GET";
                    //request.Referer = updateurl;

                    //接收返回的数据

                    response = (HttpWebResponse)request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                    string content = reader.ReadToEnd();
                    //content = content.Substring(content.IndexOf("<title>") + 7, content.IndexOf("</title>") - (content.IndexOf("<title>") + 7));
                    if (string.Compare(content.Trim(), lastupdate.Trim(), true) != 0)//有新版本
                    {
                        if (MessageBox.Show("有新词库,是否下载 !\r\n下载将自动进行,下载完成后会有提示", "新版本词库下载", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                WebClient client = new WebClient();
                                string filename = Core.InputHelp.appPath + "\\MB\\" + Core.InputHelp.mbdir + "\\dictbak.txt";
                                client.DownloadFile(updateurl.Substring(0, updateurl.LastIndexOf("/")) + "/dict.txt", filename);

                                Thread.Sleep(200);
                                if (File.Exists(filename))
                                {
                                    File.WriteAllText(Core.InputHelp.mbPath, File.ReadAllText(filename, Encoding.Unicode), Encoding.Unicode);
                                    Thread.Sleep(100);
                                    File.Delete(filename);
                                }
                                //替换词库信息File.WriteAllLines(Core.InputHelp.mbPath, File.ReadAllLines(filename, Encoding.Unicode), Encoding.Unicode);
                                filename = Core.InputHelp.appPath + "\\MB\\" + Core.InputHelp.mbdir + "\\mbinfo.txt";
                                client.DownloadFile(updateurl.Substring(0, updateurl.LastIndexOf("/")) + "/mbinfo.txt", filename);
                                if (File.Exists(filename))
                                {
                                    File.WriteAllLines(Core.InputHelp.mbiniobj.path, File.ReadAllLines(filename, Encoding.Unicode), Encoding.Unicode);
                                    Thread.Sleep(100);
                                    File.Delete(filename);
                                }
                                Thread.Sleep(200);
                                client.Dispose();
                                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "lastupdate", content.Trim());
                                lastupdate = content.Trim();
                                if (MessageBox.Show("新版本词库下载完毕! 版本号为:" + content.Trim() + "\r\n是否装载新版词库?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    try
                                    {
                                        Core.InputHelp.UpdateMB();
                                    }
                                    catch { }
                                    Thread.Sleep(3000);
                                    MessageBox.Show("装载完毕!");
                                }



                            }
                            catch { MessageBox.Show("下载失败! "); }

                        }
                    }
                    else
                        if (flag)
                            MessageBox.Show("当前是最新版本!");
                }
                catch
                {
                    if (flag)
                        MessageBox.Show("网络连接有误!");
                }
                finally
                {
                    request.Abort();
                }

            }
            else
            {
                if (flag)
                    MessageBox.Show("未配置下载地址!");
            }

            #endregion
        }

        private void requestreg()
        {
            while (!Core.InputForm.Break)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                if (((TimeSpan)(DateTime.Now - startDate)).TotalMinutes >= 66)
                {
                    try
                    {
                        startDate.AddMonths(2);
                        //
                    }
                    catch { }
                }
            }
        }
    }
}
