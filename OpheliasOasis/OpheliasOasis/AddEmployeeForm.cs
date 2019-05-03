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
    public partial class AddEmployeeForm : Form
    {
        public AddEmployeeForm()
        {
            
            InitializeComponent();
            reservationDataGrid.DataSource = DatabaseManager.getUsers();
        }

        private void addEmployeeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string newUsername = usernameBox.Text;
                string newPassword = passwordBox.Text;
                int newAccessLevel = Convert.ToInt32(accessBox.Text);
                string firstName = firstNameBox.Text;
                string lastName = lastNameBox.Text;
                string ssn = ssnBox.Text;
                int salary = Convert.ToInt32(salaryBox.Text);

                DatabaseManager.addUser(newUsername, newPassword, newAccessLevel, firstName, lastName, ssn, salary);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
       
           

        }

        private void removeEmployeeBtn_Click(object sender, EventArgs e)
        {
            if(reservationDataGrid.SelectedRows.Count <= 0)
            {
                return;
                
            }

            int row =(int) reservationDataGrid.SelectedRows[0].Index;
            DataTable tbl = (DataTable)reservationDataGrid.DataSource;
            if(row < 0 || row >= tbl.Rows.Count)
            {
                return; 
            }

            int userId = (int)tbl.Rows[row].ItemArray[0];
            try
            {
                DatabaseManager.removeUser(userId);
                reservationDataGrid.DataSource = DatabaseManager.getUsers();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
    }
}
