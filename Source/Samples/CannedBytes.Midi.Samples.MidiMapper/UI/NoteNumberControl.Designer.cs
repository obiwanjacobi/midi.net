namespace CannedBytes.Midi.Samples.MidiMapper.UI
{
    partial class NoteNumberControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            this.NoteNameOctave = new System.Windows.Forms.NumericUpDown();
            this.NoteNameCmb = new System.Windows.Forms.ComboBox();
            this.NoteNo = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NoteNameOctave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteNo)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(105, 3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(14, 13);
            label1.TabIndex = 2;
            label1.Text = "#";
            //
            // NoteNameOctave
            //
            this.NoteNameOctave.Location = new System.Drawing.Point(63, 0);
            this.NoteNameOctave.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.NoteNameOctave.Name = "NoteNameOctave";
            this.NoteNameOctave.Size = new System.Drawing.Size(36, 20);
            this.NoteNameOctave.TabIndex = 1;
            this.NoteNameOctave.ValueChanged += new System.EventHandler(this.NoteNameOctave_ValueChanged);
            //
            // NoteNameCmb
            //
            this.NoteNameCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NoteNameCmb.FormattingEnabled = true;
            this.NoteNameCmb.Items.AddRange(new object[] {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"});
            this.NoteNameCmb.Location = new System.Drawing.Point(0, 0);
            this.NoteNameCmb.Name = "NoteNameCmb";
            this.NoteNameCmb.Size = new System.Drawing.Size(56, 21);
            this.NoteNameCmb.TabIndex = 0;
            this.NoteNameCmb.SelectedIndexChanged += new System.EventHandler(this.NoteNameCmb_SelectedIndexChanged);
            //
            // NoteNo
            //
            this.NoteNo.Location = new System.Drawing.Point(122, 0);
            this.NoteNo.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.NoteNo.Name = "NoteNo";
            this.NoteNo.Size = new System.Drawing.Size(55, 20);
            this.NoteNo.TabIndex = 3;
            this.NoteNo.ValueChanged += new System.EventHandler(this.NoteNo_ValueChanged);
            //
            // NoteNumberControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NoteNo);
            this.Controls.Add(label1);
            this.Controls.Add(this.NoteNameCmb);
            this.Controls.Add(this.NoteNameOctave);
            this.Name = "NoteNumberControl";
            this.Size = new System.Drawing.Size(180, 21);
            ((System.ComponentModel.ISupportInitialize)(this.NoteNameOctave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteNo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Component Designer generated code

        private System.Windows.Forms.NumericUpDown NoteNameOctave;
        private System.Windows.Forms.ComboBox NoteNameCmb;
        private System.Windows.Forms.NumericUpDown NoteNo;
    }
}