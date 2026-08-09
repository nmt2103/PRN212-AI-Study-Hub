namespace PRN212.AIStudyHub.Application.DTOs.Auth;

public class UserDto(Guid id, string email, string firstName, string lastName, string role, bool isActive, DateTime createdAt, DateTime? updatedAt)
{
  public Guid Id { get; set; } = id;
  public string Email { get; set; } = email;
  public string FirstName { get; set; } = firstName;
  public string LastName { get; set; } = lastName;
  public string Role { get; set; } = role;
  public bool IsActive { get; set; } = isActive;
  public DateTime CreatedAt { get; set; } = createdAt;
  public DateTime? UpdatedAt { get; set; } = updatedAt;
}