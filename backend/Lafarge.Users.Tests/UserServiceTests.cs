using Lafarge.Users.Api.Data;
using Lafarge.Users.Api.Dtos;
using Lafarge.Users.Api.Models;
using Lafarge.Users.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lafarge.Users.Tests;

public class UserServiceTests
{
    private static UserService CreateService(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var ctx = new AppDbContext(options);
        return new UserService(ctx);
    }

    [Fact]
    public async Task Create_Then_Get_Works()
    {
        var svc = CreateService(nameof(Create_Then_Get_Works));
        var created = await svc.Create(new UserCreateDto
        {
            FirstName = "Olaoye",
            LastName = "Rasheed",
            Email = "olaoyerasheed@gmail.com",
            Role = UserRole.Admin
        }, null);

        Assert.True(created.Id > 0);
        var got = await svc.Get(created.Id);
        Assert.NotNull(got);
        Assert.Equal("ada@example.com", got!.Email);
    }

    [Fact]
    public async Task Search_By_Name_Works()
    {
        var svc = CreateService(nameof(Search_By_Name_Works));
        await svc.Create(new UserCreateDto { FirstName="John", LastName="Doe", Email="olaoyerasheed@gmail.com"}, null);
        await svc.Create(new UserCreateDto { FirstName="Jane", LastName="Smith", Email="rasheed@gmail.com"}, null);

        var page = await svc.Search("j", 1, 10);
        Assert.True(page.Total >= 2);
        Assert.Contains(page.Items, i => i.FirstName == "John");
        Assert.Contains(page.Items, i => i.FirstName == "Jane");
    }

    [Fact]
    public async Task Update_Enforces_Email_Uniqueness()
    {
        var svc = CreateService(nameof(Update_Enforces_Email_Uniqueness));
        var a = await svc.Create(new UserCreateDto { FirstName="A", LastName="A", Email="a@gmail.com"}, null);
        var b = await svc.Create(new UserCreateDto { FirstName="B", LastName="B", Email="b@gmail.com"}, null);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => {
            await svc.Update(b.Id, new UserUpdateDto {
                FirstName = "B",
                LastName = "B",
                Email = "a@x.com"
            }, null);
        });
    }

    [Fact]
    public async Task BulkDelete_Works()
    {
        var svc = CreateService(nameof(BulkDelete_Works));
        var u1 = await svc.Create(new UserCreateDto { FirstName="Ola", LastName="L", Email="ola@gmail.com"}, null);
        var u2 = await svc.Create(new UserCreateDto { FirstName="Rasheed", LastName="L", Email="ola2@gmail.com"}, null);

        var deleted = await svc.BulkDelete(new [] { u1.Id, u2.Id });
        Assert.Equal(2, deleted);
    }
}
