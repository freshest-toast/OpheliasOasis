using Microsoft.Reporting.WinForms;

namespace OpheliasOasis
{
    partial class GenerateExpectedIncomeReport
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            //this.components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(800, 450);
            //this.Text = "GenerateExpectedIncomeReport";

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
            this.Name = "ExpectedIncomeReport";
            this.Text = "Expected Income Report";
            this.Load += new System.EventHandler(this.GenerateExpectedIncomeReport_Load);
            this.ResumeLayout(false);

            // get data from the database
            LocalReport localReport = reportViewer1.LocalReport;
//            this.reportViewer1.LocalReport.DataSources.Add();
//            this.reportViewer1.LocalReport.ReportEmbeddedResource = "";






            // Get data from the database
            ReportDataSource a = new ReportDataSource();
            a.Name = "ExpectedIncomeReport";
            a.Value = ""; // This should be a table.  So the value of the report is the filtered table
            // pass value I get from the function


            // TODO if I get a table returned, I can just output the table onto the report window





            // 

        }

         
        #endregion

    }
}