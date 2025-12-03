namespace Host.WebAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // کانفیگ‌های اضافی اگر نیاز باشد
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // پیکربندی اضافی
    }
}