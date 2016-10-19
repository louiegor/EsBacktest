namespace WinFormData
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.statusTextBox = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.traderId = new System.Windows.Forms.TextBox();
            this.eSignalOutputPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.univShareSize = new System.Windows.Forms.TextBox();
            this.pproPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Lv1PositionGrid = new System.Windows.Forms.DataGridView();
            this.PassiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numOfRetries = new System.Windows.Forms.NumericUpDown();
            this.isTms = new System.Windows.Forms.CheckBox();
            this.totalTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numStock = new System.Windows.Forms.TextBox();
            this.discardGrid = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.IsFilter = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ProfitFilter = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.WinRatioFilter = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.FilteredProfit = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.FilterNumStock = new System.Windows.Forms.TextBox();
            this.InstructionTextBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Lv1PositionGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOfRetries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discardGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Keep";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(652, 706);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(731, 706);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // statusTextBox
            // 
            this.statusTextBox.Location = new System.Drawing.Point(563, 503);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.Size = new System.Drawing.Size(245, 197);
            this.statusTextBox.TabIndex = 6;
            this.statusTextBox.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(563, 487);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Status";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(31, 586);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(49, 13);
            this.label21.TabIndex = 8;
            this.label21.Text = "TraderID";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(9, 613);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(71, 13);
            this.label42.TabIndex = 9;
            this.label42.Text = "E-Signal Path";
            // 
            // traderId
            // 
            this.traderId.Location = new System.Drawing.Point(86, 583);
            this.traderId.Name = "traderId";
            this.traderId.Size = new System.Drawing.Size(294, 20);
            this.traderId.TabIndex = 10;
            this.traderId.Text = "TNVSTR05";
            // 
            // eSignalOutputPath
            // 
            this.eSignalOutputPath.Location = new System.Drawing.Point(86, 610);
            this.eSignalOutputPath.Name = "eSignalOutputPath";
            this.eSignalOutputPath.Size = new System.Drawing.Size(294, 20);
            this.eSignalOutputPath.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 665);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Share Size";
            // 
            // univShareSize
            // 
            this.univShareSize.Location = new System.Drawing.Point(86, 662);
            this.univShareSize.Name = "univShareSize";
            this.univShareSize.Size = new System.Drawing.Size(294, 20);
            this.univShareSize.TabIndex = 13;
            this.univShareSize.Text = "100";
            // 
            // pproPath
            // 
            this.pproPath.Location = new System.Drawing.Point(86, 636);
            this.pproPath.Name = "pproPath";
            this.pproPath.Size = new System.Drawing.Size(294, 20);
            this.pproPath.TabIndex = 16;
            this.pproPath.Text = "C:\\PPro8_Beta\\";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 639);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Ppro Path";
            // 
            // Lv1PositionGrid
            // 
            this.Lv1PositionGrid.AllowUserToAddRows = false;
            this.Lv1PositionGrid.AllowUserToDeleteRows = false;
            this.Lv1PositionGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Lv1PositionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.Lv1PositionGrid.Location = new System.Drawing.Point(12, 25);
            this.Lv1PositionGrid.MultiSelect = false;
            this.Lv1PositionGrid.Name = "Lv1PositionGrid";
            this.Lv1PositionGrid.ReadOnly = true;
            this.Lv1PositionGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Lv1PositionGrid.RowHeadersVisible = false;
            this.Lv1PositionGrid.Size = new System.Drawing.Size(545, 247);
            this.Lv1PositionGrid.TabIndex = 17;
            // 
            // PassiveCheckBox
            // 
            this.PassiveCheckBox.AutoSize = true;
            this.PassiveCheckBox.Checked = true;
            this.PassiveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PassiveCheckBox.Location = new System.Drawing.Point(121, 713);
            this.PassiveCheckBox.Name = "PassiveCheckBox";
            this.PassiveCheckBox.Size = new System.Drawing.Size(63, 17);
            this.PassiveCheckBox.TabIndex = 18;
            this.PassiveCheckBox.Text = "Passive";
            this.PassiveCheckBox.UseVisualStyleBackColor = true;
            this.PassiveCheckBox.CheckedChanged += new System.EventHandler(this.PassiveCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 689);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Bid/Offer # of retries";
            // 
            // numOfRetries
            // 
            this.numOfRetries.Location = new System.Drawing.Point(139, 687);
            this.numOfRetries.Name = "numOfRetries";
            this.numOfRetries.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.numOfRetries.Size = new System.Drawing.Size(120, 20);
            this.numOfRetries.TabIndex = 21;
            this.numOfRetries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // isTms
            // 
            this.isTms.AutoSize = true;
            this.isTms.Location = new System.Drawing.Point(25, 713);
            this.isTms.Name = "isTms";
            this.isTms.Size = new System.Drawing.Size(49, 17);
            this.isTms.TabIndex = 22;
            this.isTms.Text = "TMS";
            this.isTms.UseVisualStyleBackColor = true;
            this.isTms.CheckedChanged += new System.EventHandler(this.isTms_CheckedChanged);
            // 
            // totalTextBox
            // 
            this.totalTextBox.Location = new System.Drawing.Point(563, 47);
            this.totalTextBox.Name = "totalTextBox";
            this.totalTextBox.Size = new System.Drawing.Size(100, 20);
            this.totalTextBox.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(563, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Total";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(563, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Number of Stocks";
            // 
            // numStock
            // 
            this.numStock.Location = new System.Drawing.Point(563, 96);
            this.numStock.Name = "numStock";
            this.numStock.Size = new System.Drawing.Size(100, 20);
            this.numStock.TabIndex = 27;
            // 
            // discardGrid
            // 
            this.discardGrid.AllowUserToAddRows = false;
            this.discardGrid.AllowUserToDeleteRows = false;
            this.discardGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.discardGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.discardGrid.Location = new System.Drawing.Point(12, 302);
            this.discardGrid.MultiSelect = false;
            this.discardGrid.Name = "discardGrid";
            this.discardGrid.ReadOnly = true;
            this.discardGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.discardGrid.RowHeadersVisible = false;
            this.discardGrid.Size = new System.Drawing.Size(545, 257);
            this.discardGrid.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Discard";
            // 
            // IsFilter
            // 
            this.IsFilter.AutoSize = true;
            this.IsFilter.Checked = true;
            this.IsFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsFilter.Location = new System.Drawing.Point(566, 138);
            this.IsFilter.Name = "IsFilter";
            this.IsFilter.Size = new System.Drawing.Size(48, 17);
            this.IsFilter.TabIndex = 31;
            this.IsFilter.Text = "Filter";
            this.IsFilter.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(563, 216);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 13);
            this.label9.TabIndex = 35;
            this.label9.Text = "Profit above % to price";
            // 
            // ProfitFilter
            // 
            this.ProfitFilter.Location = new System.Drawing.Point(563, 232);
            this.ProfitFilter.Name = "ProfitFilter";
            this.ProfitFilter.Size = new System.Drawing.Size(100, 20);
            this.ProfitFilter.TabIndex = 34;
            this.ProfitFilter.Text = "5";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(563, 167);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 33;
            this.label10.Text = "Win ratio above %";
            // 
            // WinRatioFilter
            // 
            this.WinRatioFilter.Location = new System.Drawing.Point(563, 183);
            this.WinRatioFilter.Name = "WinRatioFilter";
            this.WinRatioFilter.Size = new System.Drawing.Size(100, 20);
            this.WinRatioFilter.TabIndex = 32;
            this.WinRatioFilter.Text = "50";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(563, 268);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Profit after filter";
            // 
            // FilteredProfit
            // 
            this.FilteredProfit.Location = new System.Drawing.Point(563, 284);
            this.FilteredProfit.Name = "FilteredProfit";
            this.FilteredProfit.ReadOnly = true;
            this.FilteredProfit.Size = new System.Drawing.Size(100, 20);
            this.FilteredProfit.TabIndex = 36;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(563, 316);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(102, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Filter Total # Stocks";
            // 
            // FilterNumStock
            // 
            this.FilterNumStock.Location = new System.Drawing.Point(563, 332);
            this.FilterNumStock.Name = "FilterNumStock";
            this.FilterNumStock.ReadOnly = true;
            this.FilterNumStock.Size = new System.Drawing.Size(100, 20);
            this.FilterNumStock.TabIndex = 38;
            // 
            // InstructionTextBox
            // 
            this.InstructionTextBox.BackColor = System.Drawing.Color.MistyRose;
            this.InstructionTextBox.Location = new System.Drawing.Point(566, 374);
            this.InstructionTextBox.Name = "InstructionTextBox";
            this.InstructionTextBox.ReadOnly = true;
            this.InstructionTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.InstructionTextBox.Size = new System.Drawing.Size(240, 81);
            this.InstructionTextBox.TabIndex = 40;
            this.InstructionTextBox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 740);
            this.Controls.Add(this.InstructionTextBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.FilterNumStock);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FilteredProfit);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ProfitFilter);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.WinRatioFilter);
            this.Controls.Add(this.IsFilter);
            this.Controls.Add(this.discardGrid);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numStock);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.totalTextBox);
            this.Controls.Add(this.isTms);
            this.Controls.Add(this.numOfRetries);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PassiveCheckBox);
            this.Controls.Add(this.Lv1PositionGrid);
            this.Controls.Add(this.pproPath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.univShareSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.eSignalOutputPath);
            this.Controls.Add(this.traderId);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Esignal Trader";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Lv1PositionGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOfRetries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discardGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.RichTextBox statusTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox traderId;
        private System.Windows.Forms.TextBox eSignalOutputPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox univShareSize;
        private System.Windows.Forms.TextBox pproPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView Lv1PositionGrid;
        private System.Windows.Forms.CheckBox PassiveCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numOfRetries;
        private System.Windows.Forms.CheckBox isTms;
        private System.Windows.Forms.TextBox totalTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox numStock;
        private System.Windows.Forms.DataGridView discardGrid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox IsFilter;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ProfitFilter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox WinRatioFilter;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox FilteredProfit;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox FilterNumStock;
        private System.Windows.Forms.RichTextBox InstructionTextBox;
    }
}

