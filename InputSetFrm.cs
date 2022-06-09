using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiouIME
{
    public partial class InputSetFrm : Form
    {
        public InputSetFrm()
        {
            InitializeComponent();
            this.TopMost = true;
        }
        private void InputSetFrm_Load(object sender, EventArgs e)
        {
            
            #region 控件数据源
            this.cmb激活关闭输入快捷键1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb激活关闭输入快捷键2.DataSource = Core.DataClass.LetterData.Copy();
            this.cmb中英文切换快捷键1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb中英文切换快捷键2.DataSource = Core.DataClass.LetterData.Copy();
            this.cmb第二重码选择键.DataSource = Core.DataClass.ControlKeyMoreData.Copy();
            this.cmb第三重码选择键.DataSource = Core.DataClass.ControlKeyMoreData.Copy();
            this.cmb简体繁体切换1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb简体繁体切换2.DataSource = Core.DataClass.LetterData.Copy();
            this.cmb全角半角切换1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb全角半角切换2.DataSource = Core.DataClass.LetterData.Copy();
            this.cmb中英文标点切换1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb中英文标点切换2.DataSource = Core.DataClass.LetterData.Copy();
            this.cmb临拼切换键1.DataSource = Core.DataClass.ControlKeyData.Copy();
            this.cmb临拼切换键2.DataSource = Core.DataClass.LetterData.Copy();
            #endregion

            #region 基本设置初始化


            string IsViewSta = Core.InputHelp.iniobj.IniReadValue("功能设置", "IsViewSta");
            if (IsViewSta == "0") this.chk隐藏输入法状态栏.Checked = false;
            else this.chk隐藏输入法状态栏.Checked = true;

            string OpenCodeView = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenCodeView");

            if (OpenCodeView == "0") this.chk开启编码提示.Checked = false;
            else this.chk开启编码提示.Checked = true;

            string OpenBJ = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenBJ");
            if (OpenBJ == "0") this.chk开启并击功能.Checked = false;
            else this.chk开启并击功能.Checked = true;

            string OpenBF = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenBF");
            if (OpenBF == "1") this.chk开启并放.Checked = true;
            else this.chk开启并放.Checked = false;

            string OpenSR = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenSR");
            if (OpenSR == "1") this.chkopenslmode.Checked = true;
            else this.chkopenslmode.Checked = false;

            string OpenOneSR = Core.InputHelp.iniobj.IniReadValue("功能设置", "OpenOneSR");
            if (OpenOneSR == "1") this.cheOpenOneSR.Checked = true;
            else this.cheOpenOneSR.Checked = false;

            string 候选项数 = Core.InputHelp.iniobj.IniReadValue("功能设置", "候选项数");
            this.cmb候选项数.Text = 候选项数;

            string 回车运用 = Core.InputHelp.iniobj.IniReadValue("功能设置", "回车运用");
            this.cmb回车用于.Text = 回车运用;

            this.chkouttype.Checked = Core.InputHelp.iniobj.IniReadValue("功能设置", "outtype") == "1" ? true : false;

            string 输入框显示方式 = Core.InputHelp.iniobj.IniReadValue("功能设置", "输入框显示方式");
            this.cmb输入框显示方式.Text = 输入框显示方式 == "0" ? "横排" : "竖排";

            string 单字模式 = Core.InputHelp.iniobj.IniReadValue("功能设置", "单字模式");
            if (单字模式 == "0") this.chk开启单字模式.Checked = false;
            else this.chk开启单字模式.Checked = true;


            string 自动启动 = Core.InputHelp.iniobj.IniReadValue("功能设置", "自动启动");
            if (自动启动 == "0") this.chk自动启动.Checked = false;
            else this.chk自动启动.Checked = true;

            string OpenTrac = Core.InputHelp.iniobj.IniReadValue("功能设置", "光标跟随");
            if (OpenTrac == "1") this.chkGBGS.Checked = true;
            else this.chkGBGS.Checked = false;

            string bdoneout = Core.InputHelp.iniobj.IniReadValue("功能设置", "标点一码输出");
            if (bdoneout == "1") this.chkBDOneOutput.Checked = true;
            else this.chkBDOneOutput.Checked = false;

            string sROneOut = Core.InputHelp.iniobj.IniReadValue("功能设置", "SROneOut");
            if (sROneOut == "1") this.chkSROneOut.Checked = true;
            else this.chkSROneOut.Checked = false;

            string cZDream = Core.InputHelp.iniobj.IniReadValue("功能设置", "CZDream");
            if (cZDream == "1") this.chkCZDream.Checked = true;
            else this.chkCZDream.Checked = false;
 

             string gBCode = Core.InputHelp.iniobj.IniReadValue("功能设置", "GBCode");
             if (gBCode.Trim().Length > 0)
                 this.cmbGBCode.Text = gBCode;
             else
                 this.cmbGBCode.Text = "unicode";

            #endregion

            #region 快捷键初始化
            string 激活关闭输入快捷键 = Core.InputHelp.iniobj.IniReadValue("快捷键", "激活关闭输入快捷键");
            this.cmb激活关闭输入快捷键1.SelectedValue = 激活关闭输入快捷键.Split('+')[0];
            this.cmb激活关闭输入快捷键2.SelectedValue = 激活关闭输入快捷键.Split('+')[1];
            string 中英文切换快捷键 = Core.InputHelp.iniobj.IniReadValue("快捷键", "中英文切换快捷键");
            this.cmb中英文切换快捷键1.SelectedValue = 中英文切换快捷键.Split('+')[0];
            this.cmb中英文切换快捷键2.SelectedValue = 中英文切换快捷键.Split('+')[1];
            string 简体繁体切换 = Core.InputHelp.iniobj.IniReadValue("快捷键", "简体繁体切换");
            this.cmb简体繁体切换1.SelectedValue = 简体繁体切换.Split('+')[0];
            this.cmb简体繁体切换2.SelectedValue = 简体繁体切换.Split('+')[1];
            string 全角半角切换 = Core.InputHelp.iniobj.IniReadValue("快捷键", "全角半角切换");
            this.cmb全角半角切换1.SelectedValue = 全角半角切换.Split('+')[0];
            this.cmb全角半角切换2.SelectedValue = 全角半角切换.Split('+')[1];
            string 中英文标点切换 = Core.InputHelp.iniobj.IniReadValue("快捷键", "中英文标点切换");
            this.cmb中英文标点切换1.SelectedValue = 中英文标点切换.Split('+')[0];
            this.cmb中英文标点切换2.SelectedValue = 中英文标点切换.Split('+')[1];
            string 第二重码选择键 = Core.InputHelp.iniobj.IniReadValue("快捷键", "第二重码选择键");
            this.cmb第二重码选择键.SelectedValue = 第二重码选择键;
            string 第三重码选择键 = Core.InputHelp.iniobj.IniReadValue("快捷键", "第三重码选择键");
            this.cmb第三重码选择键.SelectedValue = 第三重码选择键;
            string 临拼切换键 = Core.InputHelp.iniobj.IniReadValue("快捷键", "临拼切换键");
            this.cmb临拼切换键1.SelectedValue = 临拼切换键.Split('+')[0];
            this.cmb临拼切换键2.SelectedValue = 临拼切换键.Split('+')[1];
            #endregion

            #region 词库设置初始化
            //基本设置
            string 是否自动上屏 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "是否自动上屏");
            if (是否自动上屏 == "0") this.chk无重码自动上屏.Checked = false;
            else this.chk无重码自动上屏.Checked = true;
            string 错码上屏 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "错码上屏");
            if (错码上屏 == "0") this.chk开启错码上屏.Checked = false;
            else this.chk开启错码上屏.Checked = true;
            string 最大码长 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "最大码长");
            this.com最大码长.Text = 最大码长;
            string 无重码上屏码长 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "无重码上屏码长");
            this.cbwcmspmc.Text = 无重码上屏码长;
            string 词库名 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "词库名");
            this.txt输入法名.Text = 词库名;
            string 词库信息 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "词库信息");
            this.txt词库信息.Text = 词库信息;
            string 特殊键码 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "特殊键码");
            this.txt特殊键.Text = 特殊键码;
            string 停止键码 = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "停止键码");
            this.txt停止键.Text = 停止键码;
            this.lbver.Text = "词库版本(日期):" + Core.InputHelp.mbiniobj.IniReadValue("词库设置", "lastupdate");
            string autoupdate = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "autoupdate");
            if (autoupdate == "1") this.chkautoupdate.Checked = true;
            else this.chkautoupdate.Checked = false;

            this.lb词库数据.Text = CountCK();
            #endregion

          
            if (File.Exists(Core.InputHelp.mbPath.Replace("dict.txt", "maping.txt")))
            {
                this.txtmapkeys.Text = File.ReadAllText(Core.InputHelp.mbPath.Replace("dict.txt", "maping.txt"));
            }
            else File.WriteAllText(Core.InputHelp.mbPath.Replace("dict.txt", "maping.txt"), this.txtmapkeys.Text.Trim());


            this.txtSRLeftOne.Text = File.ReadAllText(Core.InputHelp.mbPath.Replace("dict.txt", "SRLeftOne.txt"));
            this.txtSRRightOne.Text = File.ReadAllText(Core.InputHelp.mbPath.Replace("dict.txt", "SRRightOne.txt"));

            #region 皮肤
            this.numStateHeight.Value = Core.InputHelp.SkinStateHeith;
            this.numStateWidth.Value = Core.InputHelp.SkinStateWidth;
            this.numInputNameWidth.Value = Core.InputHelp.SkinStateStringWidth;
            this.butinputnamecolor.ForeColor = Core.InputHelp.SkinStateFontColor;
            this.chkInputView.Checked = Core.InputHelp.SkinStateStringView;
            this.numSkinHeight.Value = Core.InputHelp.SkinHeith;
            this.numSkinWidth.Value = Core.InputHelp.SkinWidth;
            this.btnSkinbstring.ForeColor = Core.InputHelp.Skinbstring;
            this.btnSkinbcstring.ForeColor = Core.InputHelp.Skinbcstring;
            this.btnSkinfbcstring.ForeColor = Core.InputHelp.Skinfbcstring;
            this.btnSkinbordpen.ForeColor = Core.InputHelp.Skinbordpen;
            this.btnSkinFontName.Font = new System.Drawing.Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize);
            this.numSkinFontH.Value = Core.InputHelp.SkinFontH;
            this.numSkinFontJG.Value = Core.InputHelp.SkinFontJG;
            #endregion
        }
        private void but_close_Click(object sender, EventArgs e)
        {
            MiouIME.inputFrm.设置状态栏皮肤();
            MiouIME.inputFrm.IniLableSkin();
            Program.MIme.设置状态栏皮肤();

            this.Close();
        }

        private void but_save_Click(object sender, EventArgs e)
        {
             try
            {
                #region  基本设置
                
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "IsViewSta", (this.chk隐藏输入法状态栏.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "OpenCodeView", (this.chk开启编码提示.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "OpenBJ", (this.chk开启并击功能.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "OpenBF", (this.chk开启并放.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "OpenSR", (this.chkopenslmode.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "OpenOneSR", (this.cheOpenOneSR.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "outtype", (this.chkouttype.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "单字模式", (this.chk开启单字模式.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "自动启动", (this.chk自动启动.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "候选项数", this.cmb候选项数.Text);
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "回车运用", this.cmb回车用于.Text);
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "输入框显示方式", this.cmb输入框显示方式.Text == "横排" ? "0" : "1");
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "光标跟随", (this.chkGBGS.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "标点一码输出", (this.chkBDOneOutput.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "SROneOut", (this.chkSROneOut.Checked ? "1" : "0"));
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "CZDream", (this.chkCZDream.Checked ? "1" : "0"));
 
                Core.InputHelp.iniobj.IniWriteValue("功能设置", "GBCode", this.cmbGBCode.Text);
                 
                #endregion

                #region 快捷键
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "激活关闭输入快捷键", this.cmb激活关闭输入快捷键1.SelectedValue.ToString() + "+" + this.cmb激活关闭输入快捷键2.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "中英文切换快捷键", this.cmb中英文切换快捷键1.SelectedValue.ToString() + "+" + this.cmb中英文切换快捷键2.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "简体繁体切换", this.cmb简体繁体切换1.SelectedValue.ToString() + "+" + this.cmb简体繁体切换2.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "全角半角切换", this.cmb全角半角切换1.SelectedValue.ToString() + "+" + this.cmb全角半角切换2.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "中英文标点切换", this.cmb中英文标点切换1.SelectedValue.ToString() + "+" + this.cmb中英文标点切换2.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "第二重码选择键", this.cmb第二重码选择键.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "第三重码选择键", this.cmb第三重码选择键.SelectedValue.ToString());
                Core.InputHelp.iniobj.IniWriteValue("快捷键", "临拼切换键", this.cmb临拼切换键1.SelectedValue.ToString() + "+" + this.cmb临拼切换键2.SelectedValue.ToString());
                #endregion

                #region 当前词库
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "是否自动上屏", (this.chk无重码自动上屏.Checked ? "1" : "0"));
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "错码上屏", (this.chk开启错码上屏.Checked ? "1" : "0"));
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "最大码长", this.com最大码长.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "无重码上屏码长", this.cbwcmspmc.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "词库名", this.txt输入法名.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "词库信息", this.txt词库信息.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "特殊键码", this.txt特殊键.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "停止键码", this.txt停止键.Text);
                Core.InputHelp.mbiniobj.IniWriteValue("词库设置", "autoupdate", (this.chkautoupdate.Checked ? "1" : "0"));
                 
                #endregion

 

                #region 速录设置
                File.WriteAllText(Core.InputHelp.mbPath.Replace("dict.txt", "maping.txt"), this.txtmapkeys.Text.Trim());
                File.WriteAllText(Core.InputHelp.mbPath.Replace("dict.txt", "SRLeftOne.txt"), this.txtSRLeftOne.Text.Trim(), Encoding.UTF8);
                File.WriteAllText(Core.InputHelp.mbPath.Replace("dict.txt", "SRRightOne.txt"), this.txtSRRightOne.Text.Trim(), Encoding.UTF8);

                Core.InputHelp.srleftdict = File.ReadAllLines(Core.InputHelp.mbPath.Replace("dict.txt", "SRLeftOne.txt"), Encoding.UTF8);
                Core.InputHelp.srrightdict = File.ReadAllLines(Core.InputHelp.mbPath.Replace("dict.txt", "SRRightOne.txt"), Encoding.UTF8);

                #endregion

                MiouIME.inputFrm.保存状态栏皮肤();
                this.DialogResult= DialogResult.OK;
            }
            catch { }
        }

        private string CountCK()
        {
            
            int zcount = 0;
            int dzcount = 0;
            int czcount = 0;
            for (int i = 0; i < Core.InputHelp.mddtary.Length; i++)
            {
                if (Core.InputHelp.mddtary[i].Trim().Length <= 0) continue;
                string strarr = Core.InputHelp.mddtary[i];
                string fcode = strarr.ToLower().Split(' ')[0];
                if(Core.InputHelp.特殊键码.Length>0 && Core.InputHelp.特殊键码.IndexOf(fcode.Substring(0, 1)) >= 0) continue;
                string[] fvalue = strarr.Replace(strarr.Split(' ')[0], "").Trim().Split(' ');
                for (int j = 0; j < fvalue.Length; j++)
                {

                    if ((fvalue[j].Trim().Length == 1 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1))))
                    {
                        zcount++;
                        dzcount++;
                    }
                    else if (fvalue[j].Trim().Length > 1 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)))
                    {
                        zcount++;
                        czcount++;
                    }

                }
            }

            return  string.Format("词库总量:{0} 单字:{1} 词组:{2}",zcount,dzcount,czcount);
        }

 
        private void numStateWidth_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinStateWidth = int.Parse(this.numStateWidth.Value.ToString());
            Program.MIme.设置状态栏皮肤();
        }

        private void numStateHeight_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinStateHeith = int.Parse(this.numStateHeight.Value.ToString());
            Program.MIme.设置状态栏皮肤();
        }

        private void numInputNameWidth_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinStateStringWidth = int.Parse(this.numInputNameWidth.Value.ToString());
            Program.MIme.设置状态栏皮肤();
        }

        private void butinputnamecolor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.butinputnamecolor.ForeColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.butinputnamecolor.ForeColor = this.colorDialog1.Color;
                Core.InputHelp.SkinStateFontColor = this.butinputnamecolor.ForeColor;
                Program.MIme.设置状态栏皮肤();
            }
        }

        private void chkInputView_CheckedChanged(object sender, EventArgs e)
        {

            Core.InputHelp.SkinStateStringView = this.chkInputView.Checked;
            Program.MIme.设置状态栏皮肤();
        }

        private void numSkinWidth_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinWidth = int.Parse(this.numSkinWidth.Value.ToString());
            MiouIME.inputFrm.UpdateSkin();
        }

        private void numSkinHeight_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinHeith = int.Parse(this.numSkinHeight.Value.ToString());
            MiouIME.inputFrm.UpdateSkin();
        }

        private void btnSkinbstring_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.btnSkinbstring.ForeColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.btnSkinbstring.ForeColor = this.colorDialog1.Color;
                Core.InputHelp.Skinbstring = this.btnSkinbstring.ForeColor;
                Program.MIme.设置状态栏皮肤();
            }
             
        }

        private void btnSkinbcstring_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.btnSkinbcstring.ForeColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.btnSkinbcstring.ForeColor = this.colorDialog1.Color;
                Core.InputHelp.Skinbcstring = this.btnSkinbcstring.ForeColor;
                Program.MIme.设置状态栏皮肤();
            }
        }

        private void btnSkinfbcstring_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.btnSkinfbcstring.ForeColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.btnSkinfbcstring.ForeColor = this.colorDialog1.Color;
                Core.InputHelp.Skinfbcstring = this.btnSkinfbcstring.ForeColor;
                Program.MIme.设置状态栏皮肤();
            }
        }

        private void btnSkinbordpen_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.btnSkinbordpen.ForeColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.btnSkinbordpen.ForeColor = this.colorDialog1.Color;
                Core.InputHelp.Skinbordpen = this.btnSkinbordpen.ForeColor;
                Program.MIme.设置状态栏皮肤();
            }
        }

        private void btnSkinFontName_Click(object sender, EventArgs e)
        {
            this.fontDialog1.Font = new Font(Core.InputHelp.SkinFontName, Core.InputHelp.SkinFontSize);
            if (this.fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.btnSkinFontName.Font = this.fontDialog1.Font;
                Core.InputHelp.SkinFontName = this.btnSkinFontName.Font.Name;
                Core.InputHelp.SkinFontSize = (int)this.btnSkinFontName.Font.Size;
                MiouIME.inputFrm.UpdateSkin();
            }
        }

        private void numSkinFontSize_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinFontH = int.Parse(this.numSkinFontH.Value.ToString());
            MiouIME.inputFrm.UpdateSkin();
        }

        private void numSkinFontJG_ValueChanged(object sender, EventArgs e)
        {
            Core.InputHelp.SkinFontJG = int.Parse(this.numSkinFontJG.Value.ToString());
            MiouIME.inputFrm.UpdateSkin();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Core.InputHelp.appPath + "\\SR\\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtmapkeys.Text = File.ReadAllText(openFileDialog1.FileName);
            }
        }
    }
}
