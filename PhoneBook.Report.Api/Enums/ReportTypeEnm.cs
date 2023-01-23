using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Report.Api.Enums
{
    public enum ReportTypeEnm
    {
        [Display(Name ="Hazırlanıyor")]
        Preparing=1,

        [Display(Name ="Tamamlandı")]
        Completed= 2,

        [Display(Name ="Başarısız")]
        Failed = 3
    }
}
