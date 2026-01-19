using Email.Core.OperationResults.Base;

namespace Email.Api.BLL.Abstract
{
    public interface IEmailSender
    {
        Task<Result> SendEmail(string to, string subject, string body);
    }
}
