using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Microsoft.International.Converters.PinYinConverter;
using System.Runtime.Serialization.Formatters.Binary; 

namespace MiouIME.Core
{
    /// <summary>
    /// 用于对装载词库，通过编码查找对应输出值
    /// </summary>
    public class InputHelp
    {
        public static IniClass iniobj = new IniClass(System.IO.Directory.GetParent(Application.ExecutablePath.ToString()).ToString() + "\\MiouIME.ini");
        public static IniClass pfiniobj;
        public static IniClass mbiniobj;
        public static bool isComplete = false;
        public static string appPath = "";//应用程序目录
        public static string mbPath = "";//词库路径
        public static string codePath = "";//参与编码的字符字典目录
        public static string mbdir = "";//词库目录名
        public static string mbfile = "";//词库名
        public static string 皮肤目录 = "";//皮肤目录
        public static string SkinExName = "bmp";//皮肤图片的格式
        public static string SkinFontName = "宋体";//字体名
        public static int SkinFontSize = 12;//字体大小
        public static int SkinFontH = 20;//字体高度
        public static int SkinFontJG = 25;//输入与候选汉字的间高
        public static int SkinWidth = 160;//汉字候选框宽度
        public static int SkinHeith = 44;//汉字候选框高度
        public static Color Skinbordpen = Color.Gray;//边框色
        public static Color Skinbstring = Color.Black;//字体颜色
        public static Color Skinbcstring = Color.Fuchsia;//提示补码颜色
        public static Color Skinfbcstring = Color.Red;//第一候选框字体颜色

        public static int SkinStateWidth = 190;//状态栏宽
        public static int SkinStateHeith = 25;//状态栏高
        public static string SkinStateFontName = "宋体";//状态栏输入法字体名
        public static bool SkinStateStringView = true;//状态栏输入法名是否显示,1显示,0不显示
        public static int SkinStateStringWidth = 100;//状态栏输入法名是否显示,1显示,0不显示
        public static Color SkinStateFontColor = Color.Fuchsia;//状态栏输入法颜色

        public static string[] mddtary;//词库字典
        public static string[] pydtary;//拼音词库字典
        public static string[] mdcode;//参与编码字符字典
        public static string[] srmap;//速录map
 
        public static string[] qjzwdict;//全角中文按键字典
        public static string[] bjzwdict;//半角中文按键字典
        public static string[] qjywdict;//全角英文按键字典
        public static string[] srleftdict;//速录左手一简
        public static string[] srrightdict;//速录右手一简
        public static PostIndex[] posindex;//编码位置索引，提高搜索效率
        public static PostIndex[] pyposindex;//拼音编码位置索引，提高搜索效率
        public static bool indexComplete = false;//索引完成否
        public static string 特殊键码 = "";
        public static string 停止键码 = "";
        public static string 首字母排除键位 = "";
        public static string 是否自动上屏 = "0";
 
        public static short firstdh = 1;
        public static short 输入模式 = 0;
        public static bool SingleInput = true;//单字模式输出
        public static bool bjdb一码上屏 = false;
        public static bool bjdb2一码上屏 = false;
        public static bool bjdb3一码上屏 = false;
        public static bool bjdb4一码上屏 = false;
        public static bool bjdb5一码上屏 = false;
        public static bool 只用主码并击 = false;
        public static bool 自动搜索主词库 = false;

        static List<MapintKey> mapsortkeys = new List<MapintKey>();
        static List<MapintKey> mapkeys = new List<MapintKey>();
        static InputHelp()
        {
            UpdateMBThread();
        }

        ~InputHelp()
        {
            GC.ReRegisterForFinalize(this);
        }
 
