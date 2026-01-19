
using Email.Core.OperationResults.Base;

namespace Email.Core.OperationResults.Errors
{
    public static class EmailErrors
    {
        public const string ErrorSendCode = "Email.ErrorSend";

        public static Error SendError() => new Error(ErrorSendCode, $"Failed to send email");
    }
}
