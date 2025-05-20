using Microsoft.EntityFrameworkCore;

namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    
    public class AutomaticOperations
    {
        /// <summary>
        /// Defines if removal operations are done automatically each days after the automatic synchronization
        /// </summary>
        public bool AutoRemove { get; set; }
        /// <summary>
        /// Defines if restoration operations are done automatically each days after the automatic synchronization
        /// </summary>
        public bool AutoRestore { get; set; }
        /// <summary>
        /// Defines if the operations that change M-Files Accounts status based on the referenced domain account are done automatically each days after the automatic synchronization
        /// </summary>
        public bool AutoActivationHandling { get; set; }

        public AutomaticOperations() { }

        public AutomaticOperations(bool autoRemove, bool autoRestore, bool autoActivationHandling)
        {
            AutoRemove = autoRemove;
            AutoRestore = autoRestore;
            AutoActivationHandling = autoActivationHandling;
        }

        public AutomaticOperations Clone()
        {
            return new AutomaticOperations(AutoRemove, AutoRestore, AutoActivationHandling);
        }
    }
}
