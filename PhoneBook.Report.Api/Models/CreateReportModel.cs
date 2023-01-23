using System;

namespace PhoneBook.Report.Api.Models
{
    public class CreateReportModel
    {
        public Guid ReportInfoId { get; set; }
        public string Location { get; set; }

    }
}
