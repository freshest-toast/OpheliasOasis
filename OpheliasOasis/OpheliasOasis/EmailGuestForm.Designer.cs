namespace OpheliasOasis
{
    partial class EmailGuestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmailGuestForm));
            this.emailGrid = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.emailGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // emailGrid
            // 
            this.emailGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.emailGrid.Location = new System.Drawing.Point(74, 123);
            this.emailGrid.Name = "emailGrid";
            this.emailGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.emailGrid.Size = new System.Drawing.Size(643, 218);
            this.emailGrid.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(291, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 31);
            this.label2.TabIndex = 16;
            this.label2.Text = "Email Guests";
            // 
            // EmailGuestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.emailGrid);
            this.DoubleBuffered = true;
            this.Name = "EmailGuestForm";
            this.Text = "Email Guest Form";
            ((System.ComponentModel.ISupportInitialize)(this.emailGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView emailGrid;
        private System.Windows.Forms.Label label2;
    }
}