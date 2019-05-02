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

        private static string connectionString = //"Server=192.168.2.2\\freshesttoast,1434; Database=OpheliasOasis;";
            "Server=tcp:opheliasoasis.ddns.net\\freshesttoast,8080; Database=OpheliasOasis;";

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

        public static void changePassword(string newPassword)
        {
            int errorCode;
            string errorMessage;
            try
            { 
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

            //Ensure an error did not occur
            if (errorCode != 0)
                throw new Exception(errorMessage);
        }
        
        public static DataTable getUsers()
        {
            //id, firstname,lastname, username
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
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("findUser", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ExecSessionId",sessionId));
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errMsgParam);
                        using (SqlDataReader reader = command.ExecuteReader())
                            while(reader.Read())
                                returnval.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        errorCode = Convert.ToInt32(errCodeParam.Value);
                        errorMessage = Convert.ToString(errMsgParam.Value);
                       
                        if (errorCode == 0)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                        
                    }
                    catch(Exception xyz)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch(Exception yzz)
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
            if (String.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 6)
                throw new ArgumentException("The user name must be at least 6 characters");
            if(String.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("The password must be at least 6 characters");
            if (newAccessLevel < 0 || newAccessLevel > 3)
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

                //Execute the sql command to add the new user / employee in a serializable transaction
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
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("removeUser", connection, transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@UserId",userId)
                            });
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.Output;
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
            int errorCode;
            string errorMessage;
            try
            {
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("modifyUser", connection,transaction))
                {
                    try
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
                    catch(Exception e)
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
            if (String.IsNullOrWhiteSpace(guestName) || guestName.Length > 64)
                throw new ArgumentException("The guest name is too short or too long");
            int errorCode;
            string errorMessage;
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
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (SqlCommand command = new SqlCommand("findReservation", connection,transaction))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(
                            new SqlParameter[]
                            {
                            new SqlParameter("@ExecSessionId",sessionId),
                            new SqlParameter("@GuestName",guestName)
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
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar,256);
                        errMsgParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errMsgParam);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            errorCode = Convert.ToInt32(errCodeParam.Value);
                            errorMessage = Convert.ToString(errMsgParam.Value);
                            while (reader.Read())
                            {
                                DateTime date = reader.GetDateTime(1);
                                string sDate = date.ToString("MM/dd/yyyy");
                                date = reader.GetDateTime(2);
                                string eDate = date.ToString("MM/dd/yyyy");
                                returnval.Rows.Add(reader.GetString(0), reader.GetString(3), sDate, eDate);
                            }
                                
                        }
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

        public static void addReservation(DateTime startDate, DateTime endDate, string firstName, string lastName, string emailAddress)
        {
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
                            new SqlParameter("@Email", emailAddress ?? "")
                            });
                        SqlParameter startDateParameter = new SqlParameter("@StayStartDate", startDate);
                        startDateParameter.DbType = DbType.Date;
                        SqlParameter endDataParameter = new SqlParameter("@StayEndDate", endDate);
                        endDataParameter.DbType = DbType.Date;
                        command.Parameters.Add(startDateParameter);
                        command.Parameters.Add(endDataParameter);

                        //Add the output parameters
                        SqlParameter errCodeParam = new SqlParameter("@ErrCode", SqlDbType.Int);
                        errCodeParam.Direction = ParameterDirection.InputOutput;
                        command.Parameters.Add(errCodeParam);
                        SqlParameter errMsgParam = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 256);
                        errMsgParam.Direction = ParameterDirection.InputOutput;
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

        public static void changeReservation(string reservationId, DateTime startDate,DateTime endDate,string firstname, string lastname, string email)
        {
            int errorCode;
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
                    catch(Exception e)
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

        public static void checkOut(string reservationId)
        {
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
                                new SqlParameter("@NewRate",newRate),
                            });
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
            return null;
        }

        public static DataTable getExpectedOccupancyReport()
        {
            return null;
        }

        public static DataTable getIncentiveReport()
        {
            return null;
        }

        public static DataTable getDailyArrivalsReport()
        {
            return null;
        }

        public static DataTable getDailyOccupancyReport()
        {
            return null;
        }

        public struct AccomodationBill
        {
            DateTime datePrinted;
            string guestName;
            int roomNumber;
            DateTime arrivalDate;
            DateTime depatureDate;
            int numberOfNights;
            decimal totalCharge;
            bool isPrepaidOr60Day;
            DateTime datePaidInAdvance;
            decimal amountPaid;
        };

        public static AccomodationBill getAccomodationBill(int reservationId)
        {
            return new AccomodationBill();
        }
    }


}
