namespace BitcoinChecker.AutoTrading
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
            gbGeneralInfo = new GroupBox();
            textBox2 = new TextBox();
            label2 = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            gbGeneralSetting = new GroupBox();
            comboBox2 = new ComboBox();
            label4 = new Label();
            comboBox1 = new ComboBox();
            label3 = new Label();
            textBox3 = new TextBox();
            groupBox1 = new GroupBox();
            label5 = new Label();
            textBox4 = new TextBox();
            gbGeneralInfo.SuspendLayout();
            gbGeneralSetting.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // gbGeneralInfo
            // 
            gbGeneralInfo.Controls.Add(textBox2);
            gbGeneralInfo.Controls.Add(label2);
            gbGeneralInfo.Controls.Add(textBox1);
            gbGeneralInfo.Controls.Add(label1);
            gbGeneralInfo.Location = new Point(12, 12);
            gbGeneralInfo.Name = "gbGeneralInfo";
            gbGeneralInfo.Size = new Size(258, 92);
            gbGeneralInfo.TabIndex = 0;
            gbGeneralInfo.TabStop = false;
            gbGeneralInfo.Text = "일반 정보";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(92, 54);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.RightToLeft = RightToLeft.Yes;
            textBox2.Size = new Size(149, 23);
            textBox2.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 57);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 1;
            label2.Text = "보유 원화";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(92, 25);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.RightToLeft = RightToLeft.Yes;
            textBox1.Size = new Size(149, 23);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 28);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 1;
            label1.Text = "총 보유자산";
            // 
            // gbGeneralSetting
            // 
            gbGeneralSetting.Controls.Add(groupBox1);
            gbGeneralSetting.Controls.Add(textBox3);
            gbGeneralSetting.Controls.Add(comboBox2);
            gbGeneralSetting.Controls.Add(label4);
            gbGeneralSetting.Controls.Add(comboBox1);
            gbGeneralSetting.Controls.Add(label3);
            gbGeneralSetting.Location = new Point(12, 110);
            gbGeneralSetting.Name = "gbGeneralSetting";
            gbGeneralSetting.Size = new Size(258, 328);
            gbGeneralSetting.TabIndex = 1;
            gbGeneralSetting.TabStop = false;
            gbGeneralSetting.Text = "일반 설정";
            // 
            // comboBox2
            // 
            comboBox2.Enabled = false;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "1", "3", "5", "15", "10", "30", "60", "240" });
            comboBox2.Location = new Point(92, 51);
            comboBox2.Name = "comboBox2";
            comboBox2.RightToLeft = RightToLeft.Yes;
            comboBox2.Size = new Size(149, 23);
            comboBox2.TabIndex = 4;
            comboBox2.Text = "1";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Enabled = false;
            label4.Location = new Point(11, 54);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 5;
            label4.Text = "분 캔들 단위";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "초(Second)", "분(Minute)" });
            comboBox1.Location = new Point(92, 22);
            comboBox1.Name = "comboBox1";
            comboBox1.RightToLeft = RightToLeft.Yes;
            comboBox1.Size = new Size(149, 23);
            comboBox1.TabIndex = 2;
            comboBox1.Text = "초(Second)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(27, 25);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 3;
            label3.Text = "캔들 설정";
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.Black;
            textBox3.Enabled = false;
            textBox3.Location = new Point(15, 80);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(226, 1);
            textBox3.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox4);
            groupBox1.Controls.Add(label5);
            groupBox1.Location = new Point(15, 87);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(226, 100);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "매수";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 25);
            label5.Name = "label5";
            label5.Size = new Size(59, 15);
            label5.TabIndex = 7;
            label5.Text = "음봉 크기";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(77, 22);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(135, 23);
            textBox4.TabIndex = 2;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gbGeneralSetting);
            Controls.Add(gbGeneralInfo);
            MaximizeBox = false;
            Name = "frmMain";
            Text = "비트코인 자동 트레이딩";
            gbGeneralInfo.ResumeLayout(false);
            gbGeneralInfo.PerformLayout();
            gbGeneralSetting.ResumeLayout(false);
            gbGeneralSetting.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbGeneralInfo;
        private TextBox textBox1;
        private Label label1;
        private TextBox textBox2;
        private Label label2;
        private GroupBox gbGeneralSetting;
        private Label label3;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label4;
        private GroupBox groupBox1;
        private TextBox textBox4;
        private Label label5;
        private TextBox textBox3;
    }
}
