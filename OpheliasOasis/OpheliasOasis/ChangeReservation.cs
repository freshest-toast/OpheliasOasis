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

        }

       
    }
}
