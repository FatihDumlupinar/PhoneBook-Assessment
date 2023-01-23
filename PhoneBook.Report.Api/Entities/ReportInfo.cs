using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Report.Api.Entities
{
    public partial class ReportInfo: BaseEntity
    {
        public string ReportType { get; set; }

    }
}
