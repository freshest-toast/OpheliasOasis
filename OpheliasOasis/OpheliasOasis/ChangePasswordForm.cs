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
    public partial class ChangePasswordForm : Form
    {
        public ChangePasswordForm()
        {
            InitializeComponent();
        }

        private void changePasswordBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string newPass = newPasswordBox.Text;

                DatabaseManager.changePassword(newPass);
                MessageBox.Show("Password Changed Successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
