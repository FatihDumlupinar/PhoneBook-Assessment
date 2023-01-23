using System;

namespace PhoneBook.Contact.Dtos.Model.Contact
{
    public class ContactListModel
    {
        public Guid ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactCompany { get; set; }
    }
}
