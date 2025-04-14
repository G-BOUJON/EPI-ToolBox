using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Areas.LicenseManager.Data
{
    public class LicenseManagerDBContext : DbContext
    {
        public LicenseManagerDBContext() { }
        public LicenseManagerDBContext(DbContextOptions<LicenseManagerDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DBAccount>().HasKey(table => new {
                table.AccountName,
                table.Server
            });

                   
        }

        public DbSet<DBAccount> Accounts { get; set; }
    }
}
