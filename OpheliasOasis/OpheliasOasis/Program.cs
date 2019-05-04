using System;
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
                //DatabaseManager.loginUser("djones7777", "password1");
                //string reserveId;
                //var tbl1 = DatabaseManager.addReservation(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Joe", "Schmoe", "", "1234567812345678", out reserveId);
                //DatabaseManager.makePayment(reserveId);
                //var bill = DatabaseManager.getAccomodationBill(reserveId);
                //var tbl = DatabaseManager.getAccomodationBill((string)tbl1.Rows[0].ItemArray[0]);
                //DatabaseManager.changeRate(DateTime.Now, DateTime.Now.AddMonths(2), 135);
                //cost = DatabaseManager.addReservation(DateTime.Now.AddDays(24), DateTime.Now.AddDays(25), "Johnny", "Smither","jdog@gmail.com","1234567812345678");
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
