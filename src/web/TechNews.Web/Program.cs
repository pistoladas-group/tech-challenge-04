using TechNews.Common.Library.Middlewares;
using TechNews.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddHttpClient()
        .AddAuthConfiguration()
        .AddEnvironmentVariables(builder.Environment)
        .AddLoggingConfiguration(builder.Host)
        .ConfigureDependencyInjections()
        .AddControllersWithViews(options => options.Filters.AddFilterConfiguration());

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseLoggingConfiguration();
app.UseMiddleware<ResponseHeaderMiddleware>();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthConfiguration();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
