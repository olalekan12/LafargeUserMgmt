# Lafarge User Management (MSSQL + ASP.NET Core)

This is a complete solution for the assessment. It includes:
- **ASP.NET Core Web API (.NET 8)** with **Entity Framework Core (SQL Server)**.
- CRUD for Users, search (name/email/phone), **bulk delete**, and **picture upload**.
- **Swagger** docs at `/swagger`.
- Minimal **HTML/JS frontend** to call the API.
- **xUnit tests** for the service layer.
- Postman collection in `postman_collection/`.

## Quick start

1. **Prerequisites**
   - .NET 8 SDK
   - SQL Server (local or remote)

2. **Configure DB**
   - Update the connection string in `backend/Lafarge.Users.Api/appsettings.Development.json` (or `appsettings.json`).
   - Create the DB (if not already): `dotnet ef database update`

3. **Run API**
   ```bash
   cd backend/Lafarge.Users.Api
   dotnet build
   dotnet run
   ```

4. **Open Swagger**
   - Navigate to http://localhost:5169/swagger

5. **Open Frontend**
   - Simply open `frontend/index.html` in your browser. Ensure CORS origin matches API URL in `frontend/script.js`.

## Database schema (MSSQL)

```sql
CREATE TABLE [dbo].[Users](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL UNIQUE,
    [Phone] NVARCHAR(32) NULL,
    [Gender] NVARCHAR(16) NULL, -- 'Male' | 'Female' | 'Other'
    [DateOfBirth] DATE NULL,
    [Nationality] NVARCHAR(100) NULL,
    [Role] NVARCHAR(16) NOT NULL, -- 'Admin' | 'User'
    [PictureUrl] NVARCHAR(512) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2 NULL
);
CREATE INDEX IX_Users_NameEmailPhone ON [dbo].[Users]([FirstName],[LastName],[Email],[Phone]);
```

> EF Core migrations are included via the model and DbContext; run `dotnet ef migrations add InitialCreate && dotnet ef database update` if you want migrations files.

## Testing
```bash
cd backend/Lafarge.Users.Tests
dotnet test
```

## Postman
Import `postman_collection/lafarge-user-mgmt.postman_collection.json`

