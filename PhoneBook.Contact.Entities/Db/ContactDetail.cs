using PhoneBook.Contact.Entities.Db.Base;
using System;

namespace PhoneBook.Contact.Entities.Db
{
    public partial class ContactDetail : BaseEntity, IEntity
    {
        public Guid ContactId { get; set; }
        public int ContactTypeId { get; set; }
        public string Text { get; set; }


    }
}
