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
    public partial class CancelReservation : Form
    {
        public CancelReservation()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string guestName = guestNameBox.Text;
                DateTime initialDateVal = initialDate.Value;
                DateTime endDateVal = endDate.Value;
                reservationGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                
            }
           
        }

        private void cancelReservationBtn_Click(object sender, EventArgs e)
        {
            if (reservationGrid.SelectedRows.Count <= 0)
            {
                return;

            }

            int row = (int)reservationGrid.SelectedRows[0].Index;
            DataTable tbl = (DataTable)reservationGrid.DataSource;
            if (row < 0 || row >= tbl.Rows.Count)
            {
                return;
            }

            string reservationId = tbl.Rows[row].ItemArray[0].ToString();
            try
            {
                
                DatabaseManager.cancelReservation(reservationId);
                MessageBox.Show("Reservation Cancelled Successfully");


                string guestName = guestNameBox.Text;
                DateTime initialDateVal = initialDate.Value;
                DateTime endDateVal = endDate.Value;
                reservationGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
