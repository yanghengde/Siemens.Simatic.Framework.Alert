namespace Siemens.Simatic.WechatTest
{
    partial class frmTest
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
            this.lblBoxID = new DevComponents.DotNetBar.LabelX();
            this.txtPcbID = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnTest = new DevComponents.DotNetBar.ButtonX();
            this.btnSyncAgent = new DevComponents.DotNetBar.ButtonX();
            this.btnSyncOrg = new DevComponents.DotNetBar.ButtonX();
            this.btnSendImage = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // lblBoxID
            // 
            // 
            // 
            // 
            this.lblBoxID.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblBoxID.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblBoxID.Location = new System.Drawing.Point(73, 42);
            this.lblBoxID.Margin = new System.Windows.Forms.Padding(4);
            this.lblBoxID.Name = "lblBoxID";
            this.lblBoxID.Size = new System.Drawing.Size(75, 31);
            this.lblBoxID.TabIndex = 0;
            this.lblBoxID.Text = "内容";
            // 
            // txtPcbID
            // 
            // 
            // 
            // 
            this.txtPcbID.Border.Class = "TextBoxBorder";
            this.txtPcbID.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPcbID.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPcbID.Location = new System.Drawing.Point(174, 38);
            this.txtPcbID.Margin = new System.Windows.Forms.Padding(4);
            this.txtPcbID.Name = "txtPcbID";
            this.txtPcbID.PreventEnterBeep = true;
            this.txtPcbID.Size = new System.Drawing.Size(410, 39);
            this.txtPcbID.TabIndex = 1;
            this.txtPcbID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxID_KeyDown);
            // 
            // btnTest
            // 
            this.btnTest.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnTest.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnTest.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTest.Location = new System.Drawing.Point(651, 33);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(141, 51);
            this.btnTest.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnTest.TabIndex = 5;
            this.btnTest.Text = "发送文本";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSyncAgent
            // 
            this.btnSyncAgent.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSyncAgent.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSyncAgent.Enabled = false;
            this.btnSyncAgent.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSyncAgent.Location = new System.Drawing.Point(174, 318);
            this.btnSyncAgent.Margin = new System.Windows.Forms.Padding(4);
            this.btnSyncAgent.Name = "btnSyncAgent";
            this.btnSyncAgent.Size = new System.Drawing.Size(191, 51);
            this.btnSyncAgent.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSyncAgent.TabIndex = 6;
            this.btnSyncAgent.Text = "应用同步";
            this.btnSyncAgent.Visible = false;
            this.btnSyncAgent.Click += new System.EventHandler(this.btnSyncAgent_Click);
            // 
            // btnSyncOrg
            // 
            this.btnSyncOrg.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSyncOrg.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSyncOrg.Enabled = false;
            this.btnSyncOrg.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSyncOrg.Location = new System.Drawing.Point(432, 318);
            this.btnSyncOrg.Margin = new System.Windows.Forms.Padding(4);
            this.btnSyncOrg.Name = "btnSyncOrg";
            this.btnSyncOrg.Size = new System.Drawing.Size(170, 51);
            this.btnSyncOrg.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSyncOrg.TabIndex = 7;
            this.btnSyncOrg.Text = "组织人员同步";
            this.btnSyncOrg.Visible = false;
            this.btnSyncOrg.Click += new System.EventHandler(this.btnSyncOrg_Click);
            // 
            // btnSendImage
            // 
            this.btnSendImage.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSendImage.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSendImage.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSendImage.Location = new System.Drawing.Point(651, 116);
            this.btnSendImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendImage.Name = "btnSendImage";
            this.btnSendImage.Size = new System.Drawing.Size(141, 51);
            this.btnSendImage.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSendImage.TabIndex = 8;
            this.btnSendImage.Text = "发送图片";
            this.btnSendImage.Click += new System.EventHandler(this.btnSendImage_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 769);
            this.Controls.Add(this.btnSendImage);
            this.Controls.Add(this.btnSyncOrg);
            this.Controls.Add(this.btnSyncAgent);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtPcbID);
            this.Controls.Add(this.lblBoxID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTest";
            this.Text = "测试";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lblBoxID;
        private DevComponents.DotNetBar.Controls.TextBoxX txtPcbID;
        private DevComponents.DotNetBar.ButtonX btnTest;
        private DevComponents.DotNetBar.ButtonX btnSyncAgent;
        private DevComponents.DotNetBar.ButtonX btnSyncOrg;
        private DevComponents.DotNetBar.ButtonX btnSendImage;
    }
}