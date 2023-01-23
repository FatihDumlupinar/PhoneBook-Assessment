using Microsoft.EntityFrameworkCore;
using PhoneBook.Contact.Dtos.Model.Contact;
using PhoneBook.Contact.Entities.Db;
using PhoneBook.Contact.Repositories.Contexts;
using PhoneBook.Contact.Repositories.DataAccess.EntityFramework;
using PhoneBook.Contact.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Repositories.Methods
{
    public class ContactInfoRepository : EfCoreEntityRepository<ContactInfo>, IContactInfoRepository
    {
        public ContactInfoRepository(ContactDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ContactInfo>> CustomSearchAsync(ContactInfoCustomSearchModel model)
        {
            var iQueryableData = GetQueryableData(model);

            if (model.OrderByDesc)//Desc
            {
                iQueryableData = iQueryableData.OrderByDescending(i => i.CreateDate);
            }
            else if (model.OrderByDesc)//ASC
            {
                iQueryableData = iQueryableData.OrderBy(i => i.CreateDate);
            }

            if (model.Limit != -1)
            {
                int limit = model.Limit > 0 ? model.Limit : 20;

                //Not: kullanıcı 1 sayfada ise (1-1)*20 = 0 tane geç, 20 tane getir (limit default olarak 20)
                if (model.PageNo > 0)
                {
                    iQueryableData = iQueryableData.Skip((model.PageNo - 1) * limit).Take(limit);
                }
                else
                {
                    iQueryableData = iQueryableData.Take(model.Limit > 0 ? model.Limit : 20);
                }
            }
            //not: Eğer limit -1 geldi ise tüm verileri çek

            return await iQueryableData.ToListAsync();
        }

        public async Task<long> CustomSearchTotalCountAsync(ContactInfoCustomSearchModel model)
        {
            var iQueryableData = GetQueryableData(model);

            return await iQueryableData.LongCountAsync();
        }

        private IQueryable<ContactInfo> GetQueryableData(ContactInfoCustomSearchModel model)
        {
            IQueryable<ContactInfo> iQueryableData = _entities.AsQueryable().Where(i => i.IsActive); //AsQueryable where yi sorguları içinde tutuyor

            //kullanıcılar
            if (model.ContactIds.Any())
            {
                iQueryableData = iQueryableData.Where(i => model.ContactIds.Contains(i.Id));
            }

            return iQueryableData;
        }
    }
}
