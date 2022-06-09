namespace MiouIME
{
    partial class GetWordFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmblen = new System.Windows.Forms.ComboBox();
            this.cmbinfo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.butexproct = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmbtype = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "字词长度";
            // 
            // cmblen
            // 
            this.cmblen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmblen.FormattingEnabled = true;
            this.cmblen.Items.AddRange(new object[] {
            "单字",
            "两字词",
            "三字词",
            "四字词",
            "五字词",
            "六字词",
            "六字以上词",
            "全部词组",
            "全部词库"});
            this.cmblen.Location = new System.Drawing.Point(92, 19);
            this.cmblen.Name = "cmblen";
            this.cmblen.Size = new System.Drawing.Size(121, 20);
            this.cmblen.TabIndex = 1;
            // 
            // cmbinfo
            // 
            this.cmbinfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbinfo.FormattingEnabled = true;
            this.cmbinfo.Items.AddRange(new object[] {
            "仅汉字",
            "汉字和编码"});
            this.cmbinfo.Location = new System.Drawing.Point(92, 44);
            this.cmbinfo.Name = "cmbinfo";
            this.cmbinfo.Size = new System.Drawing.Size(121, 20);
            this.cmbinfo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "导出信息";
            // 
            // butexproct
            // 
            this.butexproct.Location = new System.Drawing.Point(92, 101);
            this.butexproct.Name = "butexproct";
            this.butexproct.Size = new System.Drawing.Size(75, 23);
            this.butexproct.TabIndex = 4;
            this.butexproct.Text = "导出";
            this.butexproct.UseVisualStyleBackColor = true;
            this.butexproct.Click += new System.EventHandler(this.butexproct_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 130);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(276, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // cmbtype
            // 
            this.cmbtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbtype.FormattingEnabled = true;
            this.cmbtype.Items.AddRange(new object[] {
            "全部",
            "一简",
            "二简",
            "三简",
            "四简"});
            this.cmbtype.Location = new System.Drawing.Point(92, 70);
            this.cmbtype.Name = "cmbtype";
            this.cmbtype.Size = new System.Drawing.Size(121, 20);
            this.cmbtype.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "类型信息";
            // 
            // GetWordFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 153);
            this.Controls.Add(this.cmbtype);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.butexproct);
            this.Controls.Add(this.cmbinfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmblen);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetWordFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "字词导出";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmblen;
        private System.Windows.Forms.ComboBox cmbinfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button butexproct;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox cmbtype;
        private System.Windows.Forms.Label label3;
    }
}