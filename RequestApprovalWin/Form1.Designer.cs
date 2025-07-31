namespace RequestApprovalWin
{
    partial class Form1
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
            this.dgvRequests = new System.Windows.Forms.DataGridView();
            this.btnLoadRequests = new System.Windows.Forms.Button();
            this.txtRequesterName = new System.Windows.Forms.TextBox();
            this.dtpRequestDate = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRequests
            // 
            this.dgvRequests.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dgvRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRequests.Location = new System.Drawing.Point(8, 230);
            this.dgvRequests.Name = "dgvRequests";
            this.dgvRequests.Size = new System.Drawing.Size(1904, 799);
            this.dgvRequests.TabIndex = 0;
            // 
            // btnLoadRequests
            // 
            this.btnLoadRequests.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnLoadRequests.Location = new System.Drawing.Point(852, 147);
            this.btnLoadRequests.Name = "btnLoadRequests";
            this.btnLoadRequests.Size = new System.Drawing.Size(200, 55);
            this.btnLoadRequests.TabIndex = 1;
            this.btnLoadRequests.Text = "Bekleyen Talepleri Yükle";
            this.btnLoadRequests.UseVisualStyleBackColor = true;
            this.btnLoadRequests.Click += new System.EventHandler(this.btnLoadRequests_Click);
            // 
            // txtRequesterName
            // 
            this.txtRequesterName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtRequesterName.Location = new System.Drawing.Point(852, 119);
            this.txtRequesterName.Name = "txtRequesterName";
            this.txtRequesterName.Size = new System.Drawing.Size(200, 20);
            this.txtRequesterName.TabIndex = 4;
            // 
            // dtpRequestDate
            // 
            this.dtpRequestDate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dtpRequestDate.Location = new System.Drawing.Point(852, 91);
            this.dtpRequestDate.Name = "dtpRequestDate";
            this.dtpRequestDate.Size = new System.Drawing.Size(200, 20);
            this.dtpRequestDate.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.dtpRequestDate);
            this.Controls.Add(this.txtRequesterName);
            this.Controls.Add(this.btnLoadRequests);
            this.Controls.Add(this.dgvRequests);
            this.Name = "Form1";
            this.Text = "Momo Reservation Requests";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRequests;
        private System.Windows.Forms.Button btnLoadRequests;
        private System.Windows.Forms.TextBox txtRequesterName;
        private System.Windows.Forms.DateTimePicker dtpRequestDate;
    }
}

