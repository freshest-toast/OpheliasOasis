using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpheliasOasis
{
    public partial class GenerateExpectedIncomeReport : Form
    {
        public GenerateExpectedIncomeReport()
        {
            InitializeComponent();
        }

        private void GenerateExpectedIncomeReport_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // GenerateExpectedIncomeReport
        //    // 
        //    this.ClientSize = new System.Drawing.Size(284, 261);
        //    this.Name = "GenerateExpectedIncomeReport";
        //    this.Shown += new System.EventHandler(this.GenerateExpectedIncomeReport_Shown);
        //    this.ResumeLayout(false);

        //}

        //private void GenerateExpectedIncomeReport_Shown(object sender, EventArgs e)
        //{

        //}
    }
}
