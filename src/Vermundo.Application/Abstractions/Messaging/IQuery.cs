using MediatR;
using Vermundo.Domain.Abstractions;

namespace Vermundo.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
