namespace AudioCutter
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblStart = new System.Windows.Forms.Label();
            this.trackBarStart = new System.Windows.Forms.TrackBar();
            this.trackBarEnd = new System.Windows.Forms.TrackBar();
            this.lblEnd = new System.Windows.Forms.Label();
            this.btnCut = new System.Windows.Forms.Button();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblCutDuration = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(241, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(414, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Выбрать трек";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(12, 38);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(44, 13);
            this.lblStart.TabIndex = 1;
            this.lblStart.Text = "Начало";
            // 
            // trackBarStart
            // 
            this.trackBarStart.Location = new System.Drawing.Point(15, 54);
            this.trackBarStart.Name = "trackBarStart";
            this.trackBarStart.Size = new System.Drawing.Size(642, 45);
            this.trackBarStart.TabIndex = 2;
            // 
            // trackBarEnd
            // 
            this.trackBarEnd.Location = new System.Drawing.Point(15, 105);
            this.trackBarEnd.Name = "trackBarEnd";
            this.trackBarEnd.Size = new System.Drawing.Size(642, 45);
            this.trackBarEnd.TabIndex = 3;
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(15, 85);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(38, 13);
            this.lblEnd.TabIndex = 4;
            this.lblEnd.Text = "Конец";
            // 
            // btnCut
            // 
            this.btnCut.Location = new System.Drawing.Point(241, 139);
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(414, 23);
            this.btnCut.TabIndex = 5;
            this.btnCut.Text = "Вырезать кусочек";
            this.btnCut.UseVisualStyleBackColor = true;
            this.btnCut.Click += new System.EventHandler(this.btnCut_Click);
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(12, 141);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(84, 21);
            this.cmbFormat.TabIndex = 6;
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(12, 17);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(186, 13);
            this.lblTotalTime.TabIndex = 7;
            this.lblTotalTime.Text = "Общая длительность: 0 мин. 0 сек.";
            // 
            // lblCutDuration
            // 
            this.lblCutDuration.AutoSize = true;
            this.lblCutDuration.Location = new System.Drawing.Point(102, 144);
            this.lblCutDuration.Name = "lblCutDuration";
            this.lblCutDuration.Size = new System.Drawing.Size(113, 13);
            this.lblCutDuration.TabIndex = 8;
            this.lblCutDuration.Text = "Длительность: 0 сек";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 178);
            this.Controls.Add(this.lblCutDuration);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.btnCut);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.trackBarEnd);
            this.Controls.Add(this.trackBarStart);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.btnLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AudioCutter";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TrackBar trackBarStart;
        private System.Windows.Forms.TrackBar trackBarEnd;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Button btnCut;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblCutDuration;
    }
}

