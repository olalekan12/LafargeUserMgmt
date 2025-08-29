using Lafarge.Users.Api.Data;
using Lafarge.Users.Api.Dtos;
using Lafarge.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lafarge.Users.Api.Services;

public interface IUserService
{
    Task<UserReadDto> Create(UserCreateDto dto, string? pictureUrl);
    Task<UserReadDto?> Get(int id);
    Task<PagedResult<UserReadDto>> Search(string? q, int page, int pageSize);
    Task<UserReadDto?> Update(int id, UserUpdateDto dto, string? pictureUrl);
    Task<bool> Delete(int id);
    Task<int> BulkDelete(IEnumerable<int> ids);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<UserReadDto> Create(UserCreateDto dto, string? pictureUrl)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            Phone = dto.Phone?.Trim(),
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            Role = dto.Role,
            PictureUrl = pictureUrl
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ToReadDto(user);
    }

    public async Task<UserReadDto?> Get(int id)
    {
        var user = await _db.Users.FindAsync([id]);
        return user is null ? null : ToReadDto(user);
    }

    public async Task<PagedResult<UserReadDto>> Search(string? q, int page, int pageSize)
    {
        q = q?.Trim().ToLowerInvariant();
        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(q) ||
                u.LastName.ToLower().Contains(q) ||
                u.Email.ToLower().Contains(q) ||
                (u.Phone != null && u.Phone.ToLower().Contains(q)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => ToReadDto(u))
            .ToListAsync();

        return new PagedResult<UserReadDto> { Page = page, PageSize = pageSize, Total = total, Items = items };
    }

    public async Task<UserReadDto?> Update(int id, UserUpdateDto dto, string? pictureUrl)
    {
        var user = await _db.Users.FindAsync([id]);
        if (user is null) return null;

        // Email uniqueness check if changed
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
            if (exists) throw new InvalidOperationException("Email already exists.");
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.Trim().ToLowerInvariant();
        user.Phone = dto.Phone?.Trim();
        user.Gender = dto.Gender;
        user.DateOfBirth = dto.DateOfBirth;
        user.Nationality = dto.Nationality;
        user.Role = dto.Role;
        if (!string.IsNullOrWhiteSpace(pictureUrl))
            user.PictureUrl = pictureUrl;

        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToReadDto(user);
    }

    public async Task<bool> Delete(int id)
    {
        var user = await _db.Users.FindAsync([id]);
        if (user is null) return false;
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> BulkDelete(IEnumerable<int> ids)
    {
        var list = await _db.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        _db.Users.RemoveRange(list);
        return await _db.SaveChangesAsync();
    }

    private static UserReadDto ToReadDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Phone = u.Phone,
        Gender = u.Gender,
        DateOfBirth = u.DateOfBirth,
        Nationality = u.Nationality,
        Role = u.Role,
        PictureUrl = u.PictureUrl,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
