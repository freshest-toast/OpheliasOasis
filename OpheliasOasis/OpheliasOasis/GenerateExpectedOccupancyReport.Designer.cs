using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace OpheliasOasis
{
    partial class GenerateExpectedOccupancyReport
    {
        private System.ComponentModel.IContainer components = null;
        private ReportViewer reportViewer1;
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
            this.reportViewer1 = new ReportViewer();
            this.SuspendLayout();
            ReportDataSource a = new ReportDataSource();
            a.Name = "ExpectedIncomeReport";
            a.Value = ""; // This should be a table.  So the value of the report is the filtered table
            // pass value I get from the function

            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(1726, 734);
            this.reportViewer1.TabIndex = 0;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(a); // set the report to have the values of the datasource

            // create columns
            DataColumn col1 = new DataColumn();
            DataColumn col2 = new DataColumn();
            DataColumn col3 = new DataColumn();
            DataColumn col4 = new DataColumn();
            DataColumn col5 = new DataColumn();
            DataColumn col6 = new DataColumn();

            // set the values and properties of the columns
            col1.ColumnName = "Date";
            col2.ColumnName = "Prepaid Reservations";
            col3.ColumnName = "60 Day Reservations";
            col4.ColumnName = "Conventional Reservations";
            col5.ColumnName = "Incentive Reservations";
            col6.ColumnName = "Total Rooms Reserved";

            DataTable test = new DataTable();
            test.Columns.Add(col1);



            // sets the properties of the window 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1727, 735);
            this.Controls.Add(this.reportViewer1);
            this.Name = "GenerateExpectedOccupancyReport";
            this.Text = "Expected Occupancy Report";
            this.Load += new System.EventHandler(this.ExpectedOccupancyReport_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}