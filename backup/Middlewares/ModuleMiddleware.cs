namespace Host.WebAPI.Middlewares;

public class ModuleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IModuleRegistry _moduleRegistry;
    private readonly ILogger<ModuleMiddleware> _logger;

    public ModuleMiddleware(
        RequestDelegate next,
        IModuleRegistry moduleRegistry,
        ILogger<ModuleMiddleware> logger)
    {
        _next = next;
        _moduleRegistry = moduleRegistry;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        
        // چک کردن اگر مسیر مربوط به ماژولی است که غیرفعال است
        foreach (var module in _moduleRegistry.GetDisabledModules())
        {
            if (path.StartsWithSegments($"/api/{module.Name.ToLower()}"))
            {
                _logger.LogWarning($"Attempt to access disabled module: {module.Name}");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync($"Module '{module.Name}' is currently disabled");
                return;
            }
        }

        // چک کردن دسترسی‌های ماژول
        var requestedModule = _moduleRegistry.GetModuleByPath(path);
        if (requestedModule != null)
        {
            var user = context.User;
            if (!await HasModuleAccess(user, requestedModule))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }

    private async Task<bool> HasModuleAccess(ClaimsPrincipal user, IPluginModule module)
    {
        // منطق چک کردن دسترسی
        return true; // یا منطق پیچیده‌تر
    }
}