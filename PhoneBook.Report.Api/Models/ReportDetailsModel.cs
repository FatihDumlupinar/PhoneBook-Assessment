using System;

namespace PhoneBook.Report.Api.Models
{
    public class ReportDetailsModel
    {
        public Guid ReportId { get; set; }

        public string ReportType { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string Location { get; set; }
        public int TotalContactCount { get; set; }
        public int TotalPhoneNumberCount { get; set; }
    }
}
