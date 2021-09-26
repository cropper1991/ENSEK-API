namespace ENSEK_API.Structures
{
    //Structure to Represent the Result of a Meter Reading Upload
    public struct MeterReadingUploadResult
    {
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }
    }
}