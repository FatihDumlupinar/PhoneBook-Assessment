using System;

namespace PhoneBook.Report.Api.Entities
{
    public partial class ReportDetail : BaseEntity
    {
        public Guid ReportInfoId { get; set; }

        public string Location { get; set; }
        public int TotalContactCount { get; set; }
        public int TotalPhoneNumberCount { get; set; }

    }
}
