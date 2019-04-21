namespace OpheliasOasis
{
    partial class SystemAdminTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemAdminTools));
            this.createBackupsBtn = new System.Windows.Forms.Button();
            this.restoreBackupBtn = new System.Windows.Forms.Button();
            this.addRemoveEmployeeBtn = new System.Windows.Forms.Button();
            this.modifyEmployeeBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // createBackupsBtn
            // 
            this.createBackupsBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createBackupsBtn.Location = new System.Drawing.Point(304, 88);
            this.createBackupsBtn.Name = "createBackupsBtn";
            this.createBackupsBtn.Size = new System.Drawing.Size(185, 61);
            this.createBackupsBtn.TabIndex = 0;
            this.createBackupsBtn.Text = "Create Backup";
            this.createBackupsBtn.UseVisualStyleBackColor = true;
            // 
            // restoreBackupBtn
            // 
            this.restoreBackupBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restoreBackupBtn.Location = new System.Drawing.Point(304, 176);
            this.restoreBackupBtn.Name = "restoreBackupBtn";
            this.restoreBackupBtn.Size = new System.Drawing.Size(185, 61);
            this.restoreBackupBtn.TabIndex = 1;
            this.restoreBackupBtn.Text = "Restore Backup";
            this.restoreBackupBtn.UseVisualStyleBackColor = true;
            // 
            // addRemoveEmployeeBtn
            // 
            this.addRemoveEmployeeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addRemoveEmployeeBtn.Location = new System.Drawing.Point(304, 265);
            this.addRemoveEmployeeBtn.Name = "addRemoveEmployeeBtn";
            this.addRemoveEmployeeBtn.Size = new System.Drawing.Size(185, 61);
            this.addRemoveEmployeeBtn.TabIndex = 2;
            this.addRemoveEmployeeBtn.Text = "Add/Remove Employee";
            this.addRemoveEmployeeBtn.UseVisualStyleBackColor = true;
            // 
            // modifyEmployeeBtn
            // 
            this.modifyEmployeeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modifyEmployeeBtn.Location = new System.Drawing.Point(304, 356);
            this.modifyEmployeeBtn.Name = "modifyEmployeeBtn";
            this.modifyEmployeeBtn.Size = new System.Drawing.Size(185, 61);
            this.modifyEmployeeBtn.TabIndex = 3;
            this.modifyEmployeeBtn.Text = "Modify Employee";
            this.modifyEmployeeBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(264, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "System Administrator Tools";
            // 
            // SystemAdminTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.modifyEmployeeBtn);
            this.Controls.Add(this.addRemoveEmployeeBtn);
            this.Controls.Add(this.restoreBackupBtn);
            this.Controls.Add(this.createBackupsBtn);
            this.DoubleBuffered = true;
            this.Name = "SystemAdminTools";
            this.Text = "SystemAdminTools";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button createBackupsBtn;
        private System.Windows.Forms.Button restoreBackupBtn;
        private System.Windows.Forms.Button addRemoveEmployeeBtn;
        private System.Windows.Forms.Button modifyEmployeeBtn;
        private System.Windows.Forms.Label label1;
    }
}