using System.ComponentModel.DataAnnotations;
using Lafarge.Users.Api.Models;

namespace Lafarge.Users.Api.Dtos;

public class UserCreateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = default!;
    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = default!;
    [MaxLength(32)]
    public string? Phone { get; set; }
    [MaxLength(16)]
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [MaxLength(100)]
    public string? Nationality { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
}

public class UserUpdateDto : UserCreateDto {}

public class UserReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public UserRole Role { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}

public class BulkDeleteRequest
{
    public List<int> Ids { get; set; } = new();
}
