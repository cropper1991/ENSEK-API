namespace ENSEK_API.Structures
{
    //Structure to Represent Individual Records in the Accounts Table
    public struct Account
    {
        public int accountID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}