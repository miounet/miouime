using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace MiouIME
{
    public partial class AddWordFrm : Form
    {
        public AddWordFrm()
        {
            InitializeComponent();
        }

        private void butadd_Click(object sender, EventArgs e)
        {
            if (this.txtcode.Text.Trim().Length <= 0)
            {
                MessageBox.Show("请输入编码!");
                this.txtcode.Focus();
                return;
            }
            if (this.txtText.Text.Trim().Length <= 0)
            {
                MessageBox.Show("请输入字词!");
                this.txtText.Focus();
                return;
            }
            AddWord(this.txtcode.Text.Trim(), this.txtText.Text.Trim());
            Encoding txtcode = Core.InputHelp.GetFileType(Core.InputHelp.mbPath);
            File.WriteAllLines(Core.InputHelp.mbPath, Core.InputHelp.mddtary, txtcode);
            Core.comm.EncryptMB(Core.InputHelp.mbPath);
            Core.InputHelp.UpdateMB();
            //Core.InputHelp.UpdateIndex();
            MessageBox.Show("保存成功!");
        }

        private void AddWord(string code, string word)
        {
            string[] arry = new string[Core.InputHelp.mddtary.Length + 1];
            for (int i = 0; i < Core.InputHelp.mddtary.Length; i++)
                arry[i] = Core.InputHelp.mddtary[i];

            Core.PostIndex poi = Core.InputHelp.GetPos(code.Substring(0, 1));
            if (poi != null)
            {
                for (int i = arry.Length - 1; i > poi.last; i--)
                {
                    arry[i] = arry[i - 1];
                }
                arry[poi.last+1] = code + " " + word;
                 
            }
            else
            {
                arry[arry.Length - 1] = code + " " + word;
            }
            Core.InputHelp.mddtary = arry;
        }
        private void ImportWord()
        {
            Encoding txtcode = Core.InputHelp.GetFileType(Core.InputHelp.mbPath);
            string[] arry = File.ReadAllLines(this.txtfile.Text, Encoding.Default);
            this.progressBar1.Value = 0;
            this.progressBar1.Maximum = arry.Length;
            for (int i = 0; i < arry.Length; i++)
            {
                if (arry[i].Trim().Length > 0)
                    AddWord(arry[i].Split(' ')[0], arry[i].Split(' ')[1]);
                this.progressBar1.Value++;
            }
            File.WriteAllLines(Core.InputHelp.mbPath, Core.InputHelp.mddtary, txtcode);
            Core.comm.EncryptMB(Core.InputHelp.mbPath);
            Core.InputHelp.UpdateMB();
            //Core.InputHelp.UpdateIndex();
            MessageBox.Show("保存成功!");
        }
        private void butimport_Click(object sender, EventArgs e)
        {
            if (this.txtfile.Text.Length <= 0)
            {
                MessageBox.Show("请选择要导入的词库文件");
                return;
            }
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.ImportWord));
            th.Start();
        }

        private void butopenfile_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtfile.Text = this.openFileDialog1.FileName;
            }
        }
    }
}
