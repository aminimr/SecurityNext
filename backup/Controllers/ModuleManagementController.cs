namespace Host.WebAPI.Controllers;

[ApiController]
[Route("api/admin/modules")]
[Authorize(Roles = "Admin")]
public class ModuleManagementController : ControllerBase
{
    private readonly IModuleManagementService _moduleService;
    private readonly ILogger<ModuleManagementController> _logger;
    
    public ModuleManagementController(
        IModuleManagementService moduleService,
        ILogger<ModuleManagementController> logger)
    {
        _moduleService = moduleService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetModules()
    {
        var modules = await _moduleService.GetAllModulesAsync();
        return Ok(modules);
    }
    
    [HttpGet("{moduleId}")]
    public async Task<IActionResult> GetModule(string moduleId)
    {
        var module = await _moduleService.GetModuleAsync(moduleId);
        if (module == null)
            return NotFound();
            
        return Ok(module);
    }
    
    [HttpPost("install")]
    public async Task<IActionResult> InstallModule([FromForm] InstallModuleRequest request)
    {
        if (request.ModulePackage == null || request.ModulePackage.Length == 0)
            return BadRequest("Module package is required");
        
        try
        {
            var result = await _moduleService.InstallModuleAsync(request.ModulePackage);
            return Ok(new { Message = "Module installed successfully", ModuleId = result.ModuleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to install module");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
    
    [HttpPost("{moduleId}/uninstall")]
    public async Task<IActionResult> UninstallModule(string moduleId)
    {
        try
        {
            await _moduleService.UninstallModuleAsync(moduleId);
            return Ok(new { Message = "Module uninstalled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to uninstall module {moduleId}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
    
    [HttpPost("{moduleId}/enable")]
    public async Task<IActionResult> EnableModule(string moduleId)
    {
        try
        {
            await _moduleService.EnableModuleAsync(moduleId);
            return Ok(new { Message = "Module enabled" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to enable module {moduleId}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
    
    [HttpPost("{moduleId}/disable")]
    public async Task<IActionResult> DisableModule(string moduleId)
    {
        try
        {
            await _moduleService.DisableModuleAsync(moduleId);
            return Ok(new { Message = "Module disabled" });
        }
        {
            _logger.LogError(ex, $"Failed to disable module {moduleId}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
    
    [HttpPost("scan")]
    public async Task<IActionResult> ScanModules()
    {
        var discovered = await _moduleService.ScanForNewModulesAsync();
        return Ok(new { 
            Message = "Scan completed", 
            DiscoveredModules = discovered 
        });
    }
}