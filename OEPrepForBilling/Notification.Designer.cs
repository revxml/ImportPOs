namespace OEPrepForBilling
{
    partial class Notification
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
            this.scNotification = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmbNotification = new System.Windows.Forms.ToolStripMenuItem();
            this.txtNotification = new System.Windows.Forms.TextBox();
            this.scNotification.Panel1.SuspendLayout();
            this.scNotification.Panel2.SuspendLayout();
            this.scNotification.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scNotification
            // 
            this.scNotification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scNotification.Location = new System.Drawing.Point(0, 0);
            this.scNotification.Name = "scNotification";
            this.scNotification.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scNotification.Panel1
            // 
            this.scNotification.Panel1.Controls.Add(this.menuStrip1);
            // 
            // scNotification.Panel2
            // 
            this.scNotification.Panel2.Controls.Add(this.txtNotification);
            this.scNotification.Size = new System.Drawing.Size(499, 321);
            this.scNotification.SplitterDistance = 29;
            this.scNotification.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmbNotification});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(499, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmbNotification
            // 
            this.tsmbNotification.Name = "tsmbNotification";
            this.tsmbNotification.Size = new System.Drawing.Size(53, 23);
            this.tsmbNotification.Text = "Copy";
            this.tsmbNotification.Click += new System.EventHandler(this.tsmbNotification_Click);
            // 
            // txtNotification
            // 
            this.txtNotification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotification.Location = new System.Drawing.Point(0, 0);
            this.txtNotification.Multiline = true;
            this.txtNotification.Name = "txtNotification";
            this.txtNotification.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtNotification.Size = new System.Drawing.Size(499, 288);
            this.txtNotification.TabIndex = 0;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 321);
            this.Controls.Add(this.scNotification);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Notification";
            this.Text = "Notification";
            this.Leave += new System.EventHandler(this.Notification_Leave);
            this.scNotification.Panel1.ResumeLayout(false);
            this.scNotification.Panel1.PerformLayout();
            this.scNotification.Panel2.ResumeLayout(false);
            this.scNotification.Panel2.PerformLayout();
            this.scNotification.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scNotification;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmbNotification;
        private System.Windows.Forms.TextBox txtNotification;
    }
}