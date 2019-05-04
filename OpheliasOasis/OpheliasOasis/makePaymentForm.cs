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
    public partial class makePaymentForm : Form
    {
        string reservationID; 
        public makePaymentForm(string id, decimal cost)
        {
            InitializeComponent();
            amountDueBox.Text = cost.ToString();
            amountDueBox.ReadOnly = true;
            reservationID = id; 
        }

        private void makePaymentForm_Load(object sender, EventArgs e)
        {

        }

        private void makePaymentBtn_Click(object sender, EventArgs e)
        {
            decimal amount = Convert.ToDecimal(amountDueBox.Text);

            try
            {
                DatabaseManager.makePayment(reservationID);
                MessageBox.Show("Payment Made Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
