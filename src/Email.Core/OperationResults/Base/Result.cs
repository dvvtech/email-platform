
namespace Email.Core.OperationResults.Base
{
    /// <summary>
    /// Класс Result для представления результата операции без возвращаемого значения.
    /// </summary>
    public sealed class Result
    {
        /// <summary>
        /// Конструктор для успешного результата
        /// </summary>
        protected Result()
        {
            IsSuccess = true;
            Error = Error.None;
        }

        /// <summary>
        /// Конструктор для неудачного результата
        /// </summary>
        /// <param name="error">Объект ошибки, содержащий информацию об ошибке.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если предоставленный объект ошибки равен Error.None (ошибки нет).</exception>
        protected Result(Error error)
        {
            if (error == Error.None)
            {
                throw new ArgumentException("Error cannot be None for a failure result", nameof(error));
            }

            IsSuccess = false;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new Result();

        public static Result Failure(Error error) => new Result(error);
    }
}
