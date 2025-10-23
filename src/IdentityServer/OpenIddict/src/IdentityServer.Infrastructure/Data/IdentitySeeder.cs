using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Infrastructure.Data;

public class IdentitySeeder
{
    private readonly IServiceProvider _serviceProvider;

    public IdentitySeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Admin User
        await SeedAdminUserAsync(userManager);
    }

    private async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        string[] roles = { "Admin", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = roleName == "Admin" ? "Full system access" : "Standard user access"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }

    private async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin@123456";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                // Add to Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Also add User role for compatibility
                await userManager.AddToRoleAsync(adminUser, "User");

                Console.WriteLine("✅ Admin user created successfully!");
                Console.WriteLine($"   Email: {adminEmail}");
                Console.WriteLine($"   Password: {adminPassword}");
            }
            else
            {
                Console.WriteLine("❌ Failed to create admin user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   - {error.Description}");
                }
            }
        }
        else
        {
            // Ensure admin has Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("✅ Added Admin role to existing admin user");
            }
        }
    }
}
