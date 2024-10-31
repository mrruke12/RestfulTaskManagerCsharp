using RestfulLanding.Database;
using Microsoft.AspNetCore.Identity;
using RestfulLanding.Middleware;


var builder = WebApplication.CreateBuilder(args);

await RuLocalization.Instance().Initialize();
await EnLocalization.Instance().Initialize();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppIdentityDbContext>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Home/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

var app = builder.Build();

app.Use(async (context, next) => {
    if (context.Request.Cookies.ContainsKey("UserLanguage")) {
        var lang = context.Request.Cookies["UserLanguage"];
        LocalizationManager.Set(lang);
    } else LocalizationManager.Set("RU");

    await next.Invoke();
});

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ProfileMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

