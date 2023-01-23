using PhoneBook.Contact.Dtos.Model.Contact;
using PhoneBook.Contact.Entities.Db;
using PhoneBook.Contact.Repositories.DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Repositories.Interfaces
{
    public interface IContactInfoRepository : IEntityRepository<ContactInfo>
    {
        Task<IEnumerable<ContactInfo>> CustomSearchAsync(ContactInfoCustomSearchModel model);
        Task<long> CustomSearchTotalCountAsync(ContactInfoCustomSearchModel model);

    }
}
