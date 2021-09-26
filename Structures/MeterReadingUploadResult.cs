namespace ENSEK_API.Structures
{
    //Structure to Represent the Result of a Meter Reading Upload
    public struct MeterReadingUploadResult
    {
        public int successfulCount { get; set; }
        public int failedCount { get; set; }
    }
}