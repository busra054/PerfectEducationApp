using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
// Dosya yükleme limiti (20MB) Öğretmenlerin sertifikaları için 
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024*1024*50;
});


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Mesajlaşma için ekledik.
builder.Services.AddHttpClient(); // OpenAI API için

builder.Services.AddHttpClient("OpenAI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenAI:ApiUrl"]);
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", builder.Configuration["OpenAI:ApiKey"]);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("WebApplication_Deneme")
    ).EnableSensitiveDataLogging());

// Program.cs
builder.Services.AddIdentity<User, IdentityRole<int>>() // Rol tipini belirtin
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie yapılandırması (tek şema, varsayılan yapı)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Index"; // Giriş yapılmazsa buraya yönlendir
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Session desteği ekleniyor (kullanıcı bilgilerini tutabilmek için)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Oturum süresi
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
        var roles = new[] { "Admin", "Öğretmen", "Öğrenci" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Rol oluşturma hatası");
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

app.MapHub<WebApplication_Deneme.Hubs.ChatHub>("/chathub");

app.Run();
