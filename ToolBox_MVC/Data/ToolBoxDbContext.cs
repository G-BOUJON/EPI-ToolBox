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
        public DbSet<HistoryOperation> HistoryOperations { get; set; }

        public DbSet<ActiveDirectory> ActiveDirectories { get; set; }
        public DbSet<ADAccount> ADAccounts { get; set; }
        public DbSet<ADGroup> ADGroups { get; set; }

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

            builder.Entity<MFilesServer>()
                .OwnsOne(s => s.AutomaticOP);

            // 2 serveurs ne doivent pas partager le même nom
            builder.Entity<MFilesServer>()
                .HasIndex(s => s.Name)
                .IsUnique();

            builder.Entity<MFilesServer>()
                .HasMany<MFilesAccount>()
                .WithOne()
                .HasForeignKey(a => a.ServerId)
                .IsRequired();

            builder.Entity<MFilesAccount>()
                .Property(a => a.UserId)
                .HasDefaultValue(0);

            builder.Entity<ADAccount>()
                .HasAlternateKey(a => a.GUID);

            builder.Entity<ADGroup>()
                .HasAlternateKey(g => g.GUID);

            builder.Entity<ActiveDirectory>()
                .OwnsOne(s => s.EncryptedCredentials);

            builder.Entity<HistoryOperation>()
                .HasOne(h => h.Account)
                .WithMany()
                .HasForeignKey(h => h.AccountID)
                .IsRequired(true);

            builder.Entity<MFilesAccount>()
                .HasOne(m => m.ADAccount)
                .WithMany()
                .HasForeignKey(m => m.ADAccountGUID)
                .HasPrincipalKey(a => a.GUID)
                .IsRequired(false);
            

            builder.Entity<MFilesGroup>()
                .HasOne(m => m.ADGroup)
                .WithMany()
                .HasForeignKey(m => m.ADGroupGUID)
                .HasPrincipalKey(a => a.GUID)
                .IsRequired(false);

            builder.Entity<MFilesServer>()
                .HasOne(s => s.ActiveDirectory)
                .WithMany()
                .HasForeignKey(s => s.ActiveDirectoryID)
                .HasPrincipalKey(a => a.ID)
                .IsRequired(true);
        }
    }
}
