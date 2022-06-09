using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
namespace MiouIME
{
    public partial class UWordCountFrm : Form
    {
        DataTable dt = new DataTable();
        public UWordCountFrm()
        {
            InitializeComponent();
            dt.Columns.Add("字词");
            dt.Columns.Add("编码");
            dt.Columns.Add("输出次数");
            dt.Columns[2].DataType = typeof(int);
            dt.Columns.Add("最后输出时间");
           
        }

        private void UWordCountFrm_Load(object sender, EventArgs e)
        {
            foreach(Core.CountObj item in Core.Counter.CounterList)
            {
                DataRow dr = dt.NewRow();
                dr["字词"] = item.Value;
                dr["编码"] = item.Code;
                dr["输出次数"] = item.Count;
                dr["最后输出时间"] = item.DT.ToString();
                dt.Rows.Add(dr);
            }
            this.dataGridView1.DataSource = dt.DefaultView;
        }

        private void butDelData_Click(object sender, EventArgs e)
        {
            lock (Core.Counter.CounterList)
                Core.Counter.CounterList.Clear();
             string[] arry=new string[1];
            arry[0]="";
            File.WriteAllLines(Core.InputHelp.appPath + "\\Counter\\Counter.txt", arry, Encoding.Unicode);
            dt.Clear();
            this.dataGridView1.DataSource = dt.DefaultView;
        }

        private void butclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
