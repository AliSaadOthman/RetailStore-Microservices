using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Customer", "Inventory Manager", "Order Manager", "Payment Manager", "Admin" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the roles and seed them to the database
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Define claims for each role
        var claims = new Dictionary<string, string[]>
        {
            { "Customer", new[] { "view_products", "place_order", "view_order_status" } },
            { "Inventory Manager", new[] { "view_inventory", "update_inventory", "manage_products" } },
            { "Order Manager", new[] { "view_orders", "update_order_status", "process_returns" } },
            { "Payment Manager", new[] { "process_payments", "refund_payments", "view_payment_history" } },
            { "Admin", new[] { "manage_users", "assign_roles", "view_all_reports" } }
        };

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            foreach (var claim in claims[roleName])
            {
                if (!(await roleManager.GetClaimsAsync(role)).Any(c => c.Type == claim))
                {
                    await roleManager.AddClaimAsync(role, new Claim(claim, "true"));
                }
            }
        }

        // Optionally seed an admin user
        var adminUser = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
        var user = await userManager.FindByEmailAsync(adminUser.Email);

        if (user == null)
        {
            var createPowerUser = await userManager.CreateAsync(adminUser, "Admin@12345");
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
