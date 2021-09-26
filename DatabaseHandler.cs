using ENSEK_API.Structures;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

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
                if (CheckAccountExists(Reading.AccountID, Logger))
                { //Located Account
                    using (var connection = new SqliteConnection("Data Source=ensek.db"))
                    { //Open Database Connection
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        { //Create SqlCommand
                            command.CommandText = @"INSERT INTO MeterReadings ([AccountID],[Date],[MeterReading]) VALUES ($AccountID,$Date,$MeterReading);";
                            command.Parameters.AddWithValue("$AccountID", Reading.AccountID);
                            command.Parameters.AddWithValue("$Date", Reading.Date.ToString());
                            command.Parameters.AddWithValue("$MeterReading", Reading.Reading);
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
    }
}