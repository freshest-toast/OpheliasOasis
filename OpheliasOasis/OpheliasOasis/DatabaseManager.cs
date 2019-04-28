using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpheliasOasis
{
    class DatabaseManager
    {
        private static string connectionString = "Server=tcp:opheliasoasis.ddns.net\\freshesttoast,8080; Database=OpheliasOasis;";
        private static SqlConnection connection;
        private static string sessionId = "";

        /// <summary>
        /// Connects to the SQL Database and logs in a user to the system. Ensures that the username and password are valid.
        /// If a connection is already open it is closed and a new connection is made.
        /// Throws an exception if the arguments are invalid, a connection to the database fails or if the login attempt fails 
        /// </summary>
        /// <param name="username"> A username of at least 6 characters</param>
        /// <param name="password"> A password of at least 6 characters</param>
        /// <returns> The access level of the logged in user</returns>
        public static int loginUser(string username, string password)
        {
            //Check username and password for length
            if (String.IsNullOrWhiteSpace(username) || username.Length < 6)
                throw new ArgumentException("The username is too short");
            if (String.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("The password is too short");

            //Close open connection, create credentials and connection
            connection?.Close();
            sessionId = "";
            SecureString secStr = new NetworkCredential("","_^K&5uN_WDTRGXDeCqd-!gcJc=qAqUpj").SecurePassword;
            secStr.MakeReadOnly();
            SqlCredential credential = new SqlCredential("ooLogin", secStr);
            connection = new SqlConnection(connectionString, credential);

            //Sql return parameters
            int errorCode,accessLevel;      
            string errorMessage;
            try
            {
                //Open connection and execute login user stored procedure to get a session id
                connection.Open();
                using (SqlCommand command = new SqlCommand("loginUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    //Add parameters
                    command.Parameters.Add(new SqlParameter("@UserName", username));

                    //Hash the password using SHA2 512 and add as a parameter
                    byte[] strBytes = Encoding.UTF8.GetBytes(password);
                    using (SHA512 sha = new SHA512Managed())
                    {
                        SqlParameter passwordParam = new SqlParameter("@Password", sha.ComputeHash(strBytes));
                        passwordParam.SqlDbType = SqlDbType.Binary;
                        command.Parameters.Add(passwordParam);
                    }

                    //Define the output parameters of the SQL command
                    SqlParameter sessionParam = new SqlParameter("@SessionId", SqlDbType.VarChar);
                    sessionParam.Direction = ParameterDirection.InputOutput;
                    sessionParam.Value = "";
                    SqlParameter accessLevelParam = new SqlParameter("@AccessLevel", SqlDbType.Int);
                    accessLevelParam.Direction = ParameterDirection.InputOutput;
                    SqlParameter errorMessageParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errorMessageParam.Direction = ParameterDirection.InputOutput;
                    errorMessageParam.Value = "";
                    SqlParameter errorCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errorCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(sessionParam);
                    command.Parameters.Add(accessLevelParam);
                    command.Parameters.Add(errorMessageParam);
                    command.Parameters.Add(errorCodeParam);

                    //Execute the SQL procedure and get the procedures output
                    command.ExecuteNonQuery();
                    errorCode = Convert.ToInt32(errorCodeParam.Value);
                    accessLevel = Convert.ToInt32(accessLevelParam.Value);
                    errorMessage = Convert.ToString(errorMessageParam.Value);
                    sessionId = Convert.ToString(sessionParam.Value);
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }

            //Ensure an error did not occur
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return accessLevel;
        }

        /// <summary>
        /// Adds a user/employee to the system. Checks the parameters. Throws an exception if the parameters are invalid or the logged in user
        /// does not have access rights to add users
        /// </summary>
        /// <param name="newUsername">A username of at least 6 characters</param>
        /// <param name="newPassword">A password of at least 6 characters</param>
        /// <param name="newAccessLevel">The access level of the new user. Values are from 1-3</param>
        /// <param name="firstName">The first name of the user. Should not be null or whitespace</param>
        /// <param name="lastName">The last name of the user. Should not be null or whitespace</param>
        /// <param name="ssn">The social security number of the new user. Must be 9 digits</param>
        /// <param name="salary">The salary of the new employee, must be non-negative and greater than 0</param>
        public static void addUser(string newUsername, string newPassword, int newAccessLevel, string firstName, string lastName, string ssn, int salary)
        {
            //Check the input parameters
            if (String.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 6)
                throw new ArgumentException("The user name must be at least 6 characters");
            if(String.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("The password must be at least 6 characters");
            if (newAccessLevel < 1 || newAccessLevel > 3)
                throw new ArgumentException("The user's access level must be between 1 and 3");
            if (String.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("The user must have a first name");
            if (String.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("The user must have a last name");
            if (String.IsNullOrWhiteSpace(ssn))
                throw new ArgumentException("The user's social security number must be specified");
            int ssnNumber;
            if (!Int32.TryParse(ssn,out ssnNumber))
                throw new ArgumentException("The user's social security number must be digits");
            if (ssnNumber > 999999999 || ssnNumber < 100000000)
                throw new ArgumentException("The user's social security number must be 9 digits");
            if (salary < 1)
                throw new ArgumentException("The user's salary must be greater than 0");

            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("addUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@NewUserName",newUsername),
                            new SqlParameter("@NewPassword",newPassword),
                            new SqlParameter("@NewAccessLevel",newAccessLevel),
                            new SqlParameter("@FirstName", firstName),
                            new SqlParameter("@LastName",lastName),
                            new SqlParameter("@SSN",ssnNumber),
                            new SqlParameter("@Salary",salary)
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);
                    command.ExecuteNonQuery();
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        public static void removeUser(int userId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("removeUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@UserId",userId)
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);
                    command.ExecuteNonQuery();
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        public static void modifyUser(int userId,string newUsername, int accessLevel, string firstName, string lastName, int salary)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("modifyUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@UserId",userId),
                            new SqlParameter("@NewUserName",newUsername),
                            new SqlParameter("@NewAccessLevel",accessLevel),
                            new SqlParameter("@FirstName", firstName),
                            new SqlParameter("@LastName",lastName),
                            new SqlParameter("@Salary",salary)
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);
                    command.ExecuteNonQuery();
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }
  
        public static DataTable findReservation(int guestId, DateTime initialDate, DateTime endDate)
        {
            int errorCode;
            string errorMessage;
            DataTable returnval = new DataTable("Reservations");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("reservationId",typeof(int),null,MappingType.Hidden),
                new DataColumn("Stay Start Date", typeof(DateTime)),
                new DataColumn("Stay End Date",typeof(DateTime)),
                new DataColumn("Guest Name",typeof(DateTime))
            });
            try
            {
                using (SqlCommand command = new SqlCommand("findReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@GuestID",guestId)
                        });
                    SqlParameter initDateParam = new SqlParameter("@InitialDate", SqlDbType.Date);
                    initDateParam.Value = initialDate;
                    command.Parameters.Add(initDateParam);
                    SqlParameter endDateParam = new SqlParameter("@EndDate", SqlDbType.Date);
                    endDateParam.Value = endDate;
                    command.Parameters.Add(endDateParam);
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        while(reader.Read())
                            returnval.Rows.Add(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), reader.GetString(3));
                    } 
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return returnval;
        }

        public static void cancelReservation(int reservationId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("cancelReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@ReservationId",reservationId),
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);
                    command.ExecuteNonQuery();
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }


    }
}
