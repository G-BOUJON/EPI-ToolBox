namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class HistoryOperation
    {
        public int ID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int OperationType { get; set; }
        public bool Automatic { get; set; }

        public int AccountID { get; set; }
        public MFilesAccount Account { get; set; }

        public HistoryOperation() { }

        public HistoryOperation(DateTime dateTime, int operationType, MFilesAccount account, bool automatic = true) 
            : this(DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime), operationType, account, automatic) { }

        public HistoryOperation(DateOnly date, TimeOnly time, int operationType, MFilesAccount account, bool automatic = true) 
        {
            this.Date = date;
            this.Time = time;
            this.OperationType = operationType;
            this.Automatic = automatic;
            this.AccountID = account.Id;
            this.Account = account;
        }
    }
}
