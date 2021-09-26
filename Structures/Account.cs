namespace ENSEK_API.Structures
{
    //Structure to Represent Individual Records in the Accounts Table
    public struct Account
    {
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}