namespace MelodyGenerator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTonica;
        private System.Windows.Forms.ComboBox cmbBoxTonica;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.ComboBox cmbBoxScale;
        private System.Windows.Forms.Label lblArpNotesCount;
        private System.Windows.Forms.TextBox txtArpNotesCount;
        private System.Windows.Forms.Label lblTactsNumber;
        private System.Windows.Forms.TextBox txtTactsNumber;
        private System.Windows.Forms.Label lblRepeatsNumber;
        private System.Windows.Forms.TextBox txtRepeatsNumber;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnPlayMidi;
        private System.Windows.Forms.TextBox txtProgression;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblTonica = new System.Windows.Forms.Label();
            this.cmbBoxTonica = new System.Windows.Forms.ComboBox();
            this.lblScale = new System.Windows.Forms.Label();
            this.cmbBoxScale = new System.Windows.Forms.ComboBox();
            this.lblArpNotesCount = new System.Windows.Forms.Label();
            this.txtArpNotesCount = new System.Windows.Forms.TextBox();
            this.lblTactsNumber = new System.Windows.Forms.Label();
            this.txtTactsNumber = new System.Windows.Forms.TextBox();
            this.lblRepeatsNumber = new System.Windows.Forms.Label();
            this.txtRepeatsNumber = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnPlayMidi = new System.Windows.Forms.Button();
            this.txtProgression = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblTonica
            // 
            this.lblTonica.AutoSize = true;
            this.lblTonica.Location = new System.Drawing.Point(12, 15);
            this.lblTonica.Name = "lblTonica";
            this.lblTonica.Size = new System.Drawing.Size(44, 13);
            this.lblTonica.TabIndex = 0;
            this.lblTonica.Text = "Тоника";
            // 
            // cmbBoxTonica
            // 
            this.cmbBoxTonica.BackColor = System.Drawing.SystemColors.Info;
            this.cmbBoxTonica.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxTonica.FormattingEnabled = true;
            this.cmbBoxTonica.Location = new System.Drawing.Point(113, 12);
            this.cmbBoxTonica.Name = "cmbBoxTonica";
            this.cmbBoxTonica.Size = new System.Drawing.Size(121, 21);
            this.cmbBoxTonica.TabIndex = 1;
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(12, 42);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(27, 13);
            this.lblScale.TabIndex = 2;
            this.lblScale.Text = "Лад";
            // 
            // cmbBoxScale
            // 
            this.cmbBoxScale.BackColor = System.Drawing.SystemColors.Info;
            this.cmbBoxScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxScale.FormattingEnabled = true;
            this.cmbBoxScale.Location = new System.Drawing.Point(113, 39);
            this.cmbBoxScale.Name = "cmbBoxScale";
            this.cmbBoxScale.Size = new System.Drawing.Size(121, 21);
            this.cmbBoxScale.TabIndex = 3;
            // 
            // lblArpNotesCount
            // 
            this.lblArpNotesCount.AutoSize = true;
            this.lblArpNotesCount.Location = new System.Drawing.Point(12, 69);
            this.lblArpNotesCount.Name = "lblArpNotesCount";
            this.lblArpNotesCount.Size = new System.Drawing.Size(61, 13);
            this.lblArpNotesCount.TabIndex = 4;
            this.lblArpNotesCount.Text = "Кол-во нот";
            // 
            // txtArpNotesCount
            // 
            this.txtArpNotesCount.BackColor = System.Drawing.Color.LightYellow;
            this.txtArpNotesCount.Location = new System.Drawing.Point(113, 66);
            this.txtArpNotesCount.Name = "txtArpNotesCount";
            this.txtArpNotesCount.Size = new System.Drawing.Size(121, 20);
            this.txtArpNotesCount.TabIndex = 5;
            this.txtArpNotesCount.Text = "4";
            this.txtArpNotesCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumeric_KeyPress);
            // 
            // lblTactsNumber
            // 
            this.lblTactsNumber.AutoSize = true;
            this.lblTactsNumber.Location = new System.Drawing.Point(12, 95);
            this.lblTactsNumber.Name = "lblTactsNumber";
            this.lblTactsNumber.Size = new System.Drawing.Size(78, 13);
            this.lblTactsNumber.TabIndex = 6;
            this.lblTactsNumber.Text = "Кол-во тактов";
            // 
            // txtTactsNumber
            // 
            this.txtTactsNumber.BackColor = System.Drawing.Color.LightYellow;
            this.txtTactsNumber.Location = new System.Drawing.Point(113, 92);
            this.txtTactsNumber.Name = "txtTactsNumber";
            this.txtTactsNumber.Size = new System.Drawing.Size(121, 20);
            this.txtTactsNumber.TabIndex = 7;
            this.txtTactsNumber.Text = "4";
            this.txtTactsNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumeric_KeyPress);
            // 
            // lblRepeatsNumber
            // 
            this.lblRepeatsNumber.AutoSize = true;
            this.lblRepeatsNumber.Location = new System.Drawing.Point(12, 121);
            this.lblRepeatsNumber.Name = "lblRepeatsNumber";
            this.lblRepeatsNumber.Size = new System.Drawing.Size(91, 13);
            this.lblRepeatsNumber.TabIndex = 8;
            this.lblRepeatsNumber.Text = "Кол-во повторов";
            // 
            // txtRepeatsNumber
            // 
            this.txtRepeatsNumber.BackColor = System.Drawing.Color.LightYellow;
            this.txtRepeatsNumber.Location = new System.Drawing.Point(113, 118);
            this.txtRepeatsNumber.Name = "txtRepeatsNumber";
            this.txtRepeatsNumber.Size = new System.Drawing.Size(121, 20);
            this.txtRepeatsNumber.TabIndex = 9;
            this.txtRepeatsNumber.Text = "0";
            this.txtRepeatsNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumeric_KeyPress);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(12, 144);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(222, 42);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "Генерировать";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnPlayMidi
            // 
            this.btnPlayMidi.Location = new System.Drawing.Point(240, 12);
            this.btnPlayMidi.Name = "btnPlayMidi";
            this.btnPlayMidi.Size = new System.Drawing.Size(299, 23);
            this.btnPlayMidi.TabIndex = 11;
            this.btnPlayMidi.Text = "Воспроизвести MIDI";
            this.btnPlayMidi.UseVisualStyleBackColor = true;
            // 
            // txtProgression
            // 
            this.txtProgression.BackColor = System.Drawing.Color.LightYellow;
            this.txtProgression.Location = new System.Drawing.Point(240, 39);
            this.txtProgression.Multiline = true;
            this.txtProgression.Name = "txtProgression";
            this.txtProgression.ReadOnly = true;
            this.txtProgression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtProgression.Size = new System.Drawing.Size(299, 147);
            this.txtProgression.TabIndex = 12;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(548, 198);
            this.Controls.Add(this.txtProgression);
            this.Controls.Add(this.btnPlayMidi);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtRepeatsNumber);
            this.Controls.Add(this.lblRepeatsNumber);
            this.Controls.Add(this.txtTactsNumber);
            this.Controls.Add(this.lblTactsNumber);
            this.Controls.Add(this.txtArpNotesCount);
            this.Controls.Add(this.lblArpNotesCount);
            this.Controls.Add(this.cmbBoxScale);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.cmbBoxTonica);
            this.Controls.Add(this.lblTonica);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Arpeggio Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
