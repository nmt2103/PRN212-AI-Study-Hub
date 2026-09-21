namespace PRN212.AIStudyHub.Application.Exceptions;

public class UnauthorizedException : Exception
{
  public UnauthorizedException(string message) : base(message) { }
}
