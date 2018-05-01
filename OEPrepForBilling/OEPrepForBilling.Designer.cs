namespace OEPrepForBilling
{
    partial class OEPrepForBilling
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.btnManualImport = new System.Windows.Forms.Button();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabConfirmShip = new System.Windows.Forms.TabPage();
            this.scConfirmShip = new System.Windows.Forms.SplitContainer();
            this.btnConfirmShip = new System.Windows.Forms.Button();
            this.btnTransact = new System.Windows.Forms.Button();
            this.gbSelect = new System.Windows.Forms.GroupBox();
            this.cbAdjustQty = new System.Windows.Forms.CheckBox();
            this.cbConfirmShip = new System.Windows.Forms.CheckBox();
            this.dgvShipConfirm = new System.Windows.Forms.DataGridView();
            this.tabSelectForBilling = new System.Windows.Forms.TabPage();
            this.scBilling = new System.Windows.Forms.SplitContainer();
            this.lblInvoiceDate = new System.Windows.Forms.Label();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.btnInvoice = new System.Windows.Forms.Button();
            this.gbSelection = new System.Windows.Forms.GroupBox();
            this.cbInvoice = new System.Windows.Forms.CheckBox();
            this.cbBilling = new System.Windows.Forms.CheckBox();
            this.btnSelectBilling = new System.Windows.Forms.Button();
            this.dgvBilling = new System.Windows.Forms.DataGridView();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabConfirmShip.SuspendLayout();
            this.scConfirmShip.Panel1.SuspendLayout();
            this.scConfirmShip.Panel2.SuspendLayout();
            this.scConfirmShip.SuspendLayout();
            this.gbSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShipConfirm)).BeginInit();
            this.tabSelectForBilling.SuspendLayout();
            this.scBilling.Panel1.SuspendLayout();
            this.scBilling.Panel2.SuspendLayout();
            this.scBilling.SuspendLayout();
            this.gbSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).BeginInit();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.btnManualImport);
            this.scMain.Panel1.Controls.Add(this.lblUserName);
            this.scMain.Panel1.Controls.Add(this.txtUserName);
            this.scMain.Panel1.Controls.Add(this.lblHeader);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.tcMain);
            this.scMain.Size = new System.Drawing.Size(903, 624);
            this.scMain.SplitterDistance = 64;
            this.scMain.TabIndex = 0;
            // 
            // btnManualImport
            // 
            this.btnManualImport.Location = new System.Drawing.Point(477, 21);
            this.btnManualImport.Name = "btnManualImport";
            this.btnManualImport.Size = new System.Drawing.Size(106, 23);
            this.btnManualImport.TabIndex = 3;
            this.btnManualImport.Text = "Manual Import";
            this.btnManualImport.UseVisualStyleBackColor = true;
            this.btnManualImport.Click += new System.EventHandler(this.btnManualImport_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(705, 32);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(73, 15);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "User Name:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(784, 29);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 20);
            this.txtUserName.TabIndex = 1;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17.4717F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 19);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(287, 30);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Order Prep and Billing";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabConfirmShip);
            this.tcMain.Controls.Add(this.tabSelectForBilling);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(903, 556);
            this.tcMain.TabIndex = 0;
            this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
            // 
            // tabConfirmShip
            // 
            this.tabConfirmShip.Controls.Add(this.scConfirmShip);
            this.tabConfirmShip.Location = new System.Drawing.Point(4, 22);
            this.tabConfirmShip.Name = "tabConfirmShip";
            this.tabConfirmShip.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfirmShip.Size = new System.Drawing.Size(895, 530);
            this.tabConfirmShip.TabIndex = 0;
            this.tabConfirmShip.Text = "Cofirm Ship";
            this.tabConfirmShip.UseVisualStyleBackColor = true;
            // 
            // scConfirmShip
            // 
            this.scConfirmShip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scConfirmShip.Location = new System.Drawing.Point(3, 3);
            this.scConfirmShip.Name = "scConfirmShip";
            this.scConfirmShip.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scConfirmShip.Panel1
            // 
            this.scConfirmShip.Panel1.Controls.Add(this.btnConfirmShip);
            this.scConfirmShip.Panel1.Controls.Add(this.btnTransact);
            this.scConfirmShip.Panel1.Controls.Add(this.gbSelect);
            // 
            // scConfirmShip.Panel2
            // 
            this.scConfirmShip.Panel2.Controls.Add(this.dgvShipConfirm);
            this.scConfirmShip.Size = new System.Drawing.Size(889, 524);
            this.scConfirmShip.SplitterDistance = 47;
            this.scConfirmShip.TabIndex = 1;
            // 
            // btnConfirmShip
            // 
            this.btnConfirmShip.Location = new System.Drawing.Point(191, 15);
            this.btnConfirmShip.Name = "btnConfirmShip";
            this.btnConfirmShip.Size = new System.Drawing.Size(171, 23);
            this.btnConfirmShip.TabIndex = 4;
            this.btnConfirmShip.Text = "Ship Confirm Selected";
            this.btnConfirmShip.UseVisualStyleBackColor = true;
            this.btnConfirmShip.Click += new System.EventHandler(this.btnConfirmShip_Click);
            // 
            // btnTransact
            // 
            this.btnTransact.Location = new System.Drawing.Point(48, 15);
            this.btnTransact.Name = "btnTransact";
            this.btnTransact.Size = new System.Drawing.Size(123, 23);
            this.btnTransact.TabIndex = 3;
            this.btnTransact.Text = "Transact Selected";
            this.btnTransact.UseVisualStyleBackColor = true;
            this.btnTransact.Click += new System.EventHandler(this.btnTransact_Click);
            // 
            // gbSelect
            // 
            this.gbSelect.Controls.Add(this.cbAdjustQty);
            this.gbSelect.Controls.Add(this.cbConfirmShip);
            this.gbSelect.Dock = System.Windows.Forms.DockStyle.Right;
            this.gbSelect.Location = new System.Drawing.Point(650, 0);
            this.gbSelect.Name = "gbSelect";
            this.gbSelect.Size = new System.Drawing.Size(239, 47);
            this.gbSelect.TabIndex = 2;
            this.gbSelect.TabStop = false;
            this.gbSelect.Text = "Select All";
            // 
            // cbAdjustQty
            // 
            this.cbAdjustQty.AutoSize = true;
            this.cbAdjustQty.Location = new System.Drawing.Point(19, 16);
            this.cbAdjustQty.Name = "cbAdjustQty";
            this.cbAdjustQty.Size = new System.Drawing.Size(106, 19);
            this.cbAdjustQty.TabIndex = 0;
            this.cbAdjustQty.Text = "Adjust Quantity";
            this.cbAdjustQty.UseVisualStyleBackColor = true;
            this.cbAdjustQty.CheckedChanged += new System.EventHandler(this.cbAdjustQty_CheckedChanged);
            // 
            // cbConfirmShip
            // 
            this.cbConfirmShip.AutoSize = true;
            this.cbConfirmShip.Location = new System.Drawing.Point(131, 16);
            this.cbConfirmShip.Name = "cbConfirmShip";
            this.cbConfirmShip.Size = new System.Drawing.Size(97, 19);
            this.cbConfirmShip.TabIndex = 1;
            this.cbConfirmShip.Text = "Confirm Ship";
            this.cbConfirmShip.UseVisualStyleBackColor = true;
            this.cbConfirmShip.CheckedChanged += new System.EventHandler(this.cbConfirmShip_CheckedChanged);
            // 
            // dgvShipConfirm
            // 
            this.dgvShipConfirm.AllowUserToAddRows = false;
            this.dgvShipConfirm.AllowUserToDeleteRows = false;
            this.dgvShipConfirm.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShipConfirm.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvShipConfirm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvShipConfirm.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvShipConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvShipConfirm.Location = new System.Drawing.Point(0, 0);
            this.dgvShipConfirm.Name = "dgvShipConfirm";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShipConfirm.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvShipConfirm.Size = new System.Drawing.Size(889, 473);
            this.dgvShipConfirm.TabIndex = 0;
            this.dgvShipConfirm.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvShipConfirm_RowPrePaint);
            // 
            // tabSelectForBilling
            // 
            this.tabSelectForBilling.Controls.Add(this.scBilling);
            this.tabSelectForBilling.Location = new System.Drawing.Point(4, 22);
            this.tabSelectForBilling.Name = "tabSelectForBilling";
            this.tabSelectForBilling.Size = new System.Drawing.Size(895, 530);
            this.tabSelectForBilling.TabIndex = 4;
            this.tabSelectForBilling.Text = "Select for Billing";
            this.tabSelectForBilling.UseVisualStyleBackColor = true;
            // 
            // scBilling
            // 
            this.scBilling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scBilling.Location = new System.Drawing.Point(0, 0);
            this.scBilling.Name = "scBilling";
            this.scBilling.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scBilling.Panel1
            // 
            this.scBilling.Panel1.Controls.Add(this.lblInvoiceDate);
            this.scBilling.Panel1.Controls.Add(this.dtpInvoiceDate);
            this.scBilling.Panel1.Controls.Add(this.btnInvoice);
            this.scBilling.Panel1.Controls.Add(this.gbSelection);
            this.scBilling.Panel1.Controls.Add(this.btnSelectBilling);
            // 
            // scBilling.Panel2
            // 
            this.scBilling.Panel2.Controls.Add(this.dgvBilling);
            this.scBilling.Size = new System.Drawing.Size(895, 530);
            this.scBilling.SplitterDistance = 47;
            this.scBilling.TabIndex = 0;
            // 
            // lblInvoiceDate
            // 
            this.lblInvoiceDate.AutoSize = true;
            this.lblInvoiceDate.Location = new System.Drawing.Point(285, 19);
            this.lblInvoiceDate.Name = "lblInvoiceDate";
            this.lblInvoiceDate.Size = new System.Drawing.Size(77, 15);
            this.lblInvoiceDate.TabIndex = 4;
            this.lblInvoiceDate.Text = "Invoice Date:";
            // 
            // dtpInvoiceDate
            // 
            this.dtpInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(368, 16);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(89, 20);
            this.dtpInvoiceDate.TabIndex = 3;
            // 
            // btnInvoice
            // 
            this.btnInvoice.Location = new System.Drawing.Point(191, 15);
            this.btnInvoice.Name = "btnInvoice";
            this.btnInvoice.Size = new System.Drawing.Size(75, 23);
            this.btnInvoice.TabIndex = 2;
            this.btnInvoice.Text = "Invoice";
            this.btnInvoice.UseVisualStyleBackColor = true;
            this.btnInvoice.Click += new System.EventHandler(this.btnInvoice_Click);
            // 
            // gbSelection
            // 
            this.gbSelection.Controls.Add(this.cbInvoice);
            this.gbSelection.Controls.Add(this.cbBilling);
            this.gbSelection.Dock = System.Windows.Forms.DockStyle.Right;
            this.gbSelection.Location = new System.Drawing.Point(656, 0);
            this.gbSelection.Name = "gbSelection";
            this.gbSelection.Size = new System.Drawing.Size(239, 47);
            this.gbSelection.TabIndex = 1;
            this.gbSelection.TabStop = false;
            this.gbSelection.Text = "Selelct All";
            // 
            // cbInvoice
            // 
            this.cbInvoice.AutoSize = true;
            this.cbInvoice.Location = new System.Drawing.Point(160, 17);
            this.cbInvoice.Name = "cbInvoice";
            this.cbInvoice.Size = new System.Drawing.Size(64, 19);
            this.cbInvoice.TabIndex = 1;
            this.cbInvoice.Text = "Invoice";
            this.cbInvoice.UseVisualStyleBackColor = true;
            this.cbInvoice.CheckedChanged += new System.EventHandler(this.cbInvoice_CheckedChanged);
            // 
            // cbBilling
            // 
            this.cbBilling.AutoSize = true;
            this.cbBilling.Location = new System.Drawing.Point(26, 17);
            this.cbBilling.Name = "cbBilling";
            this.cbBilling.Size = new System.Drawing.Size(114, 19);
            this.cbBilling.TabIndex = 0;
            this.cbBilling.Text = "Select for Billing";
            this.cbBilling.UseVisualStyleBackColor = true;
            this.cbBilling.CheckedChanged += new System.EventHandler(this.cbBilling_CheckedChanged);
            // 
            // btnSelectBilling
            // 
            this.btnSelectBilling.Location = new System.Drawing.Point(48, 15);
            this.btnSelectBilling.Name = "btnSelectBilling";
            this.btnSelectBilling.Size = new System.Drawing.Size(128, 23);
            this.btnSelectBilling.TabIndex = 0;
            this.btnSelectBilling.Text = "Select For Billing";
            this.btnSelectBilling.UseVisualStyleBackColor = true;
            this.btnSelectBilling.Click += new System.EventHandler(this.btnSelectBilling_Click);
            // 
            // dgvBilling
            // 
            this.dgvBilling.AllowUserToAddRows = false;
            this.dgvBilling.AllowUserToDeleteRows = false;
            this.dgvBilling.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBilling.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvBilling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBilling.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvBilling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBilling.Location = new System.Drawing.Point(0, 0);
            this.dgvBilling.Name = "dgvBilling";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBilling.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvBilling.Size = new System.Drawing.Size(895, 479);
            this.dgvBilling.TabIndex = 0;
            // 
            // OEPrepForBilling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 624);
            this.Controls.Add(this.scMain);
            this.Name = "OEPrepForBilling";
            this.Text = "Prep And Billing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OEPrepForBilling_FormClosing);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel1.PerformLayout();
            this.scMain.Panel2.ResumeLayout(false);
            this.scMain.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tabConfirmShip.ResumeLayout(false);
            this.scConfirmShip.Panel1.ResumeLayout(false);
            this.scConfirmShip.Panel2.ResumeLayout(false);
            this.scConfirmShip.ResumeLayout(false);
            this.gbSelect.ResumeLayout(false);
            this.gbSelect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShipConfirm)).EndInit();
            this.tabSelectForBilling.ResumeLayout(false);
            this.scBilling.Panel1.ResumeLayout(false);
            this.scBilling.Panel1.PerformLayout();
            this.scBilling.Panel2.ResumeLayout(false);
            this.scBilling.ResumeLayout(false);
            this.gbSelection.ResumeLayout(false);
            this.gbSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabConfirmShip;
        private System.Windows.Forms.TabPage tabSelectForBilling;
        private System.Windows.Forms.DataGridView dgvShipConfirm;
        private System.Windows.Forms.SplitContainer scConfirmShip;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.GroupBox gbSelect;
        private System.Windows.Forms.CheckBox cbAdjustQty;
        private System.Windows.Forms.CheckBox cbConfirmShip;
        private System.Windows.Forms.Button btnConfirmShip;
        private System.Windows.Forms.Button btnTransact;
        private System.Windows.Forms.SplitContainer scBilling;
        private System.Windows.Forms.GroupBox gbSelection;
        private System.Windows.Forms.CheckBox cbBilling;
        private System.Windows.Forms.Button btnSelectBilling;
        private System.Windows.Forms.DataGridView dgvBilling;
        private System.Windows.Forms.Button btnInvoice;
        private System.Windows.Forms.CheckBox cbInvoice;
        private System.Windows.Forms.Label lblInvoiceDate;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.Button btnManualImport;
    }
}

