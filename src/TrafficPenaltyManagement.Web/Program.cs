using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Application.Services;
using TrafficPenaltyManagement.Infrastructure.Identity;
using TrafficPenaltyManagement.Infrastructure.Persistence;
using TrafficPenaltyManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt=>opt.UseSqlServer(connectionString));

//Þifre Kontrolleri
builder.Services
	.AddDefaultIdentity<ApplicationUser>(options =>
	{
		options.SignIn.RequireConfirmedAccount = false;

		options.Password.RequiredLength = 8;
		options.Password.RequireDigit = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireNonAlphanumeric = false;
	})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddErrorDescriber<TurkishIdentityErrorDescriber>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ITrafficPenaltyService, TrafficPenaltyService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider
		.GetRequiredService<RoleManager<IdentityRole>>();

	string[] roles =
	{
		"Employee",
		"Manager",
		"Finance",
		"Admin"
	};

	foreach (var role in roles)
	{
		if (!await roleManager.RoleExistsAsync(role))
		{
			var result = await roleManager.CreateAsync(
				new IdentityRole(role));

			if (!result.Succeeded)
			{
				var errors = string.Join(
					", ",
					result.Errors.Select(error => error.Description));

				throw new InvalidOperationException(
					$"'{role}' rolü oluþturulamadý: {errors}");
			}
		}
	}
}

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
