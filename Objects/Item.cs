namespace CheckRuCode.Objects
{
    public class Item
    {
        //public int No_ { get; }
        public string? registrationNumber { get; }
        public DateTime? registrationDateEnd { get; }
        
        public Item(string? ruCode, DateTime? endDate)
        {
            //No_ = id;
            registrationNumber = ruCode;
            registrationDateEnd = endDate;
        }
    }
}
