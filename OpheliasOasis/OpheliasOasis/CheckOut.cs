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
    public partial class CheckOut : Form
    {
        public CheckOut()
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
                DateTime initialDateVal = filterStartDate.Value;
                DateTime endDateVal = filterEndDate.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void checkOutBtn_Click(object sender, EventArgs e)
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

                DatabaseManager.checkOut(reservationId);
                MessageBox.Show("Check Out Successful");


                string guestName = guestNameBox.Text;
                DateTime initialDateVal = filterStartDate.Value;
                DateTime endDateVal = filterEndDate.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
