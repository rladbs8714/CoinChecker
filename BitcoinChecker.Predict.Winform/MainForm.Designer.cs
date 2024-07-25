namespace BitcoinChecker.Predict.Winform
{
    partial class frmMain
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
            menuStrip1 = new MenuStrip();
            updateToolStripMenuItem = new ToolStripMenuItem();
            끝내기XToolStripMenuItem = new ToolStripMenuItem();
            tbMain = new TabControl();
            KRW_BTC = new TabPage();
            menuStrip1.SuspendLayout();
            tbMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { updateToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // updateToolStripMenuItem
            // 
            updateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 끝내기XToolStripMenuItem });
            updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            updateToolStripMenuItem.Size = new Size(57, 20);
            updateToolStripMenuItem.Text = "파일(&F)";
            // 
            // 끝내기XToolStripMenuItem
            // 
            끝내기XToolStripMenuItem.Name = "끝내기XToolStripMenuItem";
            끝내기XToolStripMenuItem.Size = new Size(125, 22);
            끝내기XToolStripMenuItem.Text = "끝내기(&X)";
            // 
            // tbMain
            // 
            tbMain.Controls.Add(KRW_BTC);
            tbMain.Dock = DockStyle.Fill;
            tbMain.Location = new Point(0, 24);
            tbMain.Name = "tbMain";
            tbMain.SelectedIndex = 0;
            tbMain.Size = new Size(800, 548);
            tbMain.TabIndex = 1;
            // 
            // KRW_BTC
            // 
            KRW_BTC.Location = new Point(4, 24);
            KRW_BTC.Name = "KRW_BTC";
            KRW_BTC.Padding = new Padding(3);
            KRW_BTC.Size = new Size(792, 520);
            KRW_BTC.TabIndex = 0;
            KRW_BTC.Text = "KRW-BTC";
            KRW_BTC.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 572);
            Controls.Add(tbMain);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "frmMain";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tbMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem updateToolStripMenuItem;
        private ToolStripMenuItem 끝내기XToolStripMenuItem;
        private TabControl tbMain;
        private TabPage KRW_BTC;
    }
}
