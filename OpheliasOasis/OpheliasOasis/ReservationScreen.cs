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
    public partial class ReservationScreen : Form
    {
        public ReservationScreen()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void createReservationBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = checkInDate.Value;
                DateTime endDate = checkOutDate.Value;
                string firstName = firstNameBox.Text;
                string lastName = lastNameBox.Text;
                string emailAddress = emailBox.Text;
                string creditCard = customerCCBox.Text;
                string reservationID;

              decimal payment =  DatabaseManager.addReservation(startDate, endDate, firstName, lastName, emailAddress, creditCard, out reservationID);

                if(payment > 0)
                {
                    makePaymentForm paymentForm = new makePaymentForm(reservationID, payment);
                    paymentForm.ShowDialog();
                }
                MessageBox.Show("Reservation Created Successfully");

                firstNameBox.Text = "";
                lastNameBox.Text = "";
                checkInDate.Text = "";
                checkOutDate.Text = "";
                emailBox.Text = "";
                customerCCBox.Text = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
