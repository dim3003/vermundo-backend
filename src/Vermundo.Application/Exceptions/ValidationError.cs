namespace Vermundo.Application.Exceptions;

public record ValidationError(string PropertyName, string ErrorMessage);
