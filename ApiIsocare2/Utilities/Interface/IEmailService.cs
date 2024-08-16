using System.Threading.Tasks;

namespace ApiIsocare2.Utilities.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);

    }
}
