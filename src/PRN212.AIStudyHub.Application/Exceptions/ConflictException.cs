namespace PRN212.AIStudyHub.Application.Exceptions;

public class ConflictException : Exception
{
  public ConflictException(string message) : base(message) { }
}
