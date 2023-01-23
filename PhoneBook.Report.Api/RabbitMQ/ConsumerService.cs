using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PhoneBook.Report.Api.Contexts;
using PhoneBook.Report.Api.Entities;
using PhoneBook.Report.Api.Enums;
using PhoneBook.Report.Api.Models;
using PhoneBook.Shared.Common.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoneBook.Report.Api.RabbitMQ
{
    public class ConsumerService : IConsumerService, IDisposable
    {
        #region Ctor&Fields

        private readonly IModel _model;
        private readonly IConnection _connection;
        private readonly ReportDbContext _reportDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string _queueName = "User";

        public ConsumerService(IRabbitMqService rabbitMqService, IHttpClientFactory httpClientFactory, IServiceScopeFactory factory)
        {
            _connection = rabbitMqService.CreateChannel();
            _model = _connection.CreateModel();
            _model.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            _model.ExchangeDeclare("UserExchange", ExchangeType.Fanout, durable: true, autoDelete: false);
            _model.QueueBind(_queueName, "UserExchange", string.Empty);
            _httpClientFactory = httpClientFactory;
            _reportDbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ReportDbContext>();
        }

        #endregion

        public async Task ReadQueue(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var model = JsonConvert.DeserializeObject<CreateReportModel>(content);

                await CreateReportAsync(model, stoppingToken);

                await Task.CompletedTask;

                _model.BasicAck(ea.DeliveryTag, false);
            };
            _model.BasicConsume(_queueName, false, consumer);
            await Task.CompletedTask;
        }

        private async Task CreateReportAsync(CreateReportModel model, CancellationToken stoppingToken)
        {
            using var client = _httpClientFactory.CreateClient("ContactClient");
            var baseAddress = client.BaseAddress.AbsoluteUri;

            //Örnek : https://localhost:44370/api/Contacts/GetAllContactByLocation/%C4%B0stanbul
            using var response = await client.GetAsync($"{baseAddress}/Contacts/GetAllContactByLocation/{model.Location}");
            string responseBody = await response.Content.ReadAsStringAsync(stoppingToken);

            var contactApiReportModel = JsonConvert.DeserializeObject<ContactApiReportModel>(responseBody, new JsonSerializerSettings()
            {
                Error = (s, e) => { e.ErrorContext.Handled = true; }
            });

            var test = await _reportDbContext.ReportInfos.ToListAsync();

            var getReportData = await _reportDbContext.ReportInfos.FirstOrDefaultAsync(i => i.Id == model.ReportInfoId, stoppingToken);

            if (getReportData == default)
            {
                return;
            }

            var dateTimeNow = DateTime.Now;
            getReportData.UpdateDate = dateTimeNow;

            if (contactApiReportModel == default)
            {
                getReportData.ReportType = EnumHelper<ReportTypeEnm>.GetDisplayValue(ReportTypeEnm.Failed);
            }
            else
            {
                getReportData.ReportType = EnumHelper<ReportTypeEnm>.GetDisplayValue(ReportTypeEnm.Completed);

                await _reportDbContext.ReportDetails.AddAsync(new ReportDetail()
                {
                    CreateDate = dateTimeNow,
                    Location = model.Location,
                    TotalContactCount = contactApiReportModel.TotalContactCount,
                    TotalPhoneNumberCount = contactApiReportModel.TotalPhoneNumberCount,
                    ReportInfoId = model.ReportInfoId

                }, stoppingToken);

            }

            _reportDbContext.ReportInfos.Update(getReportData);

            await _reportDbContext.SaveChangesAsync(stoppingToken);
        }

        public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}
