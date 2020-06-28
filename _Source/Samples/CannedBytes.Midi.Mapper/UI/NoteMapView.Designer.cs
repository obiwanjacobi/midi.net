namespace CannedBytes.Midi.Mapper.UI
{
    partial class NoteMapView
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
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader3;
            System.Windows.Forms.ColumnHeader columnHeader4;
            System.Windows.Forms.ColumnHeader columnHeader5;
            this.MapItems = new System.Windows.Forms.ListView();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 90;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Note In";
            columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Note In #";
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Note Out";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Note Out #";
            columnHeader5.Width = 70;
            // 
            // MapItems
            // 
            this.MapItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3,
            columnHeader4,
            columnHeader5});
            this.MapItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapItems.FullRowSelect = true;
            this.MapItems.Location = new System.Drawing.Point(0, 0);
            this.MapItems.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MapItems.MultiSelect = false;
            this.MapItems.Name = "MapItems";
            this.MapItems.Size = new System.Drawing.Size(445, 69);
            this.MapItems.TabIndex = 0;
            this.MapItems.UseCompatibleStateImageBehavior = false;
            this.MapItems.View = System.Windows.Forms.View.Details;
            this.MapItems.SelectedIndexChanged += new System.EventHandler(this.MapItems_SelectedIndexChanged);
            this.MapItems.DoubleClick += new System.EventHandler(this.MapItems_DoubleClick);
            this.MapItems.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MapItems_KeyUp);
            // 
            // NoteMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MapItems);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "NoteMapView";
            this.Size = new System.Drawing.Size(445, 69);
            this.ResumeLayout(false);

        }

        #endregion Component Designer generated code

        private System.Windows.Forms.ListView MapItems;
    }
}