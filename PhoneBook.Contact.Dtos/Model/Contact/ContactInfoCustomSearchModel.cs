using System;
using System.Collections.Generic;

namespace PhoneBook.Contact.Dtos.Model.Contact
{
    public class ContactInfoCustomSearchModel
    {
        public List<Guid> ContactIds { get; set; } = new();
        public int Limit { get; set; } = 10;
        public int PageNo { get; set; } = 1;
        public bool OrderByDesc { get; set; } = true;//true ise büyükten küçüğe false ise küçükten büyüğe

    }
}
