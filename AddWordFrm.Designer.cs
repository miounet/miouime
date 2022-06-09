namespace MiouIME
{
    partial class AddWordFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddWordFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtcode = new System.Windows.Forms.TextBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.butadd = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtfile = new System.Windows.Forms.TextBox();
            this.butopenfile = new System.Windows.Forms.Button();
            this.butimport = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "编码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "字词";
            // 
            // txtcode
            // 
            this.txtcode.Location = new System.Drawing.Point(49, 9);
            this.txtcode.Name = "txtcode";
            this.txtcode.Size = new System.Drawing.Size(147, 21);
            this.txtcode.TabIndex = 2;
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(49, 35);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(230, 21);
            this.txtText.TabIndex = 3;
            // 
            // butadd
            // 
            this.butadd.Location = new System.Drawing.Point(290, 9);
            this.butadd.Name = "butadd";
            this.butadd.Size = new System.Drawing.Size(51, 47);
            this.butadd.TabIndex = 4;
            this.butadd.Text = "添加";
            this.butadd.UseVisualStyleBackColor = true;
            this.butadd.Click += new System.EventHandler(this.butadd_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 144);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(357, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // txtfile
            // 
            this.txtfile.Enabled = false;
            this.txtfile.Location = new System.Drawing.Point(14, 75);
            this.txtfile.Name = "txtfile";
            this.txtfile.Size = new System.Drawing.Size(249, 21);
            this.txtfile.TabIndex = 7;
            // 
            // butopenfile
            // 
            this.butopenfile.Location = new System.Drawing.Point(266, 74);
            this.butopenfile.Name = "butopenfile";
            this.butopenfile.Size = new System.Drawing.Size(84, 23);
            this.butopenfile.TabIndex = 8;
            this.butopenfile.Text = " 选择文件...";
            this.butopenfile.UseVisualStyleBackColor = true;
            this.butopenfile.Click += new System.EventHandler(this.butopenfile_Click);
            // 
            // butimport
            // 
            this.butimport.Location = new System.Drawing.Point(266, 103);
            this.butimport.Name = "butimport";
            this.butimport.Size = new System.Drawing.Size(84, 28);
            this.butimport.TabIndex = 9;
            this.butimport.Text = "开始导入";
            this.butimport.UseVisualStyleBackColor = true;
            this.butimport.Click += new System.EventHandler(this.butimport_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AddWordFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 167);
            this.Controls.Add(this.butimport);
            this.Controls.Add(this.butopenfile);
            this.Controls.Add(this.txtfile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.butadd);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.txtcode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddWordFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自定义字词";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtcode;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Button butadd;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtfile;
        private System.Windows.Forms.Button butopenfile;
        private System.Windows.Forms.Button butimport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}