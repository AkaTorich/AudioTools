namespace WAVConverter
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
            this.openWAVFile = new System.Windows.Forms.Button();
            this.convertToMP3 = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // openWAVFile
            // 
            this.openWAVFile.BackColor = System.Drawing.Color.Green;
            this.openWAVFile.ForeColor = System.Drawing.Color.White;
            this.openWAVFile.Location = new System.Drawing.Point(13, 13);
            this.openWAVFile.Name = "openWAVFile";
            this.openWAVFile.Size = new System.Drawing.Size(400, 23);
            this.openWAVFile.TabIndex = 0;
            this.openWAVFile.Text = "Выбрать файлы";
            this.openWAVFile.UseVisualStyleBackColor = false;
            this.openWAVFile.Click += new System.EventHandler(this.openWAVFile_Click);
            // 
            // convertToMP3
            // 
            this.convertToMP3.BackColor = System.Drawing.Color.Green;
            this.convertToMP3.ForeColor = System.Drawing.Color.White;
            this.convertToMP3.Location = new System.Drawing.Point(14, 66);
            this.convertToMP3.Name = "convertToMP3";
            this.convertToMP3.Size = new System.Drawing.Size(400, 23);
            this.convertToMP3.TabIndex = 2;
            this.convertToMP3.Text = "Конвертировать";
            this.convertToMP3.UseVisualStyleBackColor = false;
            this.convertToMP3.Click += new System.EventHandler(this.convertFiles_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Green;
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.Location = new System.Drawing.Point(14, 95);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(400, 212);
            this.txtLog.TabIndex = 3;
            // 
            // comboFormat
            // 
            this.comboFormat.BackColor = System.Drawing.Color.Black;
            this.comboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFormat.ForeColor = System.Drawing.Color.Fuchsia;
            this.comboFormat.FormattingEnabled = true;
            this.comboFormat.Items.AddRange(new object[] {
            "Выберите формат для конвертации",
            "Конвертировать в MP3",
            "Конвертировать в AIFF",
            "Конвертировать в WAV"});
            this.comboFormat.Location = new System.Drawing.Point(14, 39);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(399, 21);
            this.comboFormat.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(429, 320);
            this.Controls.Add(this.comboFormat);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.convertToMP3);
            this.Controls.Add(this.openWAVFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AudioConverter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Button openWAVFile;
        private System.Windows.Forms.Button convertToMP3;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ComboBox comboFormat;
    }
}