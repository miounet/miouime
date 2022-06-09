using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiouIME
{
    public partial class AboutFrm : Form
    {
        public AboutFrm()
        {
            InitializeComponent();
        }

        private void AboutFrm_Load(object sender, EventArgs e)
        {
           Image bgimag = Image.FromFile(Core.InputHelp.appPath + "\\ico\\log32.ico");
           this.pictureBox1.BackgroundImage = bgimag;
           this.pictureBox1.Width = bgimag.Width;
           this.pictureBox1.Height = bgimag.Height;
            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://srkmm.ysepan.com/");
        }
    }
}
