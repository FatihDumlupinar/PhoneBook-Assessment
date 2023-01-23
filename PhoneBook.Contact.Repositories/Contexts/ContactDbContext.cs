using Microsoft.EntityFrameworkCore;
using PhoneBook.Contact.Entities.Db;
using System.Diagnostics.CodeAnalysis;

namespace PhoneBook.Contact.Repositories.Contexts
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext([NotNull] DbContextOptions<ContactDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactInfo>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<ContactDetail>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<ContactInfo> ContactInfos { get; set; }
        public virtual DbSet<ContactDetail> ContactDetails { get; set; }
        public virtual DbSet<StaticContactType> StaticContactTypes { get; set; }

    }
}
