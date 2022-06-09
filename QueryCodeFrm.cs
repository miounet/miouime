using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiouIME
{
    public partial class QueryCodeFrm : Form
    {
        public QueryCodeFrm()
        {
            InitializeComponent();
        }

        public string QueryValue
        {
            set { this.textBox2.Text = value; }
            get { return this.textBox2.Text; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim().Length > 0)
            {
                string vstr = Core.InputHelp.QueryCodeByValue(textBox2.Text.Trim());
                if (vstr.Length > 0)
                {
                    textBox1.Text = vstr;
                }
            }
        }
    }
}
