using Microsoft.Extensions.Options;
using PhoneBook.Report.Api.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneBook.Report.Api.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitOptions _options;

        public RabbitMqService(IOptions<RabbitOptions> options)
        {
            _options = options.Value;
        }

        public IConnection CreateChannel()
        {
            ConnectionFactory connection = new ConnectionFactory()
            {
                Uri=new(_options.Url)
            };

            connection.DispatchConsumersAsync = true;

            var channel = connection.CreateConnection();
            
            return channel;
        }
    }
}
