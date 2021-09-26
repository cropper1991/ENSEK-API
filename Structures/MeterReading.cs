using System;

namespace ENSEK_API.Structures
{

    //Structure to Represent Individual Records in the MeterReadings Table
        public struct MeterReading
    {
        public int accountID { get; set; }
        public DateTime date { get; set; }
        public string reading { get; set; }
    }
}