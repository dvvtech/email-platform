using Email.Api.BLL.Models;
using Email.Api.BLL.Services.MppTests;
using Email.Core.OperationResults.Base;

namespace Email.Api.BLL.Abstract
{
    public interface IEmailSender
    {
        Task<Result> SendEmail(string to, string subject, string body);

        Task<Result> SendTestResults(string to, EmailDataDto emailData, byte[] imageBytes, string imageFileName = "test-result.png");
    }
}
