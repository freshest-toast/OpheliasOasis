﻿namespace OpheliasOasis
{
    partial class AccomdationBills
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccomdationBills));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.guestNameBox = new System.Windows.Forms.TextBox();
            this.printBillBtn = new System.Windows.Forms.Button();
            this.reservationDataGrid = new System.Windows.Forms.DataGridView();
            this.searchGuestBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.reservationDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(238, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(330, 31);
            this.label2.TabIndex = 14;
            this.label2.Text = "Print Accomodation Bills";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(173, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Guest Name: ";
            // 
            // guestNameBox
            // 
            this.guestNameBox.Location = new System.Drawing.Point(286, 115);
            this.guestNameBox.Name = "guestNameBox";
            this.guestNameBox.Size = new System.Drawing.Size(341, 20);
            this.guestNameBox.TabIndex = 12;
            // 
            // printBillBtn
            // 
            this.printBillBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printBillBtn.Location = new System.Drawing.Point(330, 384);
            this.printBillBtn.Name = "printBillBtn";
            this.printBillBtn.Size = new System.Drawing.Size(151, 40);
            this.printBillBtn.TabIndex = 11;
            this.printBillBtn.Text = "Print Bill";
            this.printBillBtn.UseVisualStyleBackColor = true;
            // 
            // reservationDataGrid
            // 
            this.reservationDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.reservationDataGrid.Location = new System.Drawing.Point(176, 200);
            this.reservationDataGrid.Name = "reservationDataGrid";
            this.reservationDataGrid.Size = new System.Drawing.Size(451, 150);
            this.reservationDataGrid.TabIndex = 10;
            // 
            // searchGuestBtn
            // 
            this.searchGuestBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchGuestBtn.Location = new System.Drawing.Point(330, 154);
            this.searchGuestBtn.Name = "searchGuestBtn";
            this.searchGuestBtn.Size = new System.Drawing.Size(151, 27);
            this.searchGuestBtn.TabIndex = 15;
            this.searchGuestBtn.Text = "Search";
            this.searchGuestBtn.UseVisualStyleBackColor = true;
            // 
            // AccomdationBills
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.searchGuestBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.guestNameBox);
            this.Controls.Add(this.printBillBtn);
            this.Controls.Add(this.reservationDataGrid);
            this.DoubleBuffered = true;
            this.Name = "AccomdationBills";
            this.Text = "Accomodation Bills";
            ((System.ComponentModel.ISupportInitialize)(this.reservationDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox guestNameBox;
        private System.Windows.Forms.Button printBillBtn;
        private System.Windows.Forms.DataGridView reservationDataGrid;
        private System.Windows.Forms.Button searchGuestBtn;
    }
}