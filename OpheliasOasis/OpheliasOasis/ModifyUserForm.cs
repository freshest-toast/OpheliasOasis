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
    public partial class ModifyUserForm : Form
    {
        public ModifyUserForm()
        {
            InitializeComponent();
            usersDataGrid.DataSource = DatabaseManager.getUsers();
        }

        private void updateEmployeeBtn_Click(object sender, EventArgs e)
        {
            if (usersDataGrid.SelectedRows.Count <= 0)
            {
                return;

            }

            int row = (int)usersDataGrid.SelectedRows[0].Index;
            DataTable tbl = (DataTable)usersDataGrid.DataSource;
            if (row < 0 || row >= tbl.Rows.Count)
            {
                return;
            }

            int userId = (int)tbl.Rows[row].ItemArray[0];
            try
            {
                string newUserName = newUsernameBox.Text;
                int accessLevel = Convert.ToInt32(newAccessBox.Text);
                string firstName = newFirstNameBox.Text;
                string lastName = newLastNameBox.Text;
                int salary = Convert.ToInt32(newSalaryBox.Text);

                DatabaseManager.modifyUser(userId, newUserName, accessLevel, firstName, lastName, salary);
                usersDataGrid.DataSource = DatabaseManager.getUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
