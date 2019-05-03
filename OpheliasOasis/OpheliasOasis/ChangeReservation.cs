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
    public partial class ChangeReservation : Form
    {
        public ChangeReservation()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchGuestBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string guestName = guestNameBox.Text;
                DateTime initialDateVal = filterStartDateBtn.Value;
                DateTime endDateVal = filterEndDateBtn.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void updateReservationBtn_Click(object sender, EventArgs e)
        {
            if (reservationDataGrid.SelectedRows.Count <= 0)
            {
                return;

            }

            int row = (int)reservationDataGrid.SelectedRows[0].Index;
            DataTable tbl = (DataTable)reservationDataGrid.DataSource;
            if (row < 0 || row >= tbl.Rows.Count)
            {
                return;
            }

            string reservationId = tbl.Rows[row].ItemArray[0].ToString();
            try
            {
                DateTime startDate = changeCheckIn.Value;
                DateTime endDate = changeCheckOut.Value;
                string firstName = changeFirstBox.Text;
                string lastName = changeLastBox.Text;
                string email = changeEmailBox.Text;

                decimal payment = DatabaseManager.changeReservation(reservationId,startDate, endDate, firstName, lastName, email );

                if (payment > 0)
                {
                    makePaymentForm make = new makePaymentForm(reservationId, payment);
                    make.ShowDialog();
                }

                MessageBox.Show("Change Reservation Successful");


                string guestName = guestNameBox.Text;
                DateTime initialDateVal = filterStartDateBtn.Value;
                DateTime endDateVal = filterEndDateBtn.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       
    }
}
