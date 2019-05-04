/*  
 *  Author: David Jones
 *  Date: April 8, 2019
 *  The purpose of this object is to control communication between the GUI and the SQL Server Database
 *  This object communicates with the server by using stored procedures. The object also ensures that 
 *  parameters passed to the SQL Server (from user input) are valid and of the required length. The object should
 *  never be created instead functions are static
 */

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
    /*
     *  Object with static functions used to connect and communicate with the server
     */
    class DatabaseManager
    {
        
        private static string connectionString = //"Server=192.168.2.2\\freshesttoast,1434; Database=OpheliasOasis;";
        "Server=tcp:opheliasoasis.ddns.net\\freshesttoast,8080; Database=OpheliasOasis;";

        //The connection to the sql server. Remains valid until a new user logs in or until the application closes
        private static SqlConnection connection;            
     
        //The session id of the current logged in user
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
            if (String.IsNullOrWhiteSpace(username) || username.Length < 6 || username.Length > 32)
                throw new ArgumentException("The username is too short or too long");
            if (String.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 32)
                throw new ArgumentException("The password is too short or too long ");
           
            //Close open connection, create credentials and connection
            connection?.Close();
            sessionId = "";
            SecureString secStr = new NetworkCredential("","_^K&5uN_WDTRGXDeCqd-!gcJc=qAqUpj").SecurePassword;
            secStr.MakeReadOnly();
            SqlCredential credential = new SqlCredential("ooLogin", secStr);
            connection = new SqlConnection(connectionString, credential);

            //Sql return parameters
            int errorCode,accessLevel = 0;      
            string errorMessage;
            try
            {
                //Open connection and execute login user stored procedure to get a session id
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("loginUser", connection,transaction))
                {
                    try
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //Add parameters
                        command.Parameters.Add(new SqlParameter("@UserName", username.ToLower()));

                        //Hash the password using SHA2 512 and add as a parameter
                        byte[] strBytes = Encoding.UTF8.GetBytes(password);
                        using (SHA512 sha = new SHA512Managed())
                        {
                            SqlParameter passwordParam = new SqlParameter("@Password", sha.ComputeHash(strBytes));
                            passwordParam.SqlDbType = SqlDbType.Binary;
                            command.Parameters.Add(passwordParam);
                        }

                        //Define the output parameters of the SQL command
                        SqlParameter sessionParam = new SqlParameter("@SessionId", SqlDbType.VarChar,36);
                        sessionParam.Direction = ParameterDirection.Output;
                        sessionParam.Value = "";
                        SqlParameter accessLevelParam = new SqlParameter("@AccessLevel", SqlDbType.Int);
                        accessLevelParam.Direction = ParameterDirection.Output;
                        SqlParameter errorMessageParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errorMessageParam.Direction = ParameterDirection.Output;
                        errorMessageParam.Value = "";
                        SqlParameter errorCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errorCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sessionParam);
                        command.Parameters.Add(accessLevelParam);
                        command.Parameters.Add(errorMessageParam);
                        command.Parameters.Add(errorCodeParam);

                        //Execute the SQL procedure and get the procedures output
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errorCodeParam.Value);
                        errorMessage = Convert.ToString(errorMessageParam.Value);
                        if(errorCode == 0)
                        {
                            accessLevel = Convert.ToInt32(accessLevelParam.Value);
                            sessionId = Convert.ToString(sessionParam.Value);
                            transaction.Commit();
                        }
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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
        /// Changes the password of the current logged in user.
        /// </summary>
        /// <param name="newPassword">Should be at least 6 characters</param>
        public static void changePassword(string newPassword)
        {
            //Check the password parameter
            if (String.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("The specified password is too short or too long");
            int errorCode;
            string errorMessage;
            try
            { 
                //Create a transaction and build the command
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("changePassword", connection, transaction))
                {
                    try
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //Add parameters
                        command.Parameters.Add(new SqlParameter("@ExecSessionId", sessionId));

                        //Hash the password using SHA2 512 and add as a parameter
                        byte[] strBytes = Encoding.UTF8.GetBytes(newPassword);
                        using (SHA512 sha = new SHA512Managed())
                        {
                            SqlParameter passwordParam = new SqlParameter("@NewPassword", sha.ComputeHash(strBytes));
                            passwordParam.SqlDbType = SqlDbType.Binary;
                            command.Parameters.Add(passwordParam);
                        }

                        //Define the output parameters of the SQL command
                        SqlParameter errorMessageParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errorMessageParam.Direction = ParameterDirection.Output;
                        errorMessageParam.Value = "";
                        SqlParameter errorCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errorCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errorMessageParam);
                        command.Parameters.Add(errorCodeParam);

                        //Execute the SQL procedure and get the procedures output
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errorCodeParam.Value);
                        errorMessage = Convert.ToString(errorMessageParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }

            //Ensure an error did not occur in the stored function
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }
        
        /// <summary>
        /// Gets the employees / users of the system
        /// </summary>
        /// <returns> A datatable that contains the id, first name, last name and username of each user in the system</returns>
        public static DataTable getUsers()
        {
            //Create the datatable to return
            DataTable returnval = new DataTable("Users");
            returnval.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn("id",typeof(int),null,MappingType.Hidden),
                new DataColumn("First Name",typeof(string)),
                new DataColumn("Last Name",typeof(string)),
                new DataColumn("Username",typeof(string))
            });
            int errorCode;
            string errorMessage;
            try
            {
                //Build the sql command to execute the stored procedure and execute the command
                using (SqlCommand command = new SqlCommand("findUser", connection))
                {
                    //Specify the command input parameters
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ExecSessionId", sessionId));

                    //Specify the command output parameters
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);

                    //Execute the command and read the returned table. Insert the data into the data table to return (by ordinal)
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            returnval.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
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
            return returnval;
        }
         
        /// <summary>
        /// Adds a user/employee to the system. Checks the parameters. Throws an exception if the parameters are invalid or the logged in user
        /// does not have access rights to add users
        /// </summary>
        /// <param name="newUsername">A username of at least 6 characters</param>
        /// <param name="newPassword">A password of at least 6 characters</param>
        /// <param name="newAccessLevel">The access level of the new user. Values are from 0-3</param>
        /// <param name="firstName">The first name of the user. Should not be null or whitespace</param>
        /// <param name="lastName">The last name of the user. Should not be null or whitespace</param>
        /// <param name="ssn">The social security number of the new user. Must be 9 digits</param>
        /// <param name="salary">The salary of the new employee, must be non-negative and greater than 0</param>
        public static void addUser(string newUsername, string newPassword, int newAccessLevel, string firstName, string lastName, string ssn, int salary)
        {
            //Check the input parameters
            if (String.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 6 || newUsername.Length > 64)
                throw new ArgumentException("The user name must be at least 6 characters and no more than 64 characters");
            if(String.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("The password must be at least 6 characters");
            if (newAccessLevel < 0 || newAccessLevel > 3)
                throw new ArgumentException("The user's access level must be between 0 and 3");
            if (String.IsNullOrWhiteSpace(firstName) || firstName.Length > 64)
                throw new ArgumentException("The user must have a first name of no more than 64 characters");
            if (String.IsNullOrWhiteSpace(lastName) || firstName.Length > 64)
                throw new ArgumentException("The user must have a last name of no more than 64 characters");
            if (String.IsNullOrWhiteSpace(ssn))
                throw new ArgumentException("The user's social security number must be specified");
            int ssnNumber;
            if (!Int32.TryParse(ssn,out ssnNumber))
                throw new ArgumentException("The user's social security number must be 9 digits");
            if (ssnNumber > 999999999 || ssnNumber < 100000000)
                throw new ArgumentException("The user's social security number must be 9 digits");
            if (salary < 1)
                throw new ArgumentException("The user's salary must be greater than 0");

            int errorCode;
            string errorMessage;
            try
            {

                //Execute the sql command in a transaction to add the new user / employee in a serializable transaction
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("addUser", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Add the parameters to the command
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@NewUserName",newUsername),
                            
                            new SqlParameter("@NewAccessLevel",newAccessLevel),
                            new SqlParameter("@FirstName", firstName),
                            new SqlParameter("@LastName",lastName),
                            new SqlParameter("@SSN",ssnNumber),
                            new SqlParameter("@Salary",salary)
                            });
                        //Hash the password using SHA2 512 and add as a parameter
                        byte[] strBytes = Encoding.UTF8.GetBytes(newPassword);
                        using (SHA512 sha = new SHA512Managed())
                        {
                            SqlParameter passwordParam = new SqlParameter("@NewPassword", sha.ComputeHash(strBytes));
                            passwordParam.SqlDbType = SqlDbType.Binary;
                            command.Parameters.Add(passwordParam);
                        }
                        //Add the output parameters to the command
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errMsgParam);

                        //Execute the command and get the output values
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }

            //Throw an exception if an error occurred
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        /// <summary>
        /// Removes the user/employee from the database specified by the user id. The user id should be obtained from a call to findUser
        /// Throws an exception if the user id does not exist.
        /// </summary>
        /// <param name="userId">The database id of the user/employee obtained from findUser</param>
        public static void removeUser(int userId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                //Build the SQL command within a transaction
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("removeUser", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        //Add the input parameters
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId),
                                new SqlParameter("@UserId",userId)
                            });

                        //Add the output parameters
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errMsgParam);
                        
                        //Execute the stored procedure
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }

            //Throw error if the stored procedure fails
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        /// <summary>
        /// Allows modification of a specified user / employee. The userId should be obtained from a call to findUser
        /// </summary>
        /// <param name="userId"> Database id of the user/employee obtained from findUser</param>
        /// <param name="newUsername">The new username of the specified user. Should be at least 6 characters</param>
        /// <param name="accessLevel">The new access of level of the user. Should be a value from 0-3</param>
        /// <param name="firstName">The name of the user, should be non-null</param>
        /// <param name="lastName">The last name of the user, should be non-null</param>
        /// <param name="salary">The salary of the user. Should be greater than 0</param>
        public static void modifyUser(int userId,string newUsername, int accessLevel, string firstName, string lastName, int salary)
        {
            //Check the parameters
            if (String.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 6 || newUsername.Length > 64)
                throw new ArgumentException("The user name must be at least 6 characters and no more than 64 characters");
            if (accessLevel < 0 || accessLevel > 3)
                throw new ArgumentException("The user's access level must be between 0 and 3");
            if (String.IsNullOrWhiteSpace(firstName) || firstName.Length > 64)
                throw new ArgumentException("The user must have a first name of no more than 64 characters");
            if (String.IsNullOrWhiteSpace(lastName) || firstName.Length > 64)
                throw new ArgumentException("The user must have a last name of no more than 64 characters");
            if (salary < 1)
                throw new ArgumentException("The user's salary must be greater than 0");

            int errorCode;
            string errorMessage;
            try
            {
                //Create the sql command to execute the stored procedure in a transaction
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("modifyUser", connection,transaction))
                {
                    try
                    {
                        //Add the input parameters
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

                        //Add the output parameters
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);

                        //Execute the command
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            //Check to make sure the stored procedure executed properly
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }
  
        /// <summary>
        /// Finds reservations in the system under the specified guest name within the specified date range
        /// </summary>
        /// <param name="guestName">The name of the guest. May be first and/or last name</param>
        /// <param name="initialDate">The starting date to search from</param>
        /// <param name="endDate">The ending date to search from</param>
        /// <returns>A datatable that contains all related reservations to the specified guest name. 
        /// The desired reservation should be selected with the ReservationId column in the data table (ordinal 0)</returns>
        public static DataTable findReservation(string guestName, DateTime initialDate, DateTime endDate)
        {
            //Check the input parameters
            if (!String.IsNullOrWhiteSpace(guestName) && guestName.Length > 64)
                throw new ArgumentException("The guest name is too short or too long");
            int errorCode;
            string errorMessage;

            //Create the data table to return from the function
            DataTable returnval = new DataTable("Reservations");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("ReservationId",typeof(string),null,MappingType.Hidden),
                new DataColumn("Guest Name",typeof(string)),
                new DataColumn("Stay Start Date",typeof(string)),
                new DataColumn("Stay End Date",typeof(string))
            });

            try
            {
                //Create and execute the command that calls the stored procedure in a transaction
                using (SqlCommand command = new SqlCommand("findReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    //Add input parameters
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@GuestName",guestName ?? "")
                        });
                    SqlParameter initDateParam = new SqlParameter("@InitialDate", SqlDbType.Date);
                    initDateParam.Value = initialDate;
                    command.Parameters.Add(initDateParam);
                    SqlParameter endDateParam = new SqlParameter("@EndDate", SqlDbType.Date);
                    endDateParam.Value = endDate;
                    command.Parameters.Add(endDateParam);

                    //Add the output parameters
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(errMsgParam);

                    //Execute the command and get the reader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Read the table and add the info to the table to return
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime(1);
                            string sDate = date.ToString("MM/dd/yyyy");
                            date = reader.GetDateTime(2);
                            string eDate = date.ToString("MM/dd/yyyy");
                            returnval.Rows.Add(reader.GetString(0), reader.GetString(3), sDate, eDate);
                        }
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }

            //Make sure the stored procedure executed without an error
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return returnval;
        }


        public static void cancelReservation(string reservationId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("CancelReservation", connection,transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@ReservationId",reservationId),
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        public static decimal addReservation(DateTime startDate, DateTime endDate, string firstName, string lastName, string emailAddress,string creditCard, out string reservationId)
        {
            reservationId = "";
            decimal cost;
            //Check username and password for length
            if (String.IsNullOrWhiteSpace(firstName) || firstName.Length > 64)
                throw new ArgumentException("The first name is too short or too long");
            if (String.IsNullOrWhiteSpace(lastName) || lastName.Length > 64)
                throw new ArgumentException("The last name is too short or too long");
            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                if(emailAddress.Length > 256)
                    throw new ArgumentException("The email address is too long");
                if (emailAddress.Count(
                    (char c) =>
                    {
                        return c == '@';
                    }) != 1)
                    throw new ArgumentException("The email address does not contain a @");
            }
            if (String.IsNullOrWhiteSpace(creditCard) || creditCard.Length != 16)
                throw new ArgumentException("The credit card is invalid");
            for (int i = 0; i < 16; i++)
                if (!Char.IsDigit(creditCard[i]))
                    throw new Exception("The credit card is invalid");
            int errorCode;
            string errorMessage;
            try
            {
                using(SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("AddReservation", connection,transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Add the parameters to the stored procedure call
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@FirstName", firstName),
                            new SqlParameter("@LastName",lastName),
                            new SqlParameter("@Email", emailAddress ?? ""),
                            new SqlParameter("@CCNum",creditCard)
                            });
                        SqlParameter startDateParameter = new SqlParameter("@StayStartDate", startDate);
                        startDateParameter.DbType = DbType.Date;
                        SqlParameter endDataParameter = new SqlParameter("@StayEndDate", endDate);
                        endDataParameter.DbType = DbType.Date;
                        command.Parameters.Add(startDateParameter);
                        command.Parameters.Add(endDataParameter);

                        //Add the output parameters
                        SqlParameter costParam = new SqlParameter("@Cost", SqlDbType.Decimal,18);
                        costParam.Precision = 18;
                        costParam.Scale = 2;
                        costParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(costParam);
                        SqlParameter reserveIdParam = new SqlParameter("@ReservationId", SqlDbType.VarChar, 36);
                        reserveIdParam.Direction = ParameterDirection.Output;
                        reserveIdParam.Value = "";
                        command.Parameters.Add(reserveIdParam);
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errMsgParam);
                        errMsgParam.Value = "";
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        cost = Convert.ToDecimal(costParam.Value);
                        reservationId = Convert.ToString(reserveIdParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return cost;
        }

        public static decimal changeReservation(string reservationId, DateTime startDate,DateTime endDate,string firstname, string lastname, string email)
        {
            int errorCode;
            decimal cost;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("ChangeReservation", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@ReservationId",reservationId),
                            new SqlParameter("@FirstName",firstname),
                            new SqlParameter("@LastName",lastname),
                            new SqlParameter("@Email",email ?? "")
                            });
                        SqlParameter startDateParameter = new SqlParameter("@StayStartDate", startDate);
                        startDateParameter.DbType = DbType.Date;
                        SqlParameter endDataParameter = new SqlParameter("@StayEndDate", endDate);
                        endDataParameter.DbType = DbType.Date;
                        command.Parameters.Add(startDateParameter);
                        command.Parameters.Add(endDataParameter);

                        SqlParameter costParameter = new SqlParameter("@Cost",SqlDbType.Decimal,18);
                        costParameter.Precision = 18;
                        costParameter.Scale = 2;
                        costParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(costParameter);
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        cost = Convert.ToDecimal(costParameter.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch(Exception x)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return cost;
        }

        public static void checkIn(string reservationId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("CheckIn", connection,transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@ReservationId",reservationId),
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        public static decimal checkOut(string reservationId)
        {
            
            decimal cost;
            int errorCode;
            string errorMessage;
            try
            {
                using(SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("CheckOut", connection,transaction))
                {
                    
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId),
                                new SqlParameter("@ReservationId",reservationId),
                            });
                        SqlParameter costParam = new SqlParameter("@Cost", SqlDbType.Decimal,18);
                        costParam.Precision = 18;
                        costParam.Scale = 2;
                        costParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(costParam);
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,32);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        cost = Convert.ToDecimal(costParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return cost;
        }

        public static void changeRate(DateTime beginDate,DateTime endDate, decimal newRate)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("ChangeRate", connection,transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId),
                                
                            });
                        SqlParameter rateParam = new SqlParameter("@NewRate", 18);
                        rateParam.Scale = 2;
                        rateParam.Precision = 18;
                        rateParam.Value = newRate;
                        command.Parameters.Add(rateParam);
                        SqlParameter begDateParam = new SqlParameter("@BegDate", DbType.Date);
                        begDateParam.Value = beginDate;
                        command.Parameters.Add(begDateParam);
                        SqlParameter endDateParam = new SqlParameter("@EndDate", DbType.Date);
                        endDateParam.Value = endDate;
                        command.Parameters.Add(endDateParam);
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }

        public static DataTable getExpectedIncomeReport()
        {
            DataTable returnval = new DataTable("Expected Income Report");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Date",typeof(string)),
                new DataColumn("Income For Day", typeof(decimal))
            });
            
            int errorCode;
            decimal totalIncome = 0;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("GetExpectedIncomeReport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                                new SqlParameter("@ExecSessionId",sessionId)
                        });
                    SqlParameter dateParam = new SqlParameter("@CurrentDate", SqlDbType.Date,1);
                    dateParam.Direction = ParameterDirection.Output;
                    dateParam.Value = DateTime.Now;
                    command.Parameters.Add(dateParam);
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.Output;
                    errMsgParam.Value = "";

                    List<Tuple<string, decimal>> results = new List<Tuple<string, decimal>>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string date = reader.GetDateTime(1).ToString("MM/dd/yyyy");
                            decimal incomeForNight = reader.GetDecimal(0);
                            totalIncome += incomeForNight;
                            results.Add(new Tuple<string, decimal>(date, incomeForNight));
                        }
                    }
                    DateTime startDate = Convert.ToDateTime(dateParam.Value);
                    for (int i = 0; i < 30; i++)
                        returnval.Rows.Add(startDate.AddDays(i).ToString("MM/dd/yyyy"), 0.0);
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                    foreach (var y in results)
                        foreach (DataRow x in returnval.Rows)
                            if ((string)x.ItemArray[0] == y.Item1)
                            {
                                x.SetField(1,y.Item2);
                                break;
                            }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            returnval.Rows.Add("Total Income", totalIncome);
            returnval.Rows.Add("Average Income", totalIncome / 30);
            return returnval;
        }

        public static DataTable getExpectedOccupancyReport()
        {
            DataTable returnval = new DataTable("Expected Occupancy Report");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Date",typeof(string)),
                new DataColumn("Number of Prepaid",typeof(int)),
                new DataColumn("Number of 60-Day",typeof(int)),
                new DataColumn("Number of Conventional",typeof(int)),
                new DataColumn("Number of Incentive Discounts",typeof(int)),
                new DataColumn("Total Rooms Occupied", typeof(int))
            });
            int errorCode, totalOccupants = 0; ;
            string errorMessage;
            List<Tuple<string, int, int, int, int,int>> results = new List<Tuple<string, int, int, int, int,int>>();
            try
            {
                using (SqlCommand command = new SqlCommand("GetExpectedOccupancyReport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                                new SqlParameter("@ExecSessionId",sessionId)
                        });
                    SqlParameter dateParam = new SqlParameter("@CurrentDate", SqlDbType.Date, 1);
                    dateParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(dateParam);
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.Output;
                    errMsgParam.Value = "";
                    command.Parameters.Add(errMsgParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            string date = reader.GetDateTime(0).ToString("MM/dd/yyyy");
                            int dayOccupancy = 0;
                            int var1 = reader.GetInt32(1);
                            int var2 = reader.GetInt32(2);
                            int var3 = reader.GetInt32(3);
                            int var4 = reader.GetInt32(4);
                            dayOccupancy = var1 + var2 + var3 + var4;
                            totalOccupants += dayOccupancy;
                            results.Add(new Tuple<string, int, int, int, int,int>(date, var1, var2, var3, var4,dayOccupancy));
                        }
                            
                    DateTime startDate = Convert.ToDateTime(dateParam.Value);
                    for (int i = 0; i < 30; i++)
                        returnval.Rows.Add(startDate.AddDays(i).ToString("MM/dd/yyyy"), 0,0,0,0,0);
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                    foreach (var y in results)
                        foreach (DataRow x in returnval.Rows)
                            if ((string)x.ItemArray[0] == y.Item1)
                            {
                                x.SetField(1, y.Item2);
                                x.SetField(2, y.Item3);
                                x.SetField(3, y.Item4);
                                x.SetField(4, y.Item5);
                                x.SetField(5, y.Item6);
                                break;
                            }
                    
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            returnval.Rows.Add("Average Expected Occupancy Rate", (decimal)totalOccupants / (45 * 30), 0, 0, 0, 0);
            return returnval;
        }

        public static DataTable getIncentiveReport()
        {
            DataTable returnval = new DataTable("Incentive Report");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Date",typeof(string)),
                new DataColumn("Incentive Discount",typeof(string))
            });
            int errorCode;
            decimal totalIncentiveDiscount = 0;
            string errorMessage;
            List<Tuple<string, decimal>> results = new List<Tuple<string, decimal>>();
            try
            {
                using (SqlCommand command = new SqlCommand("GetIncentiveReport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                                new SqlParameter("@ExecSessionId",sessionId)
                        });
                    SqlParameter dateParam = new SqlParameter("@CurrentDate", SqlDbType.Date, 1);
                    dateParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(dateParam);
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.Output;
                    errMsgParam.Value = "";
                    command.Parameters.Add(errMsgParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            string date = reader.GetDateTime(0).ToString("MM/dd/yyyy");
                            decimal incentive = reader.GetDecimal(1);
                            totalIncentiveDiscount += incentive;
                            results.Add(new Tuple<string, decimal>(date, incentive));
                        }

                    DateTime startDate = Convert.ToDateTime(dateParam.Value);
                    for (int i = 0; i < 30; i++)
                        returnval.Rows.Add(startDate.AddDays(i).ToString("MM/dd/yyyy"), 0);
                    errorCode = Convert.ToInt32(errCodeParam.Value);
                    errorMessage = Convert.ToString(errMsgParam.Value);
                    foreach (var y in results)
                        foreach (DataRow x in returnval.Rows)
                            if ((string)x.ItemArray[0] == y.Item1)
                            {
                                x.SetField(1, y.Item2.ToString());
                                break;
                            }
                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            returnval.Rows.Add("Total Incentive Discount", totalIncentiveDiscount);
            returnval.Rows.Add("Average Incentive Discount", totalIncentiveDiscount / 30);
            return returnval;
        }

        public static DataTable getDailyArrivalsReport()
        {
            DataTable returnval = new DataTable("Daily Arrivals Report");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Name",typeof(string)),
                new DataColumn("Reservation Type", typeof(string)),
                new DataColumn("Room #", typeof(int)),
                new DataColumn("Depature Date",typeof(string))
            });
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("GetDailyArrivalsReport", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId)
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        using (SqlDataReader reader = command.ExecuteReader())
                            while (reader.Read())
                                returnval.Rows.Add(reader.GetString(0),reader.GetString(1), reader.GetInt32(2), reader.GetDateTime(3).ToString("MM/dd/yyyy"));
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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

        public static DataTable getDailyOccupancyReport()
        {
            DataTable returnval = new DataTable("Daily Occupancy Report");
            returnval.Columns.AddRange(new DataColumn[]
            {

                new DataColumn("Room #", typeof(int)),
                new DataColumn("First Name",typeof(string)),
                new DataColumn("Last Name", typeof(string)),
                new DataColumn("Depature Date",typeof(string))
            });
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("GetDailyOccupancyReport", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId)
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        using (SqlDataReader reader = command.ExecuteReader())
                            while (reader.Read())
                                returnval.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3).ToString("MM/dd/yyyy"));
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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
        
        public struct AccomodationBill
        {
            public DateTime datePrinted;
            public string guestName;
            public int roomNumber;
            public DateTime arrivalDate;
            public DateTime depatureDate;
            public int numberOfNights;
            public decimal totalCharge;
            public bool isPrepaidOr60Day;
            public DateTime datePaidInAdvance;
            public decimal amountPaid;
        };

        public static AccomodationBill getAccomodationBill(string reservationId)
        {
           
            AccomodationBill returnval = new AccomodationBill();
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("getAccomodationBill", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@ReservationID",reservationId)
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.Output;
                    errMsgParam.Value = "";
                    command.Parameters.Add(errMsgParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            //Current date, guest name, room #, arrival date, depature date, # nights, total charge, total paid, date paid
                            returnval.datePrinted = reader.GetDateTime(0);
                            returnval.guestName = reader.GetString(1);
                            returnval.roomNumber = reader.GetInt32(2);
                            returnval.arrivalDate = reader.GetDateTime(3);
                            returnval.depatureDate = reader.GetDateTime(4);
                            returnval.numberOfNights = reader.GetInt32(5);
                            returnval.totalCharge = reader.IsDBNull(6) ? 0: reader.GetDecimal(6);
                            returnval.amountPaid = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
                            returnval.datePaidInAdvance = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8);
                        }
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
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

        public static DataTable getEmailList()
        {
            DataTable returnval = new DataTable("Customer Email List");
            returnval.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Name",typeof(string)),
                new DataColumn("Email", typeof(string)),
                new DataColumn("Reservation Start Date", typeof(string)),
                new DataColumn("Reservation End Date", typeof(string)),
                new DataColumn("Charge",typeof(decimal))
            });
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlCommand command = new SqlCommand("EmailGuest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            new SqlParameter("@ExecSessionId",sessionId)
                        });
                    SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                    errCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(errCodeParam);
                    SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                    errMsgParam.Direction = ParameterDirection.Output;
                    errMsgParam.Value = "";
                    command.Parameters.Add(errMsgParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string email = reader.GetString(1);
                            string reservationStartDate = reader.GetDateTime(2).ToString("MM/dd/yyyy");
                            string reservationEndDate = reader.GetDateTime(3).ToString("MM/dd/yyyy");
                            decimal amount = reader.GetDecimal(4);
                            returnval.Rows.Add(name, email, reservationStartDate, reservationEndDate, amount);
                        }
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
            return returnval;
        }
        public static bool makePayment(string reservationId)
        {
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("MakePayment", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                                new SqlParameter("@ExecSessionId",sessionId),
                                new SqlParameter("@ReservationId",reservationId)
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
                        errMsgParam.Value = "";
                        command.Parameters.Add(errMsgParam);
                        command.ExecuteNonQuery();
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
            }
            catch
            {
                throw new Exception("A connection issued occurred. Please contact an administrator");
            }
            if (errorCode != 0)
                throw new Exception(errorMessage);
            return true;
        }
    }
   

}
