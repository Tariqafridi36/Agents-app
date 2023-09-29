using Microsoft.AspNetCore.Mvc;

namespace Agents_app.Controllers;

[ApiController]
[Route("[controller]")]
public class Index : ControllerBase
{


    private readonly ILogger<Index> _logger;

    public Index(ILogger<Index> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "StartChart")]
    public void Get()
    {
        var obj = new ChatService();
        obj.Run();

    }
}
