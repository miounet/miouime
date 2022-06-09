using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;
namespace MiouIME
{
    public partial class SWordCountfrm : Form
    {
        List<Core.CountObj> CounterList = new List<Core.CountObj>();
        DataTable dt = new DataTable();
        public SWordCountfrm()
        {
            InitializeComponent();
            dt.Columns.Add("字词");
            dt.Columns.Add("输出次数");
            dt.Columns[1].DataType = typeof(int);
        }

        public  Core.CountObj GetCounterByValue(string value, string code, DateTime dt)
        {
            for (int i = 0; i < CounterList.Count; i++)
            {
                if (CounterList[i].Value == value)
                    return CounterList[i];
            }
            Core.CountObj obj = new Core.CountObj();
            obj.Value = value;
            obj.Count = 0;
            obj.Code = code;
            obj.DT = dt;
            CounterList.Add(obj);
            return obj;
        }

        public  void AddCounter(string v,string c)
        {
            Core.CountObj obj = GetCounterByValue(v, c, DateTime.Now);
            obj.Count++;
            obj.DT = DateTime.Now;
        }

        private void butstartdz_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Length <= 0)
            {
                MessageBox.Show("没有统计内容");
                return;
            }
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.ljcount));
            th.Start();
        }

        private void butrestart_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Length <= 0)
            {
                MessageBox.Show("没有统计内容");
                return;
            }
             
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(this.recount));
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text == "全部")
            {
                dt.DefaultView.RowFilter = "";
            }
            if (this.comboBox2.Text == "单字")
            {
                dt.DefaultView.RowFilter = "len(字词)=1";
            }
            if (this.comboBox2.Text == "词组")
            {
                dt.DefaultView.RowFilter = "len(字词)>1";
            }
            if (this.comboBox2.Text == "2字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=2";
            }
            if (this.comboBox2.Text == "3字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=3";
            }
            if (this.comboBox2.Text == "4字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=4";
            }
            if (this.comboBox2.Text == "5字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=5";
            }
            if (this.comboBox2.Text == "6字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=6";
            }
            if (this.comboBox2.Text == "7字词组")
            {
                dt.DefaultView.RowFilter = "len(字词)=7";
            }
        }

        /// <summary>
        /// 重新统计
        /// </summary>
        private void recount()
        {
            this.butrestart.Enabled = false;
            this.butstartdz.Enabled = false;
            this.button1.Enabled = false;
           
            CounterList.Clear();
            dt.Clear();
            this.progressBar1.Maximum = this.textBox1.Text.Length;
            this.progressBar1.Value = 0;
            if (comboBox1.Text == "单字")
            {
                for (int i = 0; i < this.textBox1.Text.Length; i++)
                {
                    string ss = this.textBox1.Text.Substring(i, 1);
                    this.progressBar1.Value++;
                    if (ss.Length <= 0) continue;
                    AddCounter(ss, "");
                }
            }
            else
            {
                for (int i = 0; i < this.textBox1.Text.Length; i++)
                {
                    System.Threading.Thread.Sleep(5);
                    string ss = "";
                    if (i + 7 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 7);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 6;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 6 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 6);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 5;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 5 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 5);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 4;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 4 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 4);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 3;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 3 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 3);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 2;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 2 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 2);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 1;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    ss = this.textBox1.Text.Substring(i, 1);
                    this.progressBar1.Value++;
                step1:
                    if (ss.Length <= 0) continue;
                    AddCounter(ss, "");
                }
            }

            foreach (Core.CountObj item in CounterList)
            {
                DataRow dr = dt.NewRow();
                dr["字词"] = item.Value;
                dr["输出次数"] = item.Count;
                dt.Rows.Add(dr);
            }
            
            this.dataGridView1.DataSource = dt.DefaultView;
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.butrestart.Enabled = true;
            this.butstartdz.Enabled = true;
            this.button1.Enabled = true;
        }

        /// <summary>
        /// 累计统计
        /// </summary>
        private void ljcount()
        {
            this.butrestart.Enabled = false;
            this.butstartdz.Enabled = false;
            this.button1.Enabled = false;
           
            dt.Clear();
            this.progressBar1.Maximum = this.textBox1.Text.Length;
            this.progressBar1.Value = 0;
            if (comboBox1.Text == "单字")
            {
                for (int i = 0; i < this.textBox1.Text.Length; i++)
                {
                    string ss = this.textBox1.Text.Substring(i, 1);
                    this.progressBar1.Value++;
                    if (ss.Length <= 0) continue;
                    AddCounter(ss, "");
                }
            }
            else
            {
                for (int i = 0; i < this.textBox1.Text.Length; i++)
                {
                    System.Threading.Thread.Sleep(5);
                    string ss = "";
                    if (i + 7 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 7);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 6;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 6 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 6);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 5;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 5 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 5);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 4;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 4 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 4);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 3;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 3 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 3);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 2;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    if (i + 2 <= this.textBox1.Text.Length)
                    {
                        ss = this.textBox1.Text.Substring(i, 2);
                        if (Core.InputHelp.HaveCodeByValue(ss).Length > 0)
                        {
                            i = i + 1;
                            this.progressBar1.Value = i;
                            goto step1;
                        }
                    }
                    ss = this.textBox1.Text.Substring(i, 1);
                    this.progressBar1.Value++;
                step1:
                    if (ss.Length <= 0) continue;
                    AddCounter(ss, "");
                }
            }

            foreach (Core.CountObj item in CounterList)
            {
                DataRow dr = dt.NewRow();
                dr["字词"] = item.Value;
                dr["输出次数"] = item.Count;
                dt.Rows.Add(dr);
            }

            this.dataGridView1.DataSource = dt.DefaultView;
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.butrestart.Enabled = true;
            this.butstartdz.Enabled = true;
            this.button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] arr = new string[dt.Rows.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = dt.Rows[i][0].ToString();
            }
            WordCodeFrm wcfrm = new WordCodeFrm();
            wcfrm.Show();
            wcfrm.Arr = arr;
        }
    }
}
