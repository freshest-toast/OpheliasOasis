using Microsoft.Reporting.WinForms;

namespace OpheliasOasis
{
    partial class GenerateDailyArrivalsReport
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
            //this.components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(800, 450);
            //this.Text = "GenerateDailyArrivalsReport";
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(1726, 734);
            this.reportViewer1.TabIndex = 0;
            // 
            // IncentiveReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1727, 735);
            this.Controls.Add(this.reportViewer1);
            this.Name = "DailyArrivalsReport";
            this.Text = "Daily Arrivals Report";
            this.Load += new System.EventHandler(this.DailyArrivalsReport_Load);
            this.ResumeLayout(false);

            LocalReport localReport = reportViewer1.LocalReport;

            ReportDataSource a = new ReportDataSource();
            a.Name = "DailyArrivalsReport";
            a.Value = ""; // This should be a table.  So the value of the report is the filtered table


        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}