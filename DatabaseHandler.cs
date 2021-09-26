using ENSEK_API.Structures;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;

namespace ENSEK_API.Database
{
    //Class Concerned with all actions on the Database
    public class DatabaseHandler
    {
        //Method for Adding New Meter Reading
        public static bool AddMeterReading(MeterReading Reading, ILogger Logger)
        {
            try
            {
                if (CheckAccountExists(Reading.accountID, Logger))
                { //Located Account
                    using (var connection = new SqliteConnection("Data Source=ensek.db"))
                    { //Open Database Connection
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        { //Create SqlCommand
                            command.CommandText = @"INSERT INTO MeterReadings ([AccountID],[Date],[MeterReading]) VALUES ($AccountID,$Date,$MeterReading);";
                            command.Parameters.AddWithValue("$AccountID", Reading.accountID);
                            command.Parameters.AddWithValue("$Date", Reading.date.ToString());
                            command.Parameters.AddWithValue("$MeterReading", Reading.reading);
                            command.ExecuteNonQuery(); //Add New Record
                        }
                    }

                    return true;
                }
                else
                { //Can't Locate Account ID
                    return false;
                }
            }
            catch (SqliteException Ex)
            { //Exception occured during upload - Log to Logger
                Logger.LogError(Ex, "Exception during Meter Reading Upload");
            }

            return false;
        }

        //Helper Method to check whether an Account with the Specified AccountID exists
        private static bool CheckAccountExists(int AccountID, ILogger Logger)
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=ensek.db"))
                { //Create SqlConnection
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    { //Create SqlCommand
                        command.CommandText = @"SELECT [AccountID] FROM Accounts WHERE [AccountID] = $AccountID";
                        command.Parameters.AddWithValue("$AccountID", AccountID);

                        using (var reader = command.ExecuteReader())
                        { //Open DB Reader
                            return reader.Read(); //Return whether we found a matching record based on the AccountID supplied
                        }
                    }
                }
            }
            catch (SqliteException Ex)
            { //Exception occured during check - Log to Logger
                Logger.LogError(Ex, "Exception during Account Exists Check");
            }

            return false;
        }

        //Returns all Accounts in the Database
        public static Account[] GetAccounts(ILogger Logger)
        {
            Account[] ReturnValue = new Account[31];
            int Count = 0;

            try
            {
                using (var connection = new SqliteConnection("Data Source=ensek.db"))
                { //Create SqlConnection
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    { //Create SqlCommand
                        command.CommandText = @"SELECT [AccountID],[FirstName],[LastName] FROM Accounts;";

                        using (var reader = command.ExecuteReader())
                        { //Open DB Reader
                            while (reader.Read())
                            { //While More Records to be Read
                                ReturnValue[Count].accountID = reader.GetInt32(0);
                                ReturnValue[Count].firstName = reader.GetString(1);
                                ReturnValue[Count].lastName = reader.GetString(2);
                                Count +=1 ;
                                if(Count == ReturnValue.Length) Array.Resize(ref ReturnValue, Count + 32);
                            }
                        }
                    }
                }

                if(Count == 0) {
                    return null;
                } else if (Count < ReturnValue.Length) {
                    Array.Resize(ref ReturnValue, Count);
                }

                return ReturnValue;
            }
            catch (SqliteException Ex)
            { //Exception occured during check - Log to Logger
                Logger.LogError(Ex, "Exception during Account Retrieval");
            }

            return null;
        }
    }
}