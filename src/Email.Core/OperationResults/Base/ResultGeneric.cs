
namespace Email.Core.OperationResults.Base
{
    /// <summary>
    /// Обобщенный класс Result для представления результата операции с возможностью возвращения значения.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения в случае успеха.</typeparam>
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public T Value { get; }

        /// <summary>
        /// Конструктор для успешного результата
        /// </summary>
        /// <param name="value"></param>
        protected Result(T value)
        {
            IsSuccess = true;
            Error = Error.None;
            Value = value;
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

        public static Result<T> Success(T value) => new Result<T>(value);

        public static Result<T> Failure(Error error) => new Result<T>(error);
    }
}
