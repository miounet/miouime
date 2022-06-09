using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace MiouIME
{
    public partial class GetWordFrm : Form
    {
        public GetWordFrm()
        {
            InitializeComponent();
        }

        private void butexproct_Click(object sender, EventArgs e)
        {
            if (Core.InputHelp.openJM)
            {
                MessageBox.Show("因词库作者要求,已对词库加密保护!");
                return;
            }
            if (this.cmblen.SelectedItem == null)
            {
                MessageBox.Show("请选择字词长度!");
                return;
            }
            if (this.cmbinfo.SelectedItem == null)
            {
                MessageBox.Show("请选择要导出的字词信息!");
                return;
            }
            this.saveFileDialog1.Filter = "文本文件(*.txt)|*.txt";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.SaveWord));
                th.Start();
            }
        }

        private void SaveWord()
        {
            #region
            if (this.cmblen.SelectedItem.ToString() == "全部词库")
            {
                this.progressBar1.Maximum = 1;
                this.progressBar1.Value = 0;
                File.WriteAllLines(this.saveFileDialog1.FileName, Core.InputHelp.mddtary, Encoding.Unicode);
                this.progressBar1.Value = 1;
            }
            else
            {
                this.progressBar1.Maximum = Core.InputHelp.mddtary.Length;
                this.progressBar1.Value = 0;
                string valuestr = "";
                for (int i = 0; i < Core.InputHelp.mddtary.Length; i++)
                {
                    #region
                    string strarr = Core.InputHelp.mddtary[i];
                    string fcode = strarr.Split(' ')[0];
                    if (this.cmbtype.SelectedItem.ToString() == "一简" && fcode.Length != 1)
                    {
                        continue;
                    }
                    else if (this.cmbtype.SelectedItem.ToString() == "二简" && fcode.Length != 2)
                    {
                        continue;
                    }
                    else if (this.cmbtype.SelectedItem.ToString() == "三简" && fcode.Length != 3)
                    {
                        continue;
                    }
                    else if (this.cmbtype.SelectedItem.ToString() == "四简" && fcode.Length != 4)
                    {
                        continue;
                    }
                    string[] fvalue = strarr.Substring(strarr.Split(' ')[0].Length).Trim().Split(' ');
                    for (int j = 0; j < fvalue.Length; j++)
                    {
                        if (this.cmblen.SelectedItem.ToString() == "单字")
                        {
                            if (fvalue[j].Trim().Length == 1 && Core.InputHelp.CheckChinese(fvalue[j].Trim()) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "两字词")
                        {
                            if (fvalue[j].Trim().Length == 2 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "三字词")
                        {
                            if (fvalue[j].Trim().Length == 3 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "四字词")
                        {
                            if (fvalue[j].Trim().Length == 4 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "五字词")
                        {
                            if (fvalue[j].Trim().Length == 5 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "六字词")
                        {
                            if (fvalue[j].Trim().Length == 6 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "六字以上词")
                        {
                            if (fvalue[j].Trim().Length > 6 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }
                        else if (this.cmblen.SelectedItem.ToString() == "全部词组")
                        {
                            if (fvalue[j].Trim().Length > 1 && Core.InputHelp.CheckChinese(fvalue[j].Trim().Substring(0, 1)) == true)
                            {
                                valuestr += (this.cmbinfo.SelectedItem.ToString() == "汉字和编码" ? fcode + " " : "") + fvalue[j].Trim() + "\n";
                            }
                        }

                    }
                    #endregion

                    this.progressBar1.Value++;
                }

                File.WriteAllLines(this.saveFileDialog1.FileName, valuestr.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries), Encoding.Unicode);
                this.progressBar1.Value = this.progressBar1.Maximum;
            }
            #endregion
        }
    }
}
