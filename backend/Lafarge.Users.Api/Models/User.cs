using System.ComponentModel.DataAnnotations;

namespace Lafarge.Users.Api.Models;

public enum UserRole { Admin, User }

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required, MaxLength(256), EmailAddress]
    public string Email { get; set; } = default!;

    [MaxLength(32)]
    public string? Phone { get; set; }

    [MaxLength(16)]
    public string? Gender { get; set; } // Male | Female | Other

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(100)]
    public string? Nationality { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.User;

    [MaxLength(512)]
    public string? PictureUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
