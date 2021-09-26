using System;

namespace ENSEK_API.Structures
{

    //Structure to Represent Individual Records in the MeterReadings Table
        public struct MeterReading
    {
        public int AccountID { get; set; }
        public DateTime Date { get; set; }
        public string Reading { get; set; }
    }
}