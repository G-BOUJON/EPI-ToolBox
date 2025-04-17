using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Data
{
    public class ToolBoxDbContext : DbContext
    {
        public ToolBoxDbContext() { }
        public ToolBoxDbContext(DbContextOptions<ToolBoxDbContext> options) : base(options)
        {
        }

        public DbSet<MFilesServer> MFilesServers { get; set; }
        public DbSet<MFilesAccount> MFilesAccounts { get; set; }
        public DbSet<MFilesCredential> MFilesCredentials { get; set; }
        public DbSet<MFilesGroup> MFilesGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MFilesAccount>()
                .HasIndex(p => new { p.AccountName, p.ServerId }).IsUnique();

            builder.Entity<MFilesGroup>()
                .HasIndex(p => new { p.MFilesId, p.ServerId }).IsUnique();

            builder.Entity<MFilesServer>()
                .HasOne(s => s.Credential)
                .WithOne(c => c.MFilesServer)
                .HasForeignKey<MFilesCredential>(c => c.ServerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
