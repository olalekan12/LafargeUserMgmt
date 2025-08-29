using Lafarge.Users.Api.Dtos;
using Lafarge.Users.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lafarge.Users.Api.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _svc;
    private readonly IWebHostEnvironment _env;

    public UsersController(IUserService svc, IWebHostEnvironment env)
    {
        _svc = svc;
        _env = env;
    }

    // ==================== LIST & SEARCH ====================
    public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 20)
    {
        var result = await _svc.Search(q, page, pageSize);
        ViewBag.SearchQuery = q;
        return View(result);
    }

    // ==================== CREATE ====================
    //  public IActionResult Create() => View();

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(UserCreateDto dto, IFormFile? picture, CancellationToken ct)
    //{
    //    if (!ModelState.IsValid)
    //        return View(dto);

    //    string? pictureUrl = null;
    //    if (picture is not null && picture.Length > 0)
    //    {
    //        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
    //        Directory.CreateDirectory(uploadsDir);
    //        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(picture.FileName)}";
    //        var fullPath = Path.Combine(uploadsDir, fileName);

    //        using var fs = new FileStream(fullPath, FileMode.Create);
    //        await picture.CopyToAsync(fs, ct);

    //        pictureUrl = $"/uploads/{fileName}";
    //    }

    //    await _svc.CreateAsync(dto, pictureUrl, ct);
    //    return RedirectToAction(nameof(Index));
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto dto, IFormFile? picture)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            string? pictureUrl = null;
            if (picture is not null && picture.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(picture.FileName)}";
                var fullPath = Path.Combine(uploadsDir, fileName);

                using var fs = new FileStream(fullPath, FileMode.Create);
                await picture.CopyToAsync(fs);

                pictureUrl = $"/uploads/{fileName}";
            }

            await _svc.Create(dto, pictureUrl);
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {            
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }


    //public async Task<IActionResult> Edit(int id, CancellationToken ct)
    //{
    //    var user = await _svc.GetAsync(id, ct);
    //    if (user == null) return NotFound();

    //    var dto = new UserUpdateDto
    //    {
    //        FirstName = user.FirstName,
    //        LastName = user.LastName,
    //        Email = user.Email,
    //        Phone = user.Phone,
    //        Gender = user.Gender,
    //        DateOfBirth = user.DateOfBirth,
    //        Nationality = user.Nationality,
    //        Role = user.Role
    //    };

    //    return View(dto);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserUpdateDto dto, IFormFile? picture)
    {
        try
        {
        if (!ModelState.IsValid)
            return View(dto);

        string? pictureUrl = null;
        if (picture is not null && picture.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(picture.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using var fs = new FileStream(fullPath, FileMode.Create);
            await picture.CopyToAsync(fs);
           
            pictureUrl = $"/uploads/{fileName}";
        }

        var updated = await _svc.Update(id, dto, pictureUrl);
        if (updated == null) return NotFound();
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));

        }
        catch (Exception ex)
        {

            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Delete(int id, CancellationToken ct)
    //{
    //    var success = await _svc.DeleteAsync(id, ct);
    //    if (!success) return NotFound();

    //    return RedirectToAction(nameof(Index));
    //}

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultiple(int[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            TempData["ErrorMessage"] = "No users were selected to delete.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _svc.BulkDelete(ids.Distinct());
            TempData["SuccessMessage"] = $"{ids.Length} user(s) deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

}

