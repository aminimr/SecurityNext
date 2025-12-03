namespace Host.WebAPI.Services;

public class PluginMonitorService : BackgroundService
{
    private readonly ILogger<PluginMonitorService> _logger;
    private readonly IModuleRegistry _moduleRegistry;
    private readonly IConfiguration _configuration;
    private FileSystemWatcher _watcher;
    
    public PluginMonitorService(
        ILogger<PluginMonitorService> logger,
        IModuleRegistry moduleRegistry,
        IConfiguration configuration)
    {
        _logger = logger;
        _moduleRegistry = moduleRegistry;
        _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Plugin Monitor Service started");
        
        var pluginsPath = _configuration["Plugins:Path"] ?? "./Plugins";
        
        // ایجاد فولدر اگر وجود ندارد
        if (!Directory.Exists(pluginsPath))
        {
            Directory.CreateDirectory(pluginsPath);
        }
        
        // راه‌اندازی FileSystemWatcher
        _watcher = new FileSystemWatcher(pluginsPath)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | 
                          NotifyFilters.FileName | 
                          NotifyFilters.DirectoryName,
            EnableRaisingEvents = true
        };
        
        // رویدادهای تغییر فایل
        _watcher.Created += OnPluginCreated;
        _watcher.Deleted += OnPluginDeleted;
        _watcher.Renamed += OnPluginRenamed;
        _watcher.Changed += OnPluginChanged;
        
        // نگه داشتن سرویس فعال
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
        
        _watcher.Dispose();
    }
    
    private void OnPluginCreated(object sender, FileSystemEventArgs e)
    {
        if (e.Name.EndsWith(".dll") || e.Name == "manifest.json")
        {
            _logger.LogInformation($"New plugin file detected: {e.Name}");
            // بارگذاری ماژول جدید
            Task.Run(async () => await LoadNewModuleAsync(e.FullPath));
        }
    }
    
    private void OnPluginDeleted(object sender, FileSystemEventArgs e)
    {
        if (e.Name.EndsWith(".dll"))
        {
            var moduleName = Path.GetFileNameWithoutExtension(e.Name);
            _logger.LogInformation($"Plugin file removed: {moduleName}");
            // حذف ماژول از حافظه
            _moduleRegistry.Unregister(moduleName);
        }
    }
    
    private async Task LoadNewModuleAsync(string assemblyPath)
    {
        try
        {
            // منطق بارگذاری ماژول جدید
            // این می‌تواند یک ری‌استارت نرم باشد
            // یا بارگذاری داینامیک
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to load module from {assemblyPath}");
        }
    }
}