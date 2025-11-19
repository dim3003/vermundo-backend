using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Exceptions;

public sealed class DomainException : Exception
{
    public Error Error { get; }

    public DomainException(Error error)
        : base(error.Name)
    {
        Error = error;
    }
}

