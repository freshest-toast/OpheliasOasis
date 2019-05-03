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
                DatabaseManager.loginUser("djones7777", "password1");
                var currReser = DatabaseManager.findReservation("Gravel", DateTime.Now, DateTime.Now);
                DatabaseManager.checkIn(currReser.Rows[0].ItemArray[0] as string);
                var reservations = DatabaseManager.getDailyOccupancyReport();
                
                DatabaseManager.changeReservation((string)reservations.Rows[0].ItemArray[0], DateTime.Now.AddMonths(1),
                    DateTime.Now.AddMonths(2), "David", "Bones", null);
                DatabaseManager.changeRate(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1),(decimal)3.0);
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
