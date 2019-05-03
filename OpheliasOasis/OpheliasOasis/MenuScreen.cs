﻿using System;
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
    public partial class MenuScreen : Form
    {
        public MenuScreen(int accessLevel)
        {
          
            InitializeComponent();
            if (accessLevel == 1)
            {
                this.groupBox1.Visible = false;
                this.groupBox2.Visible = false;
            }
            else if (accessLevel == 2)
            {
                this.groupBox2.Visible = false;
                this.groupBox1.Visible = true;
            }
            else if (accessLevel == 3)
            {
                this.groupBox1.Visible = true;
                this.groupBox2.Visible = true;
            }
            else
            {
                MessageBox.Show("Invalid access level, try again");
            }
        }

        private void createReservationBtn_Click(object sender, EventArgs e)
        {
            ReservationScreen reservationScreen = new ReservationScreen();
            reservationScreen.ShowDialog();
        }

        private void checkInBtn_Click(object sender, EventArgs e)
        {
            CheckIn checkIn = new CheckIn();
            checkIn.ShowDialog();
        }

        private void checkOutBtn_Click(object sender, EventArgs e)
        {
            CheckOut checkOut = new CheckOut();
            checkOut.ShowDialog();
        }

        private void cancelReservationBtn_Click(object sender, EventArgs e)
        {
            CancelReservation cancelReservation = new CancelReservation();
            cancelReservation.ShowDialog();
        }

        private void changeReservationBtn_Click(object sender, EventArgs e)
        {
            ChangeReservation changeReservation = new ChangeReservation();
            changeReservation.ShowDialog();
        }

        private void adminBtn_Click(object sender, EventArgs e)
        {
            SystemAdminTools systemAdminTools = new SystemAdminTools();
            systemAdminTools.ShowDialog();
        }

        private void generateAccomodationBtn_Click(object sender, EventArgs e)
        {
            AccomdationBills accomdationBills = new AccomdationBills();
            accomdationBills.ShowDialog();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void changeBaseRateBtn_Click(object sender, EventArgs e)
        {
            ChangeBaseRateForm changeBaseRateForm = new ChangeBaseRateForm();
            changeBaseRateForm.ShowDialog();
        }

        private void emailGuestBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
