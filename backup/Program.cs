using Core.PluginSystem;
using Host.WebAPI.Configurations;
using Host.WebAPI.Middlewares;
using Host.WebAPI.Services;

using Serilog;


var builder = WebApplication.CreateBuilder(args);

// 1. بارگذاری تنظیمات
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Modules.json", optional: true)
    .AddEnvironmentVariables();

// 2. ثبت سرویس‌های اصلی
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. ثبت سرویس‌های هاست
builder.Services.AddHostServices();
builder.Services.AddBackgroundServices();

// 4. بارگذاری و ثبت ماژول‌ها
var pluginScanner = new PluginScannerService();
var modules = pluginScanner.ScanModules("./Plugins");
builder.Services.RegisterModules(modules, builder.Configuration);

// 5. ثبت میدلورها
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<ModuleMiddleware>();

// 6. اضافه کردن Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddDiskStorageHealthCheck(options => options.AddDrive("C:\\", 1024))
    .AddUrlGroup(new Uri("http://localhost:5000/health"), "Self");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateBootstrapLogger();

Log.Information("Starting Web API...");

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "EcommerceAPI")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
});
}


var app = builder.Build();

// 7. پیکربندی Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = "api-docs";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 8. استفاده از میدلورها
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ModuleMiddleware>();

app.UseAuthorization();

// 9. مپ کردن کنترلرها
app.MapControllers();

// 10. مپ کردن endpointهای ویژه
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/system/info", async (ISystemInfoService service) =>
{
    return await service.GetSystemInfoAsync();
});

// 11. فعال کردن ماژول‌ها
await app.InitializeModulesAsync();

// 12. شروع مانیتورینگ پلاگین‌ها
app.Services.GetService<PluginMonitorService>()?.Start();

app.Run();

// برای تست‌های Integration
public partial class Program { }