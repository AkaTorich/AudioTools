namespace ScaleSelector
{
    partial class ScaleSelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleSelectorForm));
            this.comboBoxScale = new System.Windows.Forms.ComboBox();
            this.comboBoxTonic = new System.Windows.Forms.ComboBox();
            this.buttonShowNotes = new System.Windows.Forms.Button();
            this.richTextBoxNotes = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxScale
            // 
            this.comboBoxScale.BackColor = System.Drawing.Color.YellowGreen;
            this.comboBoxScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScale.FormattingEnabled = true;
            this.comboBoxScale.Location = new System.Drawing.Point(62, 16);
            this.comboBoxScale.Name = "comboBoxScale";
            this.comboBoxScale.Size = new System.Drawing.Size(247, 21);
            this.comboBoxScale.TabIndex = 0;
            // 
            // comboBoxTonic
            // 
            this.comboBoxTonic.BackColor = System.Drawing.Color.YellowGreen;
            this.comboBoxTonic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTonic.FormattingEnabled = true;
            this.comboBoxTonic.Location = new System.Drawing.Point(62, 43);
            this.comboBoxTonic.Name = "comboBoxTonic";
            this.comboBoxTonic.Size = new System.Drawing.Size(247, 21);
            this.comboBoxTonic.TabIndex = 1;
            // 
            // buttonShowNotes
            // 
            this.buttonShowNotes.BackColor = System.Drawing.Color.YellowGreen;
            this.buttonShowNotes.Location = new System.Drawing.Point(315, 16);
            this.buttonShowNotes.Name = "buttonShowNotes";
            this.buttonShowNotes.Size = new System.Drawing.Size(116, 48);
            this.buttonShowNotes.TabIndex = 2;
            this.buttonShowNotes.Text = "Показать";
            this.buttonShowNotes.UseVisualStyleBackColor = false;
            this.buttonShowNotes.Click += new System.EventHandler(this.ButtonShowNotes_Click);
            // 
            // richTextBoxNotes
            // 
            this.richTextBoxNotes.BackColor = System.Drawing.Color.LightGreen;
            this.richTextBoxNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBoxNotes.Location = new System.Drawing.Point(12, 70);
            this.richTextBoxNotes.Name = "richTextBoxNotes";
            this.richTextBoxNotes.Size = new System.Drawing.Size(419, 79);
            this.richTextBoxNotes.TabIndex = 3;
            this.richTextBoxNotes.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Lime;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Гамма:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Тоника:";
            // 
            // ScaleSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ScaleSelector.Properties.Resources.ABSTRACT__18_;
            this.ClientSize = new System.Drawing.Size(444, 163);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxNotes);
            this.Controls.Add(this.buttonShowNotes);
            this.Controls.Add(this.comboBoxTonic);
            this.Controls.Add(this.comboBoxScale);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ScaleSelectorForm";
            this.Text = "ScaleSelector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScaleSelectorForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxScale;
        private System.Windows.Forms.ComboBox comboBoxTonic;
        private System.Windows.Forms.Button buttonShowNotes;
        private System.Windows.Forms.RichTextBox richTextBoxNotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
    }
}
