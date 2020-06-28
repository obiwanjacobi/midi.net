namespace CannedBytes.Midi.Mapper.UI
{
    partial class MapItemForm
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.label1 = new System.Windows.Forms.Label();
            this.NoteInNumberCtrl = new CannedBytes.Midi.Mapper.UI.NoteNumberControl();
            this.NoteOutNumberCtrl = new CannedBytes.Midi.Mapper.UI.NoteNumberControl();
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.MapItemName = new System.Windows.Forms.TextBox();
            this.HexChk = new System.Windows.Forms.CheckBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(17, 121);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 17);
            label2.TabIndex = 2;
            label2.Text = "Note Out";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 11);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(45, 17);
            label3.TabIndex = 6;
            label3.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Note In";
            // 
            // NoteInNumberCtrl
            // 
            this.NoteInNumberCtrl.Hexadecimal = false;
            this.NoteInNumberCtrl.Location = new System.Drawing.Point(17, 86);
            this.NoteInNumberCtrl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.NoteInNumberCtrl.Name = "NoteInNumberCtrl";
            this.NoteInNumberCtrl.NoteName = "C0";
            this.NoteInNumberCtrl.NoteNumber = 0;
            this.NoteInNumberCtrl.Size = new System.Drawing.Size(240, 26);
            this.NoteInNumberCtrl.TabIndex = 1;
            // 
            // NoteOutNumberCtrl
            // 
            this.NoteOutNumberCtrl.Hexadecimal = false;
            this.NoteOutNumberCtrl.Location = new System.Drawing.Point(17, 142);
            this.NoteOutNumberCtrl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.NoteOutNumberCtrl.Name = "NoteOutNumberCtrl";
            this.NoteOutNumberCtrl.NoteName = "C0";
            this.NoteOutNumberCtrl.NoteNumber = 0;
            this.NoteOutNumberCtrl.Size = new System.Drawing.Size(240, 26);
            this.NoteOutNumberCtrl.TabIndex = 3;
            // 
            // OkBtn
            // 
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new System.Drawing.Point(273, 16);
            this.OkBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(100, 28);
            this.OkBtn.TabIndex = 4;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(273, 53);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 28);
            this.CancelBtn.TabIndex = 5;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // MapItemName
            // 
            this.MapItemName.Location = new System.Drawing.Point(17, 32);
            this.MapItemName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MapItemName.Name = "MapItemName";
            this.MapItemName.Size = new System.Drawing.Size(239, 22);
            this.MapItemName.TabIndex = 7;
            // 
            // HexChk
            // 
            this.HexChk.AutoSize = true;
            this.HexChk.Location = new System.Drawing.Point(273, 121);
            this.HexChk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.HexChk.Name = "HexChk";
            this.HexChk.Size = new System.Drawing.Size(110, 21);
            this.HexChk.TabIndex = 8;
            this.HexChk.Text = "Hexadecimal";
            this.HexChk.UseVisualStyleBackColor = true;
            this.HexChk.CheckedChanged += new System.EventHandler(this.HexChk_CheckedChanged);
            // 
            // MapItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 191);
            this.Controls.Add(this.HexChk);
            this.Controls.Add(this.MapItemName);
            this.Controls.Add(label3);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.NoteOutNumberCtrl);
            this.Controls.Add(label2);
            this.Controls.Add(this.NoteInNumberCtrl);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapItemForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map Item";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label label1;
        private NoteNumberControl NoteInNumberCtrl;
        private NoteNumberControl NoteOutNumberCtrl;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.TextBox MapItemName;
        private System.Windows.Forms.CheckBox HexChk;
    }
}