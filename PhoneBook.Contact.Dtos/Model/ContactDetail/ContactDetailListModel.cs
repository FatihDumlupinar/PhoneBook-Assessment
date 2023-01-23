using System;

namespace PhoneBook.Contact.Dtos.Model.ContactDetail
{
    public class ContactDetailListModel
    {
        public Guid ContactDetailId { get; set; }
        public int ContactDetailContactTypeId { get; set; }
        public string ContactDetailContactTypeText { get; set; }
        public string ContactDetailText { get; set; }
    }
}
