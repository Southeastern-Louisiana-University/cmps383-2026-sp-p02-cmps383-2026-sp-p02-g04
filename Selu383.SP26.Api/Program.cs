using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Locations;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));

// --- IDENTITY SETUP STARTED ---
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401; // Tells tests "Not Logged In" instead of a redirect
        return Task.CompletedTask;
    };
});
// --- IDENTITY SETUP ENDED ---

builder.Services.AddControllers(); // THIS LINE IS REQUIRED
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // FIX: Define 'services' here so the lines below can use it
    var services = scope.ServiceProvider; 
    var db = services.GetRequiredService<DataContext>();
    db.Database.Migrate();

    // Seed Locations
    if (!db.Locations.Any())
    {
        db.Locations.AddRange(
            new Location { Name = "Location 1", Address = "123 Main St", TableCount = 10 },
            new Location { Name = "Location 2", Address = "456 Oak Ave", TableCount = 20 },
            new Location { Name = "Location 3", Address = "789 Pine Ln", TableCount = 15 }
        );
        db.SaveChanges();
    }

    // --- SEEDING USERS STARTED ---
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new Role { Name = "Admin" });
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new Role { Name = "User" });
    }

    if (await userManager.FindByNameAsync("galkadi") == null)
    {
        var admin = new User { UserName = "galkadi" };
        await userManager.CreateAsync(admin, "Password123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    var testUsers = new[] { "bob", "sue" };
    foreach (var name in testUsers)
    {
        if (await userManager.FindByNameAsync(name) == null)
        {
            var user = new User { UserName = name };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // MUST be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }