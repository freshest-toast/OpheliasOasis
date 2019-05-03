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
    public partial class ChangeBaseRateForm : Form
    {
        public ChangeBaseRateForm()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void changeRateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                decimal newRate = Convert.ToDecimal(baseRateBox.Text);
                DateTime startDate = fromDate.Value;
                DateTime endDate = toDate.Value;

                DatabaseManager.changeRate(startDate, endDate, newRate);
                MessageBox.Show("Base Rate changed successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
