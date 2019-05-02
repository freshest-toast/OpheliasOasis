﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpheliasOasis
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DatabaseManager.loginUser("djones7777", "password1");
                //var rsrvTable = DatabaseManager.findReservation("", DateTime.Now, DateTime.Now.AddDays(1));
                //var tbl = DatabaseManager.getDailyOccupancyReport();
                //DatabaseManager.modifyUser((int)tbl.Rows[0].ItemArray[0], "test", 3, "Tester", "Dusty", 10000);
            }
            catch (Exception e)
            {
                return;
            }
            
            Application.Run(new LogInScreen());
        }
    }
}
