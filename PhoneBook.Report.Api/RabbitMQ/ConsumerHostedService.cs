using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PhoneBook.Report.Api.RabbitMQ
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly IConsumerService _consumerService;

        public ConsumerHostedService(IConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumerService.ReadQueue(stoppingToken);
        }
    }
}
