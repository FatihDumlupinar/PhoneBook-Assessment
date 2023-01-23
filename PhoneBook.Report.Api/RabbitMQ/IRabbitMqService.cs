using RabbitMQ.Client;

namespace PhoneBook.Report.Api.RabbitMQ
{
    public interface IRabbitMqService
    {
        IConnection CreateChannel();
    }
}
