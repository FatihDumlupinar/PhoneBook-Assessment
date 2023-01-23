using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Contact.Dtos.Enums
{
    public enum StaticContactTypeEnm
    {
        [Display(Name = "Phone Number")]
        PhoneNumber = 1,

        [Display(Name = "Email")]
        Email = 2,
        
        [Display(Name ="Location")]
        Location = 3

    }
}
