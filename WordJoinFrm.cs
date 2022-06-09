using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiouIME
{
    public partial class WordJoinFrm : Form
    {
        public WordJoinFrm()
        {
            InitializeComponent();
        }

        private void butStart_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Length <= 0)
            {
                MessageBox.Show("无数据");
                this.textBox1.Focus();
                return;
            }

            this.progressBar1.Maximum = this.textBox1.Text.Trim().Length;
            this.progressBar1.Value = 0;

            for (int i = 0; i < this.progressBar1.Maximum; i++)
            {
                if (this.textBox2.Text.IndexOf(this.textBox1.Text.Substring(i, 1)) < 0)
                    this.textBox2.AppendText(this.textBox1.Text.Substring(i, 1));

                this.progressBar1.Value++;
            }

            this.progressBar1.Value = this.progressBar1.Maximum;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.label3.Text = "字数:" + this.textBox1.Text.Trim().Length ;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.label4.Text = "字数:" + this.textBox2.Text.Trim().Length;
        }

        private void butreplace_Click(object sender, EventArgs e)
        {

            if (this.textBox1.Text.Trim().Length <= 0)
            {
                MessageBox.Show("无数据");
                this.textBox1.Focus();
                return;
            }

            this.progressBar1.Maximum = this.textBox1.Text.Trim().Length;
            this.progressBar1.Value = 0;

            for (int i = 0; i < this.progressBar1.Maximum; i++)
            {

                this.textBox2.Text = this.textBox2.Text.Replace(this.textBox1.Text.Substring(i, 1), "");

                this.progressBar1.Value++;
            }

            this.progressBar1.Value = this.progressBar1.Maximum;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] arr = new string[this.textBox2.Text.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.textBox2.Text.Substring(i, 1);
            }
            WordCodeFrm wcfrm = new WordCodeFrm();
            wcfrm.Show();
            wcfrm.Arr = arr;
        }
    }
}
