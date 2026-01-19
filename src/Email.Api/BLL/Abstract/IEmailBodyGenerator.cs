using Email.Models;

namespace Email.Api.BLL.Abstract
{
    public interface IEmailBodyGenerator
    {
        string GenerateEmailBody(SendEmailRequest request);
    }
}
