using System;

namespace PhoneBook.Contact.Dtos.Model.ContactDetail
{
    public class ContactDetailModel
    {
        public Guid ContactId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactDetailText { get; set; }
    }
}
