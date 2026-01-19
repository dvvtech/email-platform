
namespace Email.Core.OperationResults.Base
{
    /// <summary>
    /// Представляет ошибку, возвращаемую классами Result и Result<T>.
    /// </summary>
    /// <param name="Code">Код ошибки</param>
    /// <param name="Description">Детальное описание ошибки</param>
    public sealed record Error(string Code, string? Description = null)
    {
        public static readonly Error None = new(string.Empty);
    }
}
