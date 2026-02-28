
using Email.Core.OperationResults.Base;

namespace Email.Core.OperationResults.Errors
{
    public static class EmailErrors
    {
        public const string ErrorSendCode = "Email.ErrorSend";

        public static Error SendError() => new Error(ErrorSendCode, $"Failed to send email");

        public static string InvalidEmail() => "Некорректный email адрес";
        public static string NoData() => "Отсутствуют данные для отправки";
        public static Error NoImage() => new Error(" ", "Изображение не найдено");
    }
}
