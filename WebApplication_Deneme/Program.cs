using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
// Dosya y�kleme limiti (20MB) ��retmenlerin sertifikalar� i�in 
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024*1024*50;
});



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging()); // Use connection string from appsettings.json

// Program.cs
builder.Services.AddIdentity<User, IdentityRole<int>>() // Rol tipini belirtin
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Identity'in varsay�lan cookie ayarlar�n� �zelle�tirin:
builder.Services.ConfigureApplicationCookie(options =>
{
   // options.LoginPath = "/Account/Index";
    options.LoginPath = "/Admins/Login"; // Giri� yapmazsa buraya y�nlendirsin

    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthentication()
    .AddCookie("AdminCookies", options => // Adminler i�in yeni cookie scheme
    {
        options.LoginPath = "/Admins/Login";
        options.AccessDeniedPath = "/Admins/AccessDenied";
    });

// Session deste�i ekleniyor (kullan�c� bilgilerini tutabilmek i�in)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Oturum s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var roles = new[] { "Admin", "��retmen", "��renci" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Rol olu�turma hatas�");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication middleware
app.UseAuthorization(); // Authorization middleware
app.UseSession(); // Session middleware'ini burada ekleyin


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
