namespace ToolBox.Models
{
    public class History
    {
        public List<Date> dates { get; set; }

        public History()
        {
            this.dates = new List<Date>();
        }
    }

    public class Date
    {
        public string date { get; set; }
        public List<Account> deletedAccounts { get; set; }
        public List<string> hour { get; set; }

        public Date()
        {
            this.deletedAccounts = new List<Account>();
            this.date = string.Empty;
            this.hour = new List<string>();
        }
    }
}
