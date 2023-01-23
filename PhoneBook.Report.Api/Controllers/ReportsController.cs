using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhoneBook.Report.Api.Contexts;
using PhoneBook.Report.Api.Entities;
using PhoneBook.Report.Api.Enums;
using PhoneBook.Report.Api.Models;
using PhoneBook.Report.Api.RabbitMQ;
using PhoneBook.Shared.Common.Helpers;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Report.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        #region Ctor&Fields

        private readonly ReportDbContext _reportDbContext;
        private readonly IRabbitMqService _rabbitMqService;

        public ReportsController(ReportDbContext reportDbContext, IRabbitMqService rabbitMqService)
        {
            _reportDbContext = reportDbContext;
            _rabbitMqService = rabbitMqService;
        }

        #endregion

        #region Create

        [HttpPost("Create/{Location}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Guid))]//Başarı olursa geriye rapor id yi döndürücez
        public async Task<IActionResult> CreateAsync(string Location)
        {
            ReportInfo entity = new()
            {
                CreateDate = DateTime.Now,
                ReportType = EnumHelper<ReportTypeEnm>.GetDisplayValue(ReportTypeEnm.Preparing)

            };

            await _reportDbContext.AddAsync(entity);
            await _reportDbContext.SaveChangesAsync();

            CreateReportModel createReport = new()
            {
                Location = Location,
                ReportInfoId = entity.Id

            };

            using var connection = _rabbitMqService.CreateChannel();
            using var model = connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(createReport));
            model.BasicPublish("UserExchange",
                                 string.Empty,
                                 basicProperties: null,
                                 body: body);

            return Ok(entity.Id);
        }

        #endregion

        #region List

        [HttpGet("List")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ReportListModel>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ListAsync()
        {
            var getAllData = await _reportDbContext.ReportInfos.ToListAsync();

            var returnModel = getAllData.Select(i => new ReportListModel()
            {
                ReportId = i.Id,
                ReportType = i.ReportType,
                CreateDate = i.CreateDate,
                UpdateDate = i.UpdateDate

            });

            return Ok(returnModel);
        }

        #endregion

        #region GetById

        [HttpGet("GetById/{Id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReportDetailsModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid Id)
        {
            var getReportData = await _reportDbContext.ReportInfos.FirstOrDefaultAsync(i => i.Id == Id);
            if (getReportData == default)
            {
                return NotFound();
            }

            ReportDetailsModel returnModel = new()
            {
                CreateDate = getReportData.CreateDate,
                ReportId = getReportData.Id,
                ReportType = getReportData.ReportType,
                UpdateDate = getReportData.UpdateDate,
            };

            if (getReportData.ReportType == EnumHelper<ReportTypeEnm>.GetDisplayValue(ReportTypeEnm.Completed))
            {
                var getReportDetailsData = await _reportDbContext.ReportDetails.FirstOrDefaultAsync(i => i.ReportInfoId == Id);
                if (getReportDetailsData == default)
                {
                    return NotFound();
                }

                returnModel.TotalContactCount = getReportDetailsData.TotalContactCount;
                returnModel.TotalPhoneNumberCount = getReportDetailsData.TotalPhoneNumberCount;
                returnModel.Location = getReportDetailsData.Location;

            }

            return Ok(returnModel);
        }

        #endregion

    }
}
