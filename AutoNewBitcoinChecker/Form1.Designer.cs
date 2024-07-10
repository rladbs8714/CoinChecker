namespace AutoNewBitcoinChecker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MainContainer = new SplitContainer();
            txtMainLog = new TextBox();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            grpUser = new GroupBox();
            grpMoney = new GroupBox();
            grpHaveCoin = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)MainContainer).BeginInit();
            MainContainer.Panel1.SuspendLayout();
            MainContainer.Panel2.SuspendLayout();
            MainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // MainContainer
            // 
            MainContainer.Dock = DockStyle.Fill;
            MainContainer.IsSplitterFixed = true;
            MainContainer.Location = new Point(0, 0);
            MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel1
            // 
            MainContainer.Panel1.Controls.Add(txtMainLog);
            // 
            // MainContainer.Panel2
            // 
            MainContainer.Panel2.Controls.Add(splitContainer1);
            MainContainer.Size = new Size(934, 586);
            MainContainer.SplitterDistance = 321;
            MainContainer.TabIndex = 0;
            // 
            // txtMainLog
            // 
            txtMainLog.Dock = DockStyle.Fill;
            txtMainLog.Location = new Point(0, 0);
            txtMainLog.Multiline = true;
            txtMainLog.Name = "txtMainLog";
            txtMainLog.Size = new Size(321, 586);
            txtMainLog.TabIndex = 0;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(grpHaveCoin);
            splitContainer1.Size = new Size(609, 586);
            splitContainer1.SplitterDistance = 203;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(grpUser);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(grpMoney);
            splitContainer2.Size = new Size(609, 203);
            splitContainer2.SplitterDistance = 301;
            splitContainer2.TabIndex = 0;
            // 
            // grpUser
            // 
            grpUser.Dock = DockStyle.Fill;
            grpUser.Location = new Point(0, 0);
            grpUser.Name = "grpUser";
            grpUser.Size = new Size(301, 203);
            grpUser.TabIndex = 0;
            grpUser.TabStop = false;
            grpUser.Text = "사용자 정보";
            // 
            // grpMoney
            // 
            grpMoney.Dock = DockStyle.Fill;
            grpMoney.Location = new Point(0, 0);
            grpMoney.Name = "grpMoney";
            grpMoney.Size = new Size(304, 203);
            grpMoney.TabIndex = 0;
            grpMoney.TabStop = false;
            grpMoney.Text = "화폐";
            // 
            // grpHaveCoin
            // 
            grpHaveCoin.Dock = DockStyle.Fill;
            grpHaveCoin.Location = new Point(0, 0);
            grpHaveCoin.Name = "grpHaveCoin";
            grpHaveCoin.Size = new Size(609, 379);
            grpHaveCoin.TabIndex = 0;
            grpHaveCoin.TabStop = false;
            grpHaveCoin.Text = "소유 화폐";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(934, 586);
            Controls.Add(MainContainer);
            MaximizeBox = false;
            Name = "Form1";
            Text = "업비트 신규 코인 자동 탐색기";
            MainContainer.Panel1.ResumeLayout(false);
            MainContainer.Panel1.PerformLayout();
            MainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainContainer).EndInit();
            MainContainer.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer MainContainer;
        private TextBox txtMainLog;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private GroupBox grpUser;
        private GroupBox grpMoney;
        private GroupBox grpHaveCoin;
    }
}