        /// <summary>
        /// 设置信息更新
        /// </summary>
        public static void UpdateSet()
        {
            特殊键码 = mbiniobj.IniReadValue("词库设置", "特殊键码");
            停止键码 = mbiniobj.IniReadValue("词库设置", "停止键码");
            首字母排除键位 = mbiniobj.IniReadValue("词库设置", "首字母排除键位");
            是否自动上屏 = mbiniobj.IniReadValue("词库设置", "是否自动上屏");

            bjdb一码上屏 = Core.InputHelp.iniobj.IniReadValue("并击设置", "bjdb一码上屏") == "0" ? false : true;
            bjdb2一码上屏 = Core.InputHelp.iniobj.IniReadValue("并击设置", "bjdb2一码上屏") == "0" ? false : true;
            bjdb3一码上屏 = Core.InputHelp.iniobj.IniReadValue("并击设置", "bjdb3一码上屏") == "0" ? false : true;
            bjdb4一码上屏 = Core.InputHelp.iniobj.IniReadValue("并击设置", "bjdb4一码上屏") == "0" ? false : true;
            bjdb5一码上屏 = Core.InputHelp.iniobj.IniReadValue("并击设置", "bjdb5一码上屏") == "0" ? false : true;
            只用主码并击 = Core.InputHelp.iniobj.IniReadValue("并击设置", "只用主码并击") == "0" ? false : true;
            自动搜索主词库 = Core.InputHelp.iniobj.IniReadValue("并击设置", "自动搜索主词库") == "0" ? false : true;
            SingleInput = Core.InputHelp.iniobj.IniReadValue("功能设置", "单字模式") == "0" ? false : true;
            openJM = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "加密") == "1" ? true : false;
            输入模式 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "输入模式") == "" ? (short)0 : short.Parse(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "输入模式"));
            InputFrm.shangpLen = Convert.ToInt32(mbiniobj.IniReadValue("词库设置", "最大码长"));

            #region 速录映射
            mapsortkeys = new List<MapintKey>();
            string mapstr = "~1qaz2wsx3edc4rfv5tgb6yhn7ujm8ik,9ol.0p;/-['=]，。、；";

            for (short i = 0; i < mapstr.Length; i++)
            {
                MapintKey map = new MapintKey();
                map.ZM = mapstr.Substring(i, 1);
                map.Pos = i;
                mapsortkeys.Add(map);
            }

            if (!File.Exists(appPath + "\\MB\\" + mbdir + "\\maping.txt"))
                File.WriteAllText(appPath + "\\MB\\" + mbdir + "\\maping.txt", "");
            srmap = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\maping.txt");
            mapkeys = new List<MapintKey>();
            foreach (string m in srmap)
            {
                if (!string.IsNullOrEmpty(m))
                {
                    MapintKey map = new MapintKey();
                    string zm = m.Split('=')[0];
                    mapsortkeys.FindAll(ma => zm.IndexOf(ma.ZM) >= 0).OrderBy(o => o.Pos).ToList().ForEach(f => map.ZM += f.ZM);
                    map.Pos = (short)m.Split('=')[0].Length;
                    map.Map = m.Split('=')[1];
                    mapkeys.Add(map);
                }
            }
            if (mapkeys.Find(m => m.Map == "~") == null)
            {
                MapintKey map = new MapintKey();
                map.ZM = "~";
                map.Pos = 1;
                map.Map = "~";
                mapkeys.Add(map);
            }
            mapkeys = mapkeys.OrderByDescending(m => m.Pos).ToList();

            #endregion
        }
        #region 词库相关
        /// <summary>
        /// 保存手工调频的数据
        /// </summary>
        public static void SaveMove(object inputstr)
        {
       
                if (输入模式 == 0)
                {
                    File.WriteAllLines(mbPath, mddtary, Encoding.Unicode);
                    Core.comm.EncryptMB(Core.InputHelp.mbPath);
                    InputFrm.shangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长"));
                    InputFrm.wcmshangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "无重码上屏码长"));
                }
                else if (输入模式 == 2)
                {
                    File.WriteAllLines(appPath + "\\MB\\" + mbdir + "\\pinyin.txt", pydtary, Encoding.Unicode);
                    //Core.comm.EncryptMB(Core.InputHelp.mbPath);
                    InputFrm.shangpLen = 100;
                    InputFrm.wcmshangpLen = 100;
                }
                else
                {
                    InputFrm.shangpLen = 100;
                    InputFrm.wcmshangpLen = 100;
                }
          
        }
        public static bool openJM = false;
        public static void UpdateMB()
        {

            isComplete = false;
            if (mbPath != "")
            {

                openJM = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "加密") == "1" ? true : false;
                if (openJM)
                {
                    //解密
                    string jmc = comm.Decrypt(mbPath).Replace("\r\n", "\n");
                    mddtary = jmc.Split('\n');
                }
                else
                {
                    Encoding txtcode = GetFileType(mbPath);
                    mddtary = File.ReadAllLines(mbPath, txtcode);
                }

                
                try
                {
                    for (int i = 0; i < mddtary.Length; i++)
                    {
                        if (mddtary[i].Trim().Length > 0 && mddtary[i].Split(' ').Length > 1 && mddtary[i].Split(' ')[0].Length > 0)
                        {
                            mddtary[i] = mddtary[i];
                        }
                        else
                            mddtary[i] = ") (";
                    }
                    mddtary = mddtary.OrderBy(o => o.Split(' ')[0].Substring(0, 1)).ToArray();
                }
                catch
                {

                }
                Application.DoEvents();
                pydtary = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\pinyin.txt", Encoding.Unicode);
                for (int i = 0; i < pydtary.Length; i++)
                {
                    if (pydtary[i].Trim().Length > 0 && pydtary[i].Split(' ').Length > 1)
                        pydtary[i] = pydtary[i];
                }
                pydtary = pydtary.OrderBy(o => o.Split(' ')[0]).ToArray();
                Application.DoEvents();
                mdcode = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\code.txt", Encoding.Unicode);
                if (输入模式 != 0) IniPYCode();
                qjzwdict = File.ReadAllLines(appPath + "\\dict\\qjzw.txt", Encoding.Unicode);
                bjzwdict = File.ReadAllLines(appPath + "\\dict\\bjzw.txt", Encoding.Unicode);
                qjywdict = File.ReadAllLines(appPath + "\\dict\\qjyw.txt", Encoding.Unicode);

                if (!File.Exists(appPath + "\\MB\\" + mbdir + "\\SRLeftOne.txt"))
                {
                    File.WriteAllText(appPath + "\\MB\\" + mbdir + "\\SRLeftOne.txt", "", Encoding.UTF8);
                }
                if (!File.Exists(appPath + "\\MB\\" + mbdir + "\\SRRightOne.txt"))
                {
                    File.WriteAllText(appPath + "\\MB\\" + mbdir + "\\SRRightOne.txt", "", Encoding.UTF8);
                }
                srleftdict = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\SRLeftOne.txt", Encoding.UTF8);
                srrightdict = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\SRRightOne.txt", Encoding.UTF8);
   
                Thread th = new Thread(new ThreadStart(UpdateIndex));//创建索引线程
                th.Start();
           
                Thread th1 = new Thread(new ThreadStart(UpdatePYIndex));//创建索引线程
                th1.Start();
         
            }

            isComplete = true;
        }
        public static void UpdateZMB()
        {
            if (openJM)
            {
                //解密
                string jmc = comm.Decrypt(mbPath).Replace("\r\n", "\n");
                mddtary = jmc.Split('\n');
            }
            else
                mddtary = File.ReadAllLines(mbPath, Encoding.Unicode);


        }

        public static void UpdateMBThread()
        {
            appPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
            mbdir = iniobj.IniReadValue("词库设置", "词库目录");
            mbfile = iniobj.IniReadValue("词库设置", "词库文件");
            mbPath = appPath + "\\MB\\" + mbdir + "\\dict.txt";
            皮肤目录 = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + "\\Skin\\" + iniobj.IniReadValue("皮肤设置", "皮肤目录");
            pfiniobj = new IniClass(皮肤目录 + "\\skin.ini");
            SkinExName = pfiniobj.IniReadValue("皮肤信息", "SkinExName");
            if (string.IsNullOrEmpty(SkinExName)) SkinExName = "bmp";
            mbiniobj = new IniClass(appPath + "\\MB\\" + mbdir + "\\mb.ini");
            UpdateSet();
            UpdateMB();
 
        }

        /// <summary>
        /// 建立字库索引
        /// </summary>
        public static void UpdateIndex()
        {
            posindex = new PostIndex[mdcode.Length];
            for (int i = 0; i < mdcode.Length; i++)
            {
                posindex[i] = new PostIndex();
            }
            int count = -1;
            int counti = 0;
            string oldl = "";
            string newl = "a";
            for (int i = 0; i < mddtary.Length; i++)
            {
                if (mddtary[i].Trim().Length == 0) continue;
                newl = mddtary[i].Substring(0, 1);
                //if (!CheckCode(newl)) continue;
                if (oldl != newl)
                {
                    count++;
                    if (count >= posindex.Length)
                    {
                        break;
                    }
                    posindex[count] = new PostIndex();
                    oldl = newl;
                    posindex[count].letter = oldl;
                    posindex[count].first = i;
                    posindex[count].last = i;
                    counti = i;
                }
                else
                {
                    posindex[count].last = i;
                }

            }
            indexComplete = true;
        }
        /// <summary>
        /// 建立拼音索引
        /// </summary>
        public static void UpdatePYIndex()
        {
            indexComplete = false;
            pyposindex = new PostIndex[mdcode.Length];
            for (int i = 0; i < mdcode.Length; i++)
            {
                pyposindex[i] = new PostIndex();
            }
            int count = -1;
            int counti = 0;
            string oldl = "";
            string newl = "a";
            for (int i = 0; i < pydtary.Length; i++)
            {
                newl = pydtary[i].Substring(0, 1);
                if (oldl != newl)
                {
                    count++;
                    pyposindex[count] = new PostIndex();
                    oldl = newl;
                    pyposindex[count].letter = oldl;
                    pyposindex[count].first = i;
                    pyposindex[count].last = i;
                    counti = i;
                }
                else
                {
                    pyposindex[count].last = i;
                }

            }
            indexComplete = true;
        }

        /// <summary>
        /// 获取词库索引
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static PostIndex GetPos(string letter)
        {
            foreach (PostIndex pti1 in posindex)
                if (pti1.letter == letter)
                    return pti1;
            return new PostIndex();
        }
        /// <summary>
        /// 获取拼音索引
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static PostIndex GetPYPos(string letter)
        {
            foreach (PostIndex pti1 in pyposindex)
                if (pti1.letter == letter.ToLower())
                    return pti1;
            return new PostIndex();
        }
        /// <summary>
        /// 搜索输出值
        /// </summary>
        /// <param name="inputstr">输入编码</param>
        /// <returns></returns>
        public static string[] GetInputValue(string inputstr, bool dream = false)
        {

            if (inputstr.Length == 0) return null;

            string valuestr = "";
            int count = 0;
            int first = 0, last = mddtary.Length - 1;
            int pcount = SingleInput == true ? 50 : 100;
            #region 取字
            if (indexComplete)
            {
                PostIndex poi = GetPos(inputstr.Substring(0, 1));
                first = poi.first;
                last = poi.last;

            }
            if (dream)
            {
                valuestr = GetGroupValue(inputstr);
            }
            else
            {
                #region
                if (输入模式 != 2)
                {
                    //if (last > 0)
                    //{
                    for (int i = first; i <= last; i++)
                    {
                        if (mddtary[i].StartsWith(inputstr))
                        {
                            string strarr = mddtary[i];
                            string fcode = strarr.Split(' ')[0];
                            string[] fvalue = strarr.Substring(strarr.Split(' ')[0].Length).Trim().Split(' ');
                            for (int j = 0; j < fvalue.Length; j++)
                            {
                                int startint = fcode.IndexOf(inputstr) + inputstr.Length;
                                if (SingleInput)
                                {
                                    if (fvalue[j].Length == 1 || CheckChinese(fvalue[j].Substring(0, 1)) == false)
                                    {
                                        if (valuestr.IndexOf("z" + fvalue[j] + "|") < 0)
                                        {
                                            if (string.Compare(MiouIME.GBCode, "unicode", true) == 0)
                                            {
                                                valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                            }
                                            else if (string.Compare(MiouIME.GBCode, "gbk", true) == 0)
                                            {
                                                if (IsGBKCode(fvalue[j].Substring(0, 1)))
                                                    valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                            }
                                            else
                                            {
                                                if (IsGBCode(fvalue[j].Substring(0, 1)))
                                                    valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                            }
                                            count++;
                                        }
                                    }
                                }
                                else if (!SingleInput)
                                {
                                    if (valuestr.IndexOf("z" + fvalue[j] + "|") < 0)
                                    {
                                        if (string.Compare(MiouIME.GBCode, "unicode", true) == 0)
                                        {
                                            valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                        }
                                        else if (string.Compare(MiouIME.GBCode, "gbk", true) == 0)
                                        {
                                            if (IsGBKCode(fvalue[j].Substring(0, 1)))
                                                valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                        }
                                        else
                                        {
                                            if (IsGBCode(fvalue[j].Substring(0, 1)))
                                                valuestr += i + "z" + j + "z" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                        }
                                        count++;
                                    }
                                }

                                if (count < 2 && 输入模式 == 1)
                                {
                                    valuestr += GetPingValue(inputstr, 100);
                                }

                                if (count > pcount) break;
                            }
                        }

                        if (count > pcount) break;
                    }
                    if (count < 2 && 输入模式 == 1)
                    {
                        valuestr += GetPingValue(inputstr, 100);
                    }
                    //}
                    else if (输入模式 == 1)
                    {
                        valuestr += GetPingValue(inputstr, 100);
                    }
                    //if (valuestr.IndexOf('|') < 0)
                    //    valuestr += GetZJValue("", inputstr);
                }
                else if (输入模式 == 2)
                {
                    valuestr = GetPingValue(inputstr, 100);
                }
                #endregion
            }
     
            if (valuestr.Length > 0)
            {
                valuestr = valuestr.Replace("^", "＾");
                if (!MiouIME.Is简体)
                {
                    valuestr = Microsoft.VisualBasic.Strings.StrConv(valuestr, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
                }
                string[] vsa = valuestr.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

      
                return vsa;

            }
            #endregion
            return null;
        }

        /// <summary>
        /// 获取词组联想汉字
        /// </summary>
        /// <param name="pres"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetGroupValue(string pres, int count = 50)
        {
            string vstring = string.Empty;
            int pos = 0;
            foreach (string vv in mddtary.Where(s => s.Split(' ').Length > 1 && s.Split(' ')[1].StartsWith(pres) && s.Split(' ')[1] != pres))
            {
                if (pos > count) break;
                if (vstring.IndexOf("z" + vv.Split(' ')[1] + "|") < 0)
                {
                    vstring += string.Format("{0}z{0}z", pos) + vv.Split(' ')[1] + "|\n";
                    pos++;
                }

            }
            return vstring;
        }

        public class OutputObj
        {
            public string V = string.Empty;
            public string C = string.Empty;
            public int S = 0;
            public int P = 0;
        }
        /// <summary>
        /// 加入拼音参与的编码
        /// </summary>
        public static void IniPYCode()
        {
            mdcode = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\code.txt", Encoding.Unicode);
            string pycode = "qwertyuiopasdfghjklzxcvbnm";
            InputFrm.shangpLen = 100;
            InputFrm.wcmshangpLen = 100;
            for (int i = 0; i < pycode.Length; i++)
            {
                bool hava = false;
                foreach (string s in mdcode)
                {
                    if (s.ToLower() == pycode.Substring(i, 1)) { hava = true; break; }
                }
                if (!hava)
                {
                    string[] tmdcode = new string[mdcode.Length + 1];
                    mdcode.CopyTo(tmdcode, 0);
                    tmdcode[tmdcode.Length - 1] = pycode.Substring(i, 1);
                    mdcode = tmdcode;
                }
            }
        }
        /// <summary>
        /// 加入拼音参与的编码
        /// </summary>
        public static void IniCode()
        {
            //mdcode = File.ReadAllLines(appPath + "\\MB\\" + mbdir + "\\code.txt", Encoding.Unicode);
            InputFrm.shangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长"));
            InputFrm.wcmshangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "无重码上屏码长"));
        }
        /// <summary>
        /// 搜索拼音汉字
        /// </summary>
        /// <param name="inputstr"></param>
        /// <param name="outstr"></param>
        /// <param name="count">搜索多少个</param>
        /// <returns></returns>
        public static string GetPingValue(string inputstr, int countnum)
        {
            if (inputstr.Length == 0) return "";
            int count = 0;
            string valuestr = "";
            countnum = 500;
            int first = 0, last = pydtary.Length - 1;
            if (indexComplete)
            {
                PostIndex poi = GetPYPos(inputstr.Substring(0, 1));
                first = poi.first;
                last = poi.last;

            }
            //取拼音
            for (int i = first; i <= last; i++)
            {
                if (pydtary[i].StartsWith(inputstr))
                {
                    string strarr = pydtary[i];
                    string fcode = strarr.Split(' ')[0];
                    string[] fvalue = strarr.Replace(strarr.Split(' ')[0], "").Trim().Split(' ');
                    for (int j = 0; j < fvalue.Length; j++)
                    {
                        int startint = fcode.IndexOf(inputstr) + inputstr.Length;
                        if (SingleInput)
                        {
                            if (fvalue[j].Length == 1 || CheckChinese(fvalue[j].Substring(0, 1)) == false)
                            {
                                if (valuestr.IndexOf("p" + fvalue[j] + "|") < 0)
                                {
                                    if (string.Compare(MiouIME.GBCode, "unicode", true) == 0)
                                    {
                                        valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                    }
                                    else if (string.Compare(MiouIME.GBCode, "gbk", true) == 0)
                                    {
                                        if (IsGBKCode(fvalue[j].Substring(0, 1)))
                                            valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                    }
                                    else
                                    {
                                        if (IsGBCode(fvalue[j].Substring(0, 1)))
                                            valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                    }

                                    count++;
                                }
                            }
                        }
                        else if (!SingleInput)
                        {
                            if (valuestr.IndexOf("p" + fvalue[j] + "|") < 0)
                            {
                                if (string.Compare(MiouIME.GBCode, "unicode", true) == 0)
                                {
                                    valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                }
                                else if (string.Compare(MiouIME.GBCode, "gbk", true) == 0)
                                {
                                    if (IsGBKCode(fvalue[j].Substring(0, 1)))
                                        valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                }
                                else
                                {
                                    if (IsGBCode(fvalue[j].Substring(0, 1)))
                                        valuestr += i + "p" + j + "p" + fvalue[j] + "|" + (InputFrm.OpenCodeView == true ? (startint < fcode.Length ? fcode.Substring(startint, fcode.Length - inputstr.Length) : "") : "") + "\n";
                                }

                                count++;
                            }
                        }

                        if (count >= countnum) break;
                    }
                }
                if (count >= countnum) break;
            }
            return valuestr;
        }


   
        /// <summary>
        /// 替换系统变量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSystem(string str)
        {
            //str = str.ToLower();
            str = str.Replace("$20", " ");
            str = str.Replace("$y", DateTime.Now.Year.ToString());
            str = str.Replace("$d", DateTime.Now.Day.ToString().PadLeft(2, '0'));
            str = str.Replace("$h", DateTime.Now.Hour.ToString().PadLeft(2, '0'));
            str = str.Replace("$mi", DateTime.Now.Minute.ToString().PadLeft(2, '0'));
            str = str.Replace("$m", DateTime.Now.Month.ToString().PadLeft(2, '0'));
            str = str.Replace("$s", DateTime.Now.Second.ToString().PadLeft(2, '0'));
            str = str.Replace("$w", DateTime.Now.DayOfWeek.ToString().PadLeft(2, '0'));
            str = str.Replace("$os", System.Environment.OSVersion.Platform.ToString());
            str = str.Replace("$mname", System.Environment.MachineName.ToString());

            return str;
        }
 

        /// <summary>
        /// 获取的内存词库
        /// </summary>
        /// <param name="inputstr"></param>
        /// <returns></returns>
        public static string[] GetInputMB(string inputstr)
        {
 
            return mddtary;
        }

        /// <summary>
        /// 判定是否为汉字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckChinese(string str)
        {
            return Regex.IsMatch(str, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 判断输入的编码是否是参与的编码的字符
        /// </summary>
        /// <param name="codechar"></param>
        /// <returns></returns>
        public static bool CheckCode(string codechar)
        {
            bool valuebool = false;
            for (int i = 0; i < mdcode.Length; i++)
            {
                if (mdcode[i].Trim() == codechar)
                {
                    valuebool = true;
                    break;
                }
            }
            if (MiouIME.OpenSR && !valuebool) valuebool = CheckSRCode(codechar);
            return valuebool;
        }

        /// <summary>
        /// 判断输入的编码是否是参与的编码的字符
        /// </summary>
        /// <param name="codechar"></param>
        /// <returns></returns>
        public static bool CheckSRCode(string codechar)
        {
            if (codechar == "~") return false;
            bool valuebool = mapkeys.FindAll(f => f.ZM == codechar).Count > 0;

            return valuebool;
        }

        /// <summary>
        /// 判断一个word是否为GB2312编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsGBCode(string word)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(word);
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code or other code
            {
                return false;
            }

            //判断是否是GB2312
            byte byte1 = bytes[0];
            byte byte2 = bytes[1];
            return (byte1 >= 176 && byte1 <= 247 && byte2 >= 160 && byte2 <= 254);
        }

        /// <summary>
        /// 判断一个word是否为GBK编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsGBKCode(string word)
        {
            byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(word.ToString());
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code
            {
                return false;
            }


            byte byte1 = bytes[0];
            byte byte2 = bytes[1];
            return (byte1 >= 129 && byte1 <= 254 && byte2 >= 64 && byte2 <= 254);
        }

        /// <summary>
        /// 判断一个word是否为Big5编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsBig5Code(string word)
        {
            byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(word.ToString());
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code
            {
                return false;
            }

            byte byte1 = bytes[0];
            byte byte2 = bytes[1];
            return ((byte1 >= 129 && byte1 <= 254) && ((byte2 >= 64 && byte2 <= 126) || (byte2 >= 161 && byte2 <= 254)));
        }
        /// <summary>
        /// 判断输入的编码是否是26个小写字母
        /// </summary>
        /// <param name="codechar"></param>
        /// <returns></returns>
        public static bool IsLowerLetter(string codechar)
        {
            if (codechar.Length > 1) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(codechar, "[a-z]");
        }

        /// <summary>
        /// 判断输入的编码是否是26个大写字母
        /// </summary>
        /// <param name="codechar"></param>
        /// <returns></returns>
        public static bool IsUpperLetter(string codechar)
        {
            if (codechar.Length > 1) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(codechar, "[A-Z]");
        }

        /// <summary>
        /// 判断输入的编码是否是0-9的数字
        /// </summary>
        /// <param name="codechar"></param>
        /// <returns></returns>
        public static bool IsNumber(string codechar)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(codechar, "[0-9]");
        }

        /// <summary>
        /// 将字符串位数进行组合并输出数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ArrayList GetStringArry(string word)
        {
            if (word == "") return new ArrayList();
            ArrayList result = GetStringArryT(word);
            List<string> array = new List<string>();

            for (int i = result.Count - 1; i >= 0; --i)
            {
                array.Add(result[i].ToString());
            }
            for (int i = array.Count - 1; i >= 0; --i)
            {

                if (array.IndexOf(array[i]) != i)
                    array.RemoveAt(i);
            }
            result = new ArrayList();
            array.Insert(0, word);
            result.Add(array[0]);
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] != word)
                    result.Add(array[i]);
            }

            return result;

        }

        /// <summary>
        /// 将字符串位数进行组合并输出数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ArrayList GetStringArryT(string word)
        {
            ArrayList result = new ArrayList();
            if (word.Length == 1)
            {
                result.Add(word);
                return result;
            }
            for (int i = 0; i < word.Length; i++)
            {
                string shorter_word = word.Substring(0, i) + word.Substring(i + 1, word.Length - i - 1);
                ArrayList shorter_Permutations = GetStringArry(shorter_word);
                for (int j = 0; j < shorter_Permutations.Count; j++)
                {
                    string longer_word = word[i].ToString() + shorter_Permutations[j].ToString();
                    result.Add(longer_word);
                }
            }
            return result;

        }

        ///str_value 字符
        ///str_len 要截取的字符长度
        public static string CutString(string str_value, int str_len)
        {
            //return str_value;
            int p_num = 0;
            int i;
            string New_Str_value = "";

            if (str_value == "")
            {
                New_Str_value = "";
            }
            else
            {
                int Len_Num = str_value.Length;

                for (i = 0; i <= Len_Num - 1; i++)
                {
                    //str_value.Substring(i,1);
                    if (i > Len_Num) break;
                    char c = Convert.ToChar(str_value.Substring(i, 1));
                    if (((int)c > 255) || ((int)c < 0))
                    {
                        p_num = p_num + 2;

                    }
                    else
                    {
                        p_num = p_num + 1;

                    }

                    if (p_num >= str_len)
                    {

                        New_Str_value = str_value.Substring(0, i + 1);

                        break;
                    }
                    else
                    {
                        New_Str_value = str_value;
                    }

                }

            }
            return New_Str_value + (p_num >= str_len ? "..." : "");
        }
        /// <summary>
        /// 将几个符号转换
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetKeysString(Keys keys)
        {
            string str = "";
            if (keys == Keys.Space)
                str = "~";
            else if (keys == Keys.Oemcomma)
                str = ",";
            else if (keys == Keys.OemPeriod)
                str = ".";
            else if (keys == Keys.OemQuestion)
                str = "/";
            else if (keys == Keys.Oem1)
                str = ";";
            else
                str = keys.ToString();
            return str;
        }

        /// <summary>
        /// 全半角输出
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string CheckKeysString(Keys keys)
        {
            string str = "";
            if (MiouIME.Is全角)
            {
                #region
                if (MiouIME.Is中文句号)
                {
                    #region 不按shift键的情况
                    if (!MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "　";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < qjzwdict.Length; i++)
                                {
                                    if (keys.ToString() == qjzwdict[i].Split('=')[0] && qjzwdict[i].Split(' ').Length > 1)
                                    {

                                        str = qjzwdict[i].Split('=')[1].Split(' ')[0];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "‘";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "’";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 按shift键的情况
                    if (MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < qjzwdict.Length; i++)
                                {
                                    if (keys.ToString() == qjzwdict[i].Split('=')[0] && qjzwdict[i].Split(' ').Length > 1)
                                    {

                                        str = qjzwdict[i].Split('=')[1].Split(' ')[1];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "“";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "”";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 不按shift键的情况
                    if (!MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "　";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < qjywdict.Length; i++)
                                {
                                    if (keys.ToString() == qjywdict[i].Split('=')[0] && qjywdict[i].Split(' ').Length > 1)
                                    {

                                        str = qjywdict[i].Split('=')[1].Split(' ')[0];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "‘";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "’";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 按shift键的情况
                    if (MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < qjywdict.Length; i++)
                                {
                                    if (keys.ToString() == qjywdict[i].Split('=')[0] && qjywdict[i].Split(' ').Length > 1)
                                    {

                                        str = qjywdict[i].Split('=')[1].Split(' ')[1];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "“";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "”";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                if (MiouIME.Is中文句号 || MiouIME.OpenSR)
                {
                    #region 不按shift键的情况
                    if (!MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < bjzwdict.Length; i++)
                                {
                                    if (keys.ToString() == bjzwdict[i].Split('=')[0] && bjzwdict[i].Split(' ').Length > 1)
                                    {

                                        str = bjzwdict[i].Split('=')[1].Split(' ')[0];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "‘";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "’";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 按shift键的情况
                    if (MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Space)
                            str = "";
                        else
                        {
                            if (keys != Keys.Oem7)
                            {
                                for (int i = 0; i < bjzwdict.Length; i++)
                                {
                                    if (keys.ToString() == bjzwdict[i].Split('=')[0] && bjzwdict[i].Split(' ').Length > 1)
                                    {
                                        str = bjzwdict[i].Split('=')[1].Split(' ')[1];
                                        break;
                                    }
                                }
                            }
                            else if (keys == Keys.Oem7)
                            {
                                if (firstdh == 1)
                                {
                                    str = "“";
                                    firstdh++;
                                }
                                else
                                {
                                    str = "”";
                                    firstdh = 1;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    if (!MiouIME.IsPressShift)
                    {
                        if (keys == Keys.Oemcomma)
                            str = "<";
                        else if (keys == Keys.OemPeriod)
                            str = ">";
                        else if (keys == Keys.OemQuestion)
                            str = "?";
                        else if (keys == Keys.Oem1)
                            str = ":";
                        else if (keys == Keys.Oem7)
                            str = "\"";
                    }
                    else
                    {
                        if (keys == Keys.Oemcomma)
                            str = ",";
                        else if (keys == Keys.OemPeriod)
                            str = ".";
                        else if (keys == Keys.OemQuestion)
                            str = "/";
                        else if (keys == Keys.Oem1)
                            str = ";";
                        else if (keys == Keys.Oem7)
                            str = "'";
                    }
                }

            }
            return str;
        }
        static string mapstr1 = "~1qaz2wsx3edc4rfv5tgb";
        static string mapstr2 = "6yhn7ujm8ik,9ol.0p;/-['=]";
        private static bool SRLeft(string s)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (mapstr1.IndexOf(s.Substring(i, 1)) >= 0) count++;
            }

            return count == s.Length;
        }
        private static bool SRRight(string s)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (mapstr2.IndexOf(s.Substring(i, 1)) >= 0) count++;
            }

            return count == s.Length;
        }
        public static string CovertStr(string v, bool px = true)
        {
            bool hleft = false;
            bool hright = false;
            string oldv = v;
            v = v.Replace("；", ";").Replace("，", ",").Replace("。", ".").Replace("、", "/").Replace("‘", "'").Replace("’", "'");
            v = v.Replace("，", ",").Replace("．", ".");
            if (v.Length == 1 && !CheckSRCode(v) && px) return oldv;
            string s = string.Empty;
            if (px)
            {
                mapsortkeys.FindAll(f => v.IndexOf(f.ZM) >= 0).OrderBy(o => o.Pos).ToList().ForEach(f => s += f.ZM);
                v = s;
                s = string.Empty;
            }
            bool havezh = false;

        lbg:
            bool have = false;
            foreach (var m in mapkeys)
            {
                if (v.StartsWith(m.ZM))
                {
                    s += m.Map;
                    v = v.Replace(m.ZM, string.Empty);
                    have = true;
                    if (m.ZM.Length > 1 && string.Compare(m.Map, m.ZM, true) != 0)
                        havezh = true;
                    if (mapstr1.IndexOf(m.ZM) >= 0) hleft = true;
                    if (mapstr2.IndexOf(m.ZM) >= 0) hright = true;

                    break;
                }
                if (v.Length == 0) break;
            }
            if (v.Length > 0 && have) goto lbg;
            if (px && !havezh && !(hleft && hright))
            {
                return CovertStr(oldv, false);
            }
            return s + v;
        }

        public static void CheckLetRight(string v, out bool hleft, out bool hright)
        {
            hleft = false;
            hright = false;
            string oldv = v;
            v = v.Replace("；", ";").Replace("，", ",").Replace("。", ".").Replace("、", "/").Replace("‘", "'").Replace("’", "'");
            if (v.Length == 1 && !CheckSRCode(v)) return;
            string s = string.Empty;

            mapsortkeys.FindAll(f => v.IndexOf(f.ZM) >= 0).OrderBy(o => o.Pos).ToList().ForEach(f => s += f.ZM);
            v = s;
            s = string.Empty;


        lbg:
            bool have = false;
            foreach (var m in mapkeys)
            {
                if (v.StartsWith(m.ZM))
                {
                    s += m.Map;
                    v = v.Replace(m.ZM, string.Empty);
                    have = true;
                    hleft = SRLeft(m.ZM);
                    hright = SRRight(m.ZM);
                    break;
                }
                if (v.Length == 0) break;
            }
            if (v.Length > 0 && have) goto lbg;


        }

        /// <summary>
        /// 排序一下
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string SRSort(string v)
        {

            string s = string.Empty;

            mapsortkeys.FindAll(f => v.IndexOf(f.ZM) >= 0).OrderBy(o => o.Pos).ToList().ForEach(f => s += f.ZM);
            v = s;

            return v;
        }

        public static string CovertNoStr(string v)
        {
            return v.Replace("；", ";").Replace("，", ",").Replace("。", ".").Replace("、", "/").Replace("‘", "'").Replace("’", "'");
        }
        /// <summary>
        /// 根据汉字查编码和拼音
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string QueryCodeByValue(string strc)
        {
            string vstr = "";
            string str = strc;
            string[] ar = null;
            int count = 0;
            for (int v = 0; v < strc.Length; v++)
            {
                str = strc.Substring(v, 1);
                for (int i = 0; i < mddtary.Length; i++)
                {
                    ar = mddtary[i].Split(' ');
                    for (int j = 0; j < ar.Length; j++)
                    {
                        if (ar[j] == str)
                        {
                            count++;
                            if (count == 1) vstr += str + ":";
                            vstr += ar[0] + " ";
                            if (count > 5)
                                goto step1;
                        }
                    }
                }
            step1:
                try
                {
                    ChineseChar chineseChar = new ChineseChar(Convert.ToChar(str));

                    ReadOnlyCollection<string> pinyin = chineseChar.Pinyins;

                    string[] aa = getpys(pinyin).Split(',');

                    for (int i = 0; i < aa.Length; i++)
                    {
                        if (aa[i].Length > 0 && aa[i].IndexOf("5") < 0)
                        {
                            if (i == 0) vstr += "拼:";

                            vstr += aa[i].ToLower() + " ";
                        }

                    }
                }
                catch
                {

                }
                if (vstr.Length > 0)
                    vstr += " \r\n";
                count = 0;
            }
            return vstr;
        }

        /// <summary>
        /// 根据汉字判断是否有编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HaveCodeByValue(string strc)
        {
            string[] ar = null;
            for (int i = 0; i < mddtary.Length; i++)
            {
                ar = mddtary[i].Split(' ');
                for (int j = 0; j < ar.Length; j++)
                {
                    if (ar[j] == strc)
                    {
                        return "true";
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 根据汉字获取主码的编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetCodeByValue(string strc)
        {
            string vstr = "";

            string[] ar = null;

            for (int i = 0; i < mddtary.Length; i++)
            {
                ar = mddtary[i].Split(' ');
                for (int j = 0; j < ar.Length; j++)
                {
                    if (ar[j] == strc)
                    {
                        if (vstr == "") vstr += strc;
                        vstr += " " + ar[0];
                        break;
                    }
                }

            }
            if (vstr.Length > 0)
                vstr += " \r\n";
            return vstr;
        }

        #endregion

        #region 皮肤相关函数
        public static string 中()
        {
            return 皮肤目录 + "\\zw." + SkinExName;
        }
        public static string 英()
        {
            return 皮肤目录 + "\\yw." + SkinExName;
        }
        public static string 全角()
        {
            return 皮肤目录 + "\\qj." + SkinExName;
        }
        public static string 半角()
        {
            return 皮肤目录 + "\\bj." + SkinExName;
        }
        public static string 句号()
        {
            return 皮肤目录 + "\\jh." + SkinExName;
        }
        public static string 逗号()
        {
            return 皮肤目录 + "\\dh." + SkinExName;
        }
        public static string 繁体()
        {
            return 皮肤目录 + "\\ft." + SkinExName;
        }
        public static string 简体()
        {
            return 皮肤目录 + "\\jt." + SkinExName;
        }
        public static string 软键盘()
        {
            return 皮肤目录 + "\\jpa." + SkinExName;
        }

        public static string 编码输出框背景图片()
        {
            return 皮肤目录 + "\\srbj." + SkinExName;
        }
        public static string 编码输出框背景图片H()
        {
            return 皮肤目录 + "\\srbjh." + SkinExName;
        }
        public static string 状态栏背景框()
        {
            return 皮肤目录 + "\\bjt." + SkinExName;
        }
        public static string 状态栏Log()
        {
            return 皮肤目录 + "\\bjtlog." + SkinExName;
        }
        public static int 编码输出框高()
        {
            return SkinHeith;
        }
        public static int 编码输出框宽()
        {
            return SkinWidth;
        }
        public static int 状态栏高()
        {
            return SkinStateHeith;
        }
        public static int 状态栏宽()
        {
            return SkinStateWidth;
        }
        public static string 输入法名()
        {
            if (!SkinStateStringView) return string.Empty;
            string inputname = mbiniobj.IniReadValue("词库设置", "词库名");
            if (输入模式 == 1)
            {
                inputname = inputname.Substring(0, 2) + "拼音";
                InputFrm.shangpLen = 100;
                InputFrm.wcmshangpLen = 100;
            }
            else if (输入模式 == 2)
            {
                inputname = "拼音输入";
                InputFrm.shangpLen = 100;
                InputFrm.wcmshangpLen = 100;
            }
            else
            {
                InputFrm.shangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长"));
                InputFrm.wcmshangpLen = Convert.ToInt32(Core.InputHelp.mbiniobj.IniReadValue("词库设置", "无重码上屏码长"));
            }
            if (!MiouIME.Is简体)
                inputname = Microsoft.VisualBasic.Strings.StrConv(inputname, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);

            return inputname;
        }
        #endregion

        /// <summary>
        /// 是否为特殊键
        /// </summary>
        /// <returns></returns>
        public static bool CheckYJSP(string code)
        {
            if (特殊键码 == "") return false;

            if (特殊键码.IndexOf(code) >= 0)
                return true;
            return false;
        }

        public static bool AutoShangPing()
        {

            if (是否自动上屏 == "0")
                return false;
            else
                return true;

        }

        #region 快捷键获取
        public static string Get激活关闭输入快捷键()
        {
            return iniobj.IniReadValue("快捷键", "激活关闭输入快捷键");
        }

        public static string Get中英文切换快捷键()
        {
            return iniobj.IniReadValue("快捷键", "中英文切换快捷键");
        }

        public static string Get简体繁体切换()
        {
            return iniobj.IniReadValue("快捷键", "简体繁体切换");
        }
        public static string Get中英文标点切换()
        {
            return iniobj.IniReadValue("快捷键", "中英文标点切换");
        }
        public static string Get全角半角切换()
        {
            return iniobj.IniReadValue("快捷键", "全角半角切换");
        }
        public static string Get第二重码选择键()
        {
            return iniobj.IniReadValue("快捷键", "第二重码选择键");
        }
        public static string Get第三重码选择键()
        {
            return iniobj.IniReadValue("快捷键", "第三重码选择键");
        }
        public static string Get临拼切换键()
        {
            return iniobj.IniReadValue("快捷键", "临拼切换键");
        }
        #endregion

        private static string getpys(ReadOnlyCollection<string> pinyin)
        {
            string pys = "";

            foreach (string pin in pinyin)
            {
                if (pin != null)
                {
                    pys += pin + ",";
                }
            }
            return pys;
        }

        #region 设置自动启动
        /// <summary> 
        /// 开机启动项 
        /// </summary> 
        /// <param name="Started">是否启动</param> 
        public static void RunWhenStart(bool Started)
        {
            try
            {
                RegistryKey HKLM = Registry.LocalMachine;
                RegistryKey Run = HKLM.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (Started == true)
                {

                    Run.SetValue("完美输入平台", Application.ExecutablePath);
                    HKLM.Close();

                }
                else
                {

                    Run.DeleteValue("完美输入平台");
                    HKLM.Close();

                }
            }
            catch
            {

            }
        }



        #endregion

        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="fname">文件路径</param>
        /// <returns></returns>
        public static System.Text.Encoding GetFileType(string fname)
        {
            string codetype = mbiniobj.IniReadValue("词库设置", "codetype");
            if (string.IsNullOrEmpty(codetype) || string.Compare(codetype, "unicode", true) == 0)
                return System.Text.Encoding.Unicode;
            else if (string.Compare(codetype, "utf-8", true) == 0)
                return System.Text.Encoding.UTF8;
            else
                return System.Text.Encoding.Default;
        }
    }

    //声明键盘钩子的封送结构类型 
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        public int vkCode;   //表示一个在1到254间的虚似键盘码 
        public int scanCode;   //表示硬件扫描码 
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    public enum KeysHashCode
    {
        退格 = 524296,
        回车 = 851981,
        空格 = 2097184,
    }

    public class KeysQueue
    {
        private Keys _keydata = new Keys();
        private DateTime _datatime = DateTime.Now;
        private KeyboardHookStruct _myKeyboardHookStruct = new KeyboardHookStruct();

        public Keys KeyData
        {
            get { return this._keydata; }
            set { this._keydata = value; }
        }

        public DateTime DataTime
        {
            get { return this._datatime; }
            set { this._datatime = value; }
        }

        public KeyboardHookStruct MyKeyboardHookStruct
        {
            get { return this._myKeyboardHookStruct; }
            set { this._myKeyboardHookStruct = value; }
        }


    }

    public class PostIndex
    {
        public PostIndex()
        {
            this.last = 0;
            this.first = 0;
            this.letter = "";
        }
        public string letter = "";
        public int first = 0;
        public int last = -1;
    }


    public class DataClass
    {
        public static DataTable ControlKeyData = new DataTable();
        public static DataTable ControlKeyMoreData = new DataTable();
        public static DataTable LetterData = new DataTable();
        public static DataTable OnlyLetterData = new DataTable();

        static DataClass()
        {
            ControlKeyData.Columns.Add("fcode");
            ControlKeyData.Columns.Add("fvalue");
            ControlKeyMoreData.Columns.Add("fcode");
            ControlKeyMoreData.Columns.Add("fvalue");
            LetterData.Columns.Add("fcode");
            LetterData.Columns.Add("fvalue");
            OnlyLetterData.Columns.Add("fcode");
            OnlyLetterData.Columns.Add("fvalue");

            ControlKeyData.Rows.Add(new object[] { "左Shift", "LShiftKey" });
            ControlKeyData.Rows.Add(new object[] { "右Shift", "RShiftKey" });
            ControlKeyData.Rows.Add(new object[] { "左Ctrl", "LControlKey" });
            ControlKeyData.Rows.Add(new object[] { "右Ctrl", "RControlKey" });
            ControlKeyData.Rows.Add(new object[] { "左Alt", "LMenu" });
            ControlKeyData.Rows.Add(new object[] { "右Alt", "RMenu" });


            ControlKeyMoreData.Rows.Add(new object[] { "左Shift", "LShiftKey" });
            ControlKeyMoreData.Rows.Add(new object[] { "右Shift", "RShiftKey" });
            ControlKeyMoreData.Rows.Add(new object[] { "左Ctrl", "LControlKey" });
            ControlKeyMoreData.Rows.Add(new object[] { "右Ctrl", "RControlKey" });
            ControlKeyMoreData.Rows.Add(new object[] { "左Alt", "LMenu" });
            ControlKeyMoreData.Rows.Add(new object[] { "右Alt", "RMenu" });
            ControlKeyMoreData.Rows.Add(new object[] { ";", "Oem1" });
            ControlKeyMoreData.Rows.Add(new object[] { "'", "Oem7" });
            ControlKeyMoreData.Rows.Add(new object[] { "\\", "Oem5" });
            ControlKeyMoreData.Rows.Add(new object[] { ",", "Oemcomma" });
            ControlKeyMoreData.Rows.Add(new object[] { ".", "OemPeriod" });
            ControlKeyMoreData.Rows.Add(new object[] { "/", "OemQuestion" });
            ControlKeyMoreData.Rows.Add(new object[] { "空格", "Space" });
            ControlKeyMoreData.Rows.Add(new object[] { "a", "A" });
            ControlKeyMoreData.Rows.Add(new object[] { "b", "B" });
            ControlKeyMoreData.Rows.Add(new object[] { "c", "C" });
            ControlKeyMoreData.Rows.Add(new object[] { "d", "D" });
            ControlKeyMoreData.Rows.Add(new object[] { "e", "E" });
            ControlKeyMoreData.Rows.Add(new object[] { "f", "F" });
            ControlKeyMoreData.Rows.Add(new object[] { "g", "G" });
            ControlKeyMoreData.Rows.Add(new object[] { "h", "H" });
            ControlKeyMoreData.Rows.Add(new object[] { "i", "I" });
            ControlKeyMoreData.Rows.Add(new object[] { "j", "J" });
            ControlKeyMoreData.Rows.Add(new object[] { "k", "K" });
            ControlKeyMoreData.Rows.Add(new object[] { "l", "L" });
            ControlKeyMoreData.Rows.Add(new object[] { "m", "M" });
            ControlKeyMoreData.Rows.Add(new object[] { "n", "N" });
            ControlKeyMoreData.Rows.Add(new object[] { "o", "O" });
            ControlKeyMoreData.Rows.Add(new object[] { "p", "P" });
            ControlKeyMoreData.Rows.Add(new object[] { "q", "Q" });
            ControlKeyMoreData.Rows.Add(new object[] { "r", "R" });
            ControlKeyMoreData.Rows.Add(new object[] { "s", "S" });
            ControlKeyMoreData.Rows.Add(new object[] { "t", "T" });
            ControlKeyMoreData.Rows.Add(new object[] { "u", "U" });
            ControlKeyMoreData.Rows.Add(new object[] { "v", "V" });
            ControlKeyMoreData.Rows.Add(new object[] { "w", "W" });
            ControlKeyMoreData.Rows.Add(new object[] { "x", "X" });
            ControlKeyMoreData.Rows.Add(new object[] { "y", "Y" });
            ControlKeyMoreData.Rows.Add(new object[] { "z", "Z" });


            LetterData.Rows.Add(new object[] { "a", "A" });
            LetterData.Rows.Add(new object[] { "b", "B" });
            LetterData.Rows.Add(new object[] { "c", "C" });
            LetterData.Rows.Add(new object[] { "d", "D" });
            LetterData.Rows.Add(new object[] { "e", "E" });
            LetterData.Rows.Add(new object[] { "f", "F" });
            LetterData.Rows.Add(new object[] { "g", "G" });
            LetterData.Rows.Add(new object[] { "h", "H" });
            LetterData.Rows.Add(new object[] { "i", "I" });
            LetterData.Rows.Add(new object[] { "j", "J" });
            LetterData.Rows.Add(new object[] { "k", "K" });
            LetterData.Rows.Add(new object[] { "l", "L" });
            LetterData.Rows.Add(new object[] { "m", "M" });
            LetterData.Rows.Add(new object[] { "n", "N" });
            LetterData.Rows.Add(new object[] { "o", "O" });
            LetterData.Rows.Add(new object[] { "p", "P" });
            LetterData.Rows.Add(new object[] { "q", "Q" });
            LetterData.Rows.Add(new object[] { "r", "R" });
            LetterData.Rows.Add(new object[] { "s", "S" });
            LetterData.Rows.Add(new object[] { "t", "T" });
            LetterData.Rows.Add(new object[] { "u", "U" });
            LetterData.Rows.Add(new object[] { "v", "V" });
            LetterData.Rows.Add(new object[] { "w", "W" });
            LetterData.Rows.Add(new object[] { "x", "X" });
            LetterData.Rows.Add(new object[] { "y", "Y" });
            LetterData.Rows.Add(new object[] { "z", "Z" });
            //LetterData.Rows.Add(new object[] { ";", "Oem1" });
            //LetterData.Rows.Add(new object[] { "'", "Oem7" });
            //LetterData.Rows.Add(new object[] { "\\", "Oem5" });
            //LetterData.Rows.Add(new object[] { ",", "Oemcomma" });
            //LetterData.Rows.Add(new object[] { ".", "OemPeriod" });
            //LetterData.Rows.Add(new object[] { "/", "OemQuestion" });
            LetterData.Rows.Add(new object[] { "空格", "Space" });
            LetterData.Rows.Add(new object[] { "", "" });


            OnlyLetterData.Rows.Add(new object[] { "a", "A" });
            OnlyLetterData.Rows.Add(new object[] { "b", "B" });
            OnlyLetterData.Rows.Add(new object[] { "c", "C" });
            OnlyLetterData.Rows.Add(new object[] { "d", "D" });
            OnlyLetterData.Rows.Add(new object[] { "e", "E" });
            OnlyLetterData.Rows.Add(new object[] { "f", "F" });
            OnlyLetterData.Rows.Add(new object[] { "g", "G" });
            OnlyLetterData.Rows.Add(new object[] { "h", "H" });
            OnlyLetterData.Rows.Add(new object[] { "i", "I" });
            OnlyLetterData.Rows.Add(new object[] { "j", "J" });
            OnlyLetterData.Rows.Add(new object[] { "k", "K" });
            OnlyLetterData.Rows.Add(new object[] { "l", "L" });
            OnlyLetterData.Rows.Add(new object[] { "m", "M" });
            OnlyLetterData.Rows.Add(new object[] { "n", "N" });
            OnlyLetterData.Rows.Add(new object[] { "o", "O" });
            OnlyLetterData.Rows.Add(new object[] { "p", "P" });
            OnlyLetterData.Rows.Add(new object[] { "q", "Q" });
            OnlyLetterData.Rows.Add(new object[] { "r", "R" });
            OnlyLetterData.Rows.Add(new object[] { "s", "S" });
            OnlyLetterData.Rows.Add(new object[] { "t", "T" });
            OnlyLetterData.Rows.Add(new object[] { "u", "U" });
            OnlyLetterData.Rows.Add(new object[] { "v", "V" });
            OnlyLetterData.Rows.Add(new object[] { "w", "W" });
            OnlyLetterData.Rows.Add(new object[] { "x", "X" });
            OnlyLetterData.Rows.Add(new object[] { "y", "Y" });
            OnlyLetterData.Rows.Add(new object[] { "z", "Z" });
            OnlyLetterData.Rows.Add(new object[] { "空格", "~" });
        }

        
    }
    public class MapintKey
    {
        public string ZM = string.Empty;
        public string Map = string.Empty;
        public short Pos = 0;
    }

    /// <summary>
    /// 大型词库对象
    /// </summary>
    [Serializable]
    public class BigDict
    {
        public BigDict()
        {
        }
        public string Head = string.Empty;
        public List<string> HeadCodeS = new List<string>();
        public List<string> Dicts = new List<string>();
    }
}
