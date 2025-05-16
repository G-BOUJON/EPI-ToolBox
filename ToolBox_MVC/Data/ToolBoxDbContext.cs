using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;

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
        public DbSet<MFilesGroup> MFilesGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MFilesAccount>()
                .HasIndex(p => new { p.AccountName, p.ServerId }).IsUnique();

            builder.Entity<MFilesGroup>()
                .HasIndex(p => new { p.MFilesId, p.ServerId }).IsUnique();
            builder.Entity<MFilesGroup>()
                .Property(g => g.Maintained)
                .HasDefaultValue(false);

            builder.Entity<MFilesServer>()
                .OwnsOne(s => s.MfCredential);

            // 2 serveurs ne doivent pas partager le même nom
            builder.Entity<MFilesServer>()
                .HasIndex(s => s.Name)
                .IsUnique();

            builder.Entity<MFilesServer>()
                .OwnsOne(s => s.ADCredential);

            builder.Entity<MFilesServer>()
                .HasMany<MFilesAccount>()
                .WithOne()
                .HasForeignKey(a => a.ServerId)
                .IsRequired();

            builder.Entity<MFilesAccount>()
                .Property(a => a.UserId)
                .HasDefaultValue(0);
        }
    }
}
