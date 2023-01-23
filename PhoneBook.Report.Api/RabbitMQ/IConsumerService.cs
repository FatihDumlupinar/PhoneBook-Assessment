using System.Threading;
using System.Threading.Tasks;

namespace PhoneBook.Report.Api.RabbitMQ
{
    public interface IConsumerService
    {
        Task ReadQueue(CancellationToken cancellationToken);
    }
}
