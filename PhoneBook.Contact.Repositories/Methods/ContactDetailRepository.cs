using PhoneBook.Contact.Entities.Db;
using PhoneBook.Contact.Repositories.Contexts;
using PhoneBook.Contact.Repositories.DataAccess.EntityFramework;
using PhoneBook.Contact.Repositories.Interfaces;

namespace PhoneBook.Contact.Repositories.Methods
{
    public class ContactDetailRepository : EfCoreEntityRepository<ContactDetail>, IContactDetailRepository
    {
        public ContactDetailRepository(ContactDbContext dbContext) : base(dbContext)
        {
        }
    }
} 
