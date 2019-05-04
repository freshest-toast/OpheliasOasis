using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mime;
using System.IO;

namespace OpheliasOasis
{
    public partial class AccomdationBills : Form
    {
        public AccomdationBills()
        {
            InitializeComponent();

           
        }

        private void searchGuestBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string guestName = guestNameBox.Text;
                DateTime initialDateVal = fromDate.Value;
                DateTime endDateVal = toDate.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void printBillBtn_Click(object sender, EventArgs e)
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

                DatabaseManager.AccomodationBill bills = DatabaseManager.getAccomodationBill(reservationId);
                DataTable table = new DataTable("Accomodation Bill");
                table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("Date Printed", typeof(DateTime)),
                        new DataColumn("Guest Name", typeof(string)),
                        new DataColumn("Room Number", typeof(int)),
                        new DataColumn("Arrival Date", typeof(DateTime)),
                        new DataColumn("Departure Date", typeof(DateTime)),
                        new DataColumn("Number of Nights", typeof(int)),
                        new DataColumn("Total Charge", typeof(decimal)),
                        new DataColumn("Is this 60 Days in Advance?", typeof(bool)),
                        new DataColumn("Date Paid in Advance", typeof(string)),
                        new DataColumn("Amount Paid", typeof(decimal))

                });

                table.Rows.Add(bills.datePrinted, bills.guestName, bills.roomNumber, bills.arrivalDate, bills.depatureDate, bills.numberOfNights, bills.totalCharge, bills.isPrepaidOr60Day, bills.datePaidInAdvance == DateTime.MinValue ? "" : bills.datePaidInAdvance.ToString(), bills.amountPaid);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "Accomodation Bill";
                saveFileDialog.Filter = "Comma Separated Values (.csv)|*.csv";
                saveFileDialog.Title = "Save Report As";
                var cancel = saveFileDialog.ShowDialog();

                if(cancel == DialogResult.Cancel || string.IsNullOrWhiteSpace(saveFileDialog.FileName) )
                {
                    return;
                }

                string filePath = saveFileDialog.FileName;

                StringBuilder input = new StringBuilder();

                string[] colNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                input.AppendLine(string.Join(",", colNames));

                foreach(DataRow datarow in table.Rows)
                {
                    string[] fields = datarow.ItemArray.Select(field => field.ToString()).ToArray();
                    input.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(filePath, input.ToString());

                this.Name = "Successful";

                string guestName = guestNameBox.Text;
                DateTime initialDateVal = fromDate.Value;
                DateTime endDateVal = toDate.Value;
                reservationDataGrid.DataSource = DatabaseManager.findReservation(guestName, initialDateVal, endDateVal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
