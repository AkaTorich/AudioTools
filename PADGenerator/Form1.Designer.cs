namespace MIDIGenerator
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
            this.cmbBoxTonality = new System.Windows.Forms.ComboBox();
            this.cmbBoxScale = new System.Windows.Forms.ComboBox();
            this.txtNumberOfChords = new System.Windows.Forms.TextBox();
            this.txtProgression = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtProgressionLength = new System.Windows.Forms.TextBox();
            this.btnPlayMIDI = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbBoxTonality
            // 
            this.cmbBoxTonality.BackColor = System.Drawing.SystemColors.Info;
            this.cmbBoxTonality.FormattingEnabled = true;
            this.cmbBoxTonality.Location = new System.Drawing.Point(106, 12);
            this.cmbBoxTonality.Name = "cmbBoxTonality";
            this.cmbBoxTonality.Size = new System.Drawing.Size(137, 21);
            this.cmbBoxTonality.TabIndex = 0;
            // 
            // cmbBoxScale
            // 
            this.cmbBoxScale.BackColor = System.Drawing.SystemColors.Info;
            this.cmbBoxScale.FormattingEnabled = true;
            this.cmbBoxScale.Location = new System.Drawing.Point(106, 39);
            this.cmbBoxScale.Name = "cmbBoxScale";
            this.cmbBoxScale.Size = new System.Drawing.Size(137, 21);
            this.cmbBoxScale.TabIndex = 1;
            // 
            // txtNumberOfChords
            // 
            this.txtNumberOfChords.BackColor = System.Drawing.SystemColors.Info;
            this.txtNumberOfChords.Location = new System.Drawing.Point(106, 66);
            this.txtNumberOfChords.MaxLength = 2;
            this.txtNumberOfChords.Name = "txtNumberOfChords";
            this.txtNumberOfChords.Size = new System.Drawing.Size(137, 20);
            this.txtNumberOfChords.TabIndex = 2;
            this.txtNumberOfChords.Text = "5";
            // 
            // txtProgression
            // 
            this.txtProgression.BackColor = System.Drawing.SystemColors.Info;
            this.txtProgression.Location = new System.Drawing.Point(249, 39);
            this.txtProgression.Multiline = true;
            this.txtProgression.Name = "txtProgression";
            this.txtProgression.Size = new System.Drawing.Size(260, 126);
            this.txtProgression.TabIndex = 3;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(12, 121);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(231, 44);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Генерировать";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Тоника";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Лад";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Прогрессия";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Кол-во аккордов";
            // 
            // txtProgressionLength
            // 
            this.txtProgressionLength.BackColor = System.Drawing.SystemColors.Info;
            this.txtProgressionLength.Location = new System.Drawing.Point(106, 92);
            this.txtProgressionLength.MaxLength = 2;
            this.txtProgressionLength.Name = "txtProgressionLength";
            this.txtProgressionLength.Size = new System.Drawing.Size(137, 20);
            this.txtProgressionLength.TabIndex = 9;
            this.txtProgressionLength.Text = "8";
            // 
            // btnPlayMIDI
            // 
            this.btnPlayMIDI.Location = new System.Drawing.Point(250, 11);
            this.btnPlayMIDI.Name = "btnPlayMIDI";
            this.btnPlayMIDI.Size = new System.Drawing.Size(259, 22);
            this.btnPlayMIDI.TabIndex = 10;
            this.btnPlayMIDI.Text = "Воспроизвести MIDI";
            this.btnPlayMIDI.UseVisualStyleBackColor = true;
            this.btnPlayMIDI.Click += new System.EventHandler(this.btnPlayMIDI_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Coral;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(518, 177);
            this.Controls.Add(this.btnPlayMIDI);
            this.Controls.Add(this.txtProgressionLength);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtProgression);
            this.Controls.Add(this.txtNumberOfChords);
            this.Controls.Add(this.cmbBoxScale);
            this.Controls.Add(this.cmbBoxTonality);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PADGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.ComboBox cmbBoxTonality;
        private System.Windows.Forms.ComboBox cmbBoxScale;
        private System.Windows.Forms.TextBox txtProgression;
        private System.Windows.Forms.TextBox txtNumberOfChords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtProgressionLength;
        private System.Windows.Forms.Button btnPlayMIDI;
    }
}

