using System;

namespace PhoneBook.Report.Api.Models
{
    public class ReportListModel
    {
        public Guid ReportId { get; set; }

        public string ReportType { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
