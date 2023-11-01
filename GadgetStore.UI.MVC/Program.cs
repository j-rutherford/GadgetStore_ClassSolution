using GadgetStore.UI.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GadgetStore.DATA.EF.Models;
//Block 2 - Implementing Entity Framework:
//Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 6.0.5
//make sure the UI layer and Data layer have the same version installed. We have 6.0.5

//run the command
// Scaffold-dbcontext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -outputdir Models
//Run that ON the data layer (default project dropdown in the NuGet Package Manager Console).

//Register the new DbContext - add the using ...Models statement above, and add a reference to the data layer.
namespace GadgetStore.UI.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // Add the new GadgetStoreContext to the Service (add the using DATA.EF.Models above)
            builder.Services.AddDbContext<GadgetStoreContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()//added to register roles
                .AddRoleManager<RoleManager<IdentityRole>>()//added to register role manager
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();//do I know you? (are you registered?)
            app.UseAuthorization();//Do you have permission to see this page? (role verification)

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}