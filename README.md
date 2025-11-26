# DomKultury — Community Portal 

Simple community/cultural center portal for managing events, classes, and users. Built with ASP.NET Core (.NET 8), Entity Framework Core, and Identity.

## Quick summary

- Framework: .NET 8 (ASP.NET Core)
- UI: Razor Pages / MVC (project contains MVC controllers and Razor pages)
- ORM: Entity Framework Core (SQL Server / SQLite supported)
- Authentication: ASP.NET Core Identity

## Technologies

- .NET 8
- ASP.NET Core (Razor Pages / MVC)
- EF Core (SqlServer, Sqlite, Npgsql available)
- ASP.NET Core Identity
- QuestPDF (PDF generation)
- Bootstrap for styling

## Prerequisites

- .NET 8 SDK
- LocalDB (SQL Server) for local development, or PostgreSQL if you prefer
- `dotnet-ef` tool (for migrations): `dotnet tool install --global dotnet-ef`

## Local setup

1. Restore packages:

   `dotnet restore`

2. Remove secrets from `appsettings.json` and use user-secrets for local secrets. In the project folder next to the `.csproj` run:

   ```powershell
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DomKulturyDB" "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DomKulturyDB;Integrated Security=True;"
   dotnet user-secrets set "WeatherSettings:ApiKey" "<YOUR_API_KEY>"
   ```

   Note: user-secrets are stored locally and are not committed to the repo.

3. Create and apply EF migrations (if migrations are not already included):

   ```powershell
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   If migrations are already present in the `Migrations/` folder, simply run `dotnet ef database update`.

4. Run the app:

   `dotnet run`

   Then open `https://localhost:5001` (or the URL shown in the console).

## Notes

- The project currently defaults to SQL Server LocalDB. To use PostgreSQL, change the DbContext registration in `Program.cs` to `UseNpgsql(...)` and provide a PostgreSQL connection string (and ensure `Npgsql.EntityFrameworkCore.PostgreSQL` is installed).

- Keep secrets out of version control. If secrets were already committed, rewrite history with a tool like `git filter-repo` or BFG — coordinate with collaborators before force-pushing.

- Migrations are tracked in the `Migrations/` folder and should be committed so others can migrate the same schema.

## Topics (suggested GitHub topics)

`aspnet-core`, `razor-pages`, `ef-core`, `identity`, `dotnet-8`, `razor`, `mvc`

---

If you want, I can also:
- add a short `CONTRIBUTING.md`,
- add a sanitized `appsettings.json` placeholder and commit it,
- or generate a `.gitattributes` file for line endings.
