using PhoneBook.Contact.Entities.Db.Base;

namespace PhoneBook.Contact.Entities.Db
{
    public partial class ContactInfo : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
    }
}
