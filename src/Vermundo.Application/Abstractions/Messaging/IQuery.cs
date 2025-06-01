using Vermundo.Domain.Abstractions;
using MediatR;

namespace Vermundo.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
