using Microsoft.EntityFrameworkCore;
using PhoneBook.Report.Api.Entities;

namespace PhoneBook.Report.Api.Contexts
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportInfo>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ReportDetail>().Property(e => e.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<ReportInfo> ReportInfos { get; set; }
        public virtual DbSet<ReportDetail> ReportDetails { get; set; }

    }
}
