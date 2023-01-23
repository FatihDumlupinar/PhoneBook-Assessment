using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

namespace PhoneBook.Report.Api.BackgroundServices
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        #region Ctor&Fields

        private IConnection _connection;
        private IModel _channel;
        private readonly ReportDbContext _reportDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConsumeRabbitMQHostedService(IHttpClientFactory httpClientFactory, IServiceScopeFactory factory)
        {
            InitRabbitMQ();
            _httpClientFactory = httpClientFactory;
            _reportDbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ReportDbContext>();
        }

        #endregion

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { Uri = new("amqps://cpfwcofs:p4Sglf5puf5KljXRAJioS6HFThGQQbEa@roedeer.rmq.cloudamqp.com/cpfwcofs") };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("demo.queue.log", false, false, false, null);
            _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var model = JsonConvert.DeserializeObject<CreateReportModel>(content);

                await CreateReportAsync(model, stoppingToken);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("demo.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        private async Task CreateReportAsync(CreateReportModel model, CancellationToken stoppingToken)
        {
            using var client = _httpClientFactory.CreateClient("ContactClient");
            var baseAddress = client.BaseAddress.AbsoluteUri;

            using var response = await client.GetAsync($"{baseAddress}/api/Contacts/GetAllContactByLocation/{model.Location}");
            string responseBody = await response.Content.ReadAsStringAsync(stoppingToken);

            var contactApiReportModel = JsonConvert.DeserializeObject<ContactApiReportModel>(responseBody, new JsonSerializerSettings()
            {
                Error = (s, e) => { e.ErrorContext.Handled = true; }
            });

            var getReportData = await _reportDbContext.ReportInfos.FirstOrDefaultAsync(i => i.Id == model.ReportInfoId, stoppingToken);

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
                    TotalPhoneNumberCount = contactApiReportModel.TotalPhoneNumberCount

                }, stoppingToken);

            }

            _reportDbContext.ReportInfos.Update(getReportData);

            await _reportDbContext.SaveChangesAsync(stoppingToken);

        }

        #region Functions

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        #endregion
    }
}
