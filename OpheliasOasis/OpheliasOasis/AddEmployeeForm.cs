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

      
    }
}
