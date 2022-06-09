namespace MiouIME
{
    partial class InputFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lbinputstr = new System.Windows.Forms.Label();
            this.panelinput = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lbinputstr
            // 
            this.lbinputstr.AutoEllipsis = true;
            this.lbinputstr.BackColor = System.Drawing.Color.Transparent;
            this.lbinputstr.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbinputstr.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbinputstr.ForeColor = System.Drawing.Color.Red;
            this.lbinputstr.Location = new System.Drawing.Point(0, 0);
            this.lbinputstr.Name = "lbinputstr";
            this.lbinputstr.Size = new System.Drawing.Size(184, 20);
            this.lbinputstr.TabIndex = 0;
            this.lbinputstr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbinputstr.Visible = false;
            this.lbinputstr.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbinputstr_MouseDown);
            this.lbinputstr.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbinputstr_MouseMove);
            // 
            // panelinput
            // 
            this.panelinput.BackColor = System.Drawing.Color.Transparent;
            this.panelinput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelinput.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panelinput.Location = new System.Drawing.Point(0, 20);
            this.panelinput.Name = "panelinput";
            this.panelinput.Size = new System.Drawing.Size(184, 24);
            this.panelinput.TabIndex = 1;
            this.panelinput.Visible = false;
            // 
            // InputFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(184, 44);
            this.Controls.Add(this.panelinput);
            this.Controls.Add(this.lbinputstr);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InputFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lbinputstr;
        public System.Windows.Forms.Panel panelinput;

    }
}

