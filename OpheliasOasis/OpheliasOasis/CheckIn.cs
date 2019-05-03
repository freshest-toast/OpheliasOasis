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
    public partial class CheckIn : Form
    {
        public CheckIn()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, EventArgs e)
        {

        }

        private void backBtn_Click_1(object sender, EventArgs e)
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
                guestDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void checkInBtn_Click(object sender, EventArgs e)
        {
            if (guestDataGrid.SelectedRows.Count <= 0)
            {
                return;

            }

            int row = (int)guestDataGrid.SelectedRows[0].Index;
            DataTable tbl = (DataTable)guestDataGrid.DataSource;
            if (row < 0 || row >= tbl.Rows.Count)
            {
                return;
            }

            string reservationId = tbl.Rows[row].ItemArray[0].ToString();
            try
            {

                DatabaseManager.checkIn(reservationId);
                MessageBox.Show("Check In Successful");


                string guestName = guestNameBox.Text;
                DateTime initialDateVal = filterStartDate.Value;
                DateTime endDateVal = filterEndDate.Value;
                guestDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
