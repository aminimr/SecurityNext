namespace Host.WebAPI.Configurations;

public static class DependencyConfiguration
{
    public static IServiceCollection AddHostServices(this IServiceCollection services)
    {
        // سرویس‌های هاست
        services.AddSingleton<IModuleRegistry, ModuleRegistry>();
        services.AddSingleton<IPluginScanner, PluginScannerService>();
        services.AddScoped<ISystemInfoService, SystemInfoService>();
        services.AddScoped<IModuleManagementService, ModuleManagementService>();
        
        // Background Services
        services.AddHostedService<PluginMonitorService>();
        services.AddHostedService<HealthCheckService>();
        
        // Database Context برای هاست
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        
        // Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetConnectionString("Redis");
        });
        
        // HttpClient برای ارتباط با ماژول‌ها
        services.AddHttpClient("ModuleClient")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            });
        
        // SignalR برای ارتباط real-time با ماژول‌ها
        services.AddSignalR();
        
        return services;
    }
    
    public static IServiceCollection RegisterModules(
        this IServiceCollection services, 
        IEnumerable<ModuleInfo> modules, 
        IConfiguration configuration)
    {
        var moduleRegistry = services.BuildServiceProvider()
            .GetRequiredService<IModuleRegistry>();
        
        foreach (var moduleInfo in modules)
        {
            try
            {
                // بارگذاری Assembly ماژول
                var assembly = Assembly.LoadFrom(moduleInfo.AssemblyPath);
                var moduleType = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(IPluginModule).IsAssignableFrom(t));
                
                if (moduleType != null)
                {
                    // ایجاد نمونه ماژول
                    if (Activator.CreateInstance(moduleType) is IPluginModule module)
                    {
                        // کانفیگ کردن سرویس‌های ماژول
                        module.ConfigureServices(services, configuration);
                        
                        // ثبت در رجیستری
                        moduleRegistry.Register(module);
                        
                        // بارگذاری کنترلرهای ماژول
                        services.AddControllers()
                            .AddApplicationPart(assembly);
                    }
                }
            }
            catch (Exception ex)
            {
                // لاگ خطا
                Console.WriteLine($"Failed to load module {moduleInfo.Name}: {ex.Message}");
            }
        }
        
        return services;
    }
}