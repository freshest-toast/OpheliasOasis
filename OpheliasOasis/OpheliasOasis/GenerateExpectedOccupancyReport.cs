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
    public partial class GenerateExpectedOccupancyReport : Form
    {
        public GenerateExpectedOccupancyReport()
        {
            InitializeComponent();
        }

        private void ExpectedOccupancyReport_Load(object sender, EventArgs e)
        {
            // this outputs the report to the window
            this.reportViewer1.RefreshReport();
        }

    }
}
