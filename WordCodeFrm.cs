using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiouIME
{
    public partial class WordCodeFrm : Form
    {
        string[] arr = null;
        public WordCodeFrm()
        {
            InitializeComponent();
        }

        private void WordCodeFrm_Load(object sender, EventArgs e)
        {

        }

        public string[] Arr
        {
            set
            {
                this.arr = value;
                FindDataTheard();
                //this.FindData();
            }
        }

        private void FindData()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.progressBar1.Maximum = arr.Length;
            this.progressBar1.Value = 0;
            this.textBox1.Text = "";
            for (int i = 0; i < arr.Length; i++)
            {
                this.textBox1.AppendText(Core.InputHelp.GetCodeByValue(arr[i]));
                this.progressBar1.Value++;
                System.Threading.Thread.Sleep(5);
            }
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
        }
        private void FindDataTheard()
        {
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.FindData));
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindDataTheard();
            //FindData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox2.Text.Trim().Length <= 0)
            {
                MessageBox.Show("没有过滤条件!");
                this.textBox2.Focus();
                return;
            }
            this.glt2();
            
        }
        private void gl2()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            string[] txts = this.textBox1.Lines;
            this.progressBar1.Maximum = txts.Length;
            this.progressBar1.Value = 0;
            string[] ss = null;
            this.textBox1.Text = "";
            for (int i = 0; i < txts.Length; i++)
            {
                System.Threading.Thread.Sleep(5);
                ss = txts[i].Split(' ');
                for (int j = 1; j < ss.Length; j++)
                {
                    if (ss[j].Trim().StartsWith(this.textBox2.Text.Trim()))
                        ss[j] = "";
                }
                for (int c = 0; c < ss.Length; c++)
                {
                    this.textBox1.Text += ss[c] + " ";
                }
                this.textBox1.Text += "\r\n";
                this.progressBar1.Value++;
            }

            this.textBox1.Text = this.textBox1.Text.Replace("    ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("   ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("  ", " ");
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
        }
        private void glt2()
        {
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.gl2));
            th.Start();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox3.Text.Trim().Length <= 0)
            {
                MessageBox.Show("没有过滤条件!");
                this.textBox3.Focus();
                return;
            }
            this.glt3();
        }

        private void gl3()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            string[] txts = this.textBox1.Lines;
            this.progressBar1.Maximum = txts.Length;
            this.progressBar1.Value = 0;
            string[] ss = null;
            this.textBox1.Text = "";
            for (int i = 0; i < txts.Length; i++)
            {
                System.Threading.Thread.Sleep(5);
                ss = txts[i].Split(' ');
                for (int j = 1; j < ss.Length; j++)
                {
                    if (ss[j].Trim().Length == Convert.ToInt32(this.textBox3.Text.Trim()))
                        ss[j] = "";
                }
                for (int c = 0; c < ss.Length; c++)
                {
                    this.textBox1.Text += ss[c] + " ";
                }
                this.textBox1.Text += "\r\n";
                this.progressBar1.Value++;
            }
            this.textBox1.Text = this.textBox1.Text.Replace("    ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("   ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("  ", " ");
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
        }
        private void glt3()
        {
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.gl3));
            th.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.glt4();
        }

        private void gl4()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            string[] txts = this.textBox1.Lines;
            this.progressBar1.Maximum = txts.Length;
            this.progressBar1.Value = 0;
            string ss = "";
            this.textBox1.Text = "";
            for (int i = 0; i < txts.Length; i++)
            {
                System.Threading.Thread.Sleep(5);
                ss = txts[i].Trim();

                if (ss.Length > 1)
                {
                    this.textBox1.Text += ss+"\r\n";
                }

                this.progressBar1.Value++;
            }
            this.textBox1.Text = this.textBox1.Text.Replace("    ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("   ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("  ", " ");
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
        }
        private void glt4()
        {
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.gl4));
            th.Start();
        }


        private void blym()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            string[] txts = this.textBox1.Lines;
            this.progressBar1.Maximum = txts.Length;
            this.progressBar1.Value = 0;
            string[] ss = null;
            this.textBox1.Text = "";
            for (int i = 0; i < txts.Length; i++)
            {
                System.Threading.Thread.Sleep(5);
                ss = txts[i].Trim().Split(' ');

                if (ss.Length > 1)
                {
                    this.textBox1.Text += ss[0] + " " + ss[1] + "\r\n";
                }

                this.progressBar1.Value++;
            }
            this.textBox1.Text = this.textBox1.Text.Replace("    ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("   ", " ");
            this.textBox1.Text = this.textBox1.Text.Replace("  ", " ");
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
        }
        private void blymt()
        {
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.blym));
            th.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.blymt();
        }
    }
}
