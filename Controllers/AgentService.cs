using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Agents_app.Controllers;

[ApiController]
[Route("[controller]")]
public class AgentServiceController : ControllerBase
{

  // private ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Index> _logger;

     private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // public AgentServiceController(ILogger<Index> logger, ApplicationDbContext applicationDb)
    // {
    //     _logger = logger;
    //    // _applicationDbContext = applicationDb;
    // }

    [HttpGet(Name = "StartChart")]
    public IEnumerable<string> Get()
    {
      return Summaries;
       // var obj = new ChatService(_applicationDbContext);
       // obj.Run(); 
        

    }
}
