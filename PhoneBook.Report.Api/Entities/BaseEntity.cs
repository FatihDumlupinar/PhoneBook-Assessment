using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Report.Api.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
