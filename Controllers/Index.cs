using Microsoft.AspNetCore.Mvc;

namespace Agents_app.Controllers;

[ApiController]
[Route("[controller]")]
public class Index : ControllerBase
{

  private ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Index> _logger;

    public Index(ILogger<Index> logger, ApplicationDbContext applicationDb)
    {
        _logger = logger;
        _applicationDbContext = applicationDb;
    }

    [HttpGet(Name = "StartChart")]
    public void Get()
    {
        var obj = new ChatService(_applicationDbContext);
        obj.Run();

    }
}
