namespace Danmu
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.highDMTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.normalDMTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.giftTB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // highDMTB
            // 
            this.highDMTB.Enabled = false;
            this.highDMTB.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.highDMTB.Location = new System.Drawing.Point(1, 552);
            this.highDMTB.Multiline = true;
            this.highDMTB.Name = "highDMTB";
            this.highDMTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.highDMTB.Size = new System.Drawing.Size(753, 279);
            this.highDMTB.TabIndex = 0;
            this.highDMTB.TextChanged += new System.EventHandler(this.highDMTB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 515);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "房管+贵族弹幕";
            // 
            // normalDMTB
            // 
            this.normalDMTB.Enabled = false;
            this.normalDMTB.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normalDMTB.Location = new System.Drawing.Point(1, 44);
            this.normalDMTB.Multiline = true;
            this.normalDMTB.Name = "normalDMTB";
            this.normalDMTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.normalDMTB.Size = new System.Drawing.Size(753, 456);
            this.normalDMTB.TabIndex = 2;
            this.normalDMTB.TextChanged += new System.EventHandler(this.normalDMTB_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "普通弹幕区";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(793, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "礼物区";
            // 
            // giftTB
            // 
            this.giftTB.Enabled = false;
            this.giftTB.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.giftTB.Location = new System.Drawing.Point(779, 44);
            this.giftTB.Multiline = true;
            this.giftTB.Name = "giftTB";
            this.giftTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.giftTB.Size = new System.Drawing.Size(383, 787);
            this.giftTB.TabIndex = 5;
            this.giftTB.TextChanged += new System.EventHandler(this.giftTB_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 832);
            this.Controls.Add(this.giftTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.normalDMTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.highDMTB);
            this.MaximumSize = new System.Drawing.Size(1180, 871);
            this.MinimumSize = new System.Drawing.Size(1180, 871);
            this.Name = "Form1";
            this.Text = "随便玩玩-弹幕助手";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox highDMTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox normalDMTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox giftTB;
    }
}

