namespace ToolBox_MVC.Models
{
    public class TaskHistory
    {
        public List<TaskDate> SuppressionDates { get; set; }
        public List<TaskDate> RestorationDates { get; set; }

        public TaskHistory()
        {
            this.SuppressionDates = new List<TaskDate>();
            this.RestorationDates = new List<TaskDate>();
        }

        public bool RestoreContains(DateOnly date)
        {
            bool result = false;
            foreach (TaskDate taskDate in RestorationDates)
            {
                if (date.Equals(taskDate))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool SuppressionContains(DateOnly date)
        {
            bool result = false;
            foreach (TaskDate taskDate in SuppressionDates)
            {
                if (date.Equals(taskDate))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
    public class TaskDate
    {
        public DateOnly Date { get; set; }
        public List<Account> Accounts { get; set; }
        public List<TimeOnly> Hours { get; set; }

        public TaskDate()
        {
            this.Accounts = new List<Account>();
            this.Date = DateOnly.FromDateTime(DateTime.Now);
            this.Hours = new List<TimeOnly>();
        }
    }
}

