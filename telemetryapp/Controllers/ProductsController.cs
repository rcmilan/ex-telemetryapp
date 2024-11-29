using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using telemetryapp.IO;

namespace telemetryapp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetProductOutput>> Get([FromServices] ILogger<ProductsController> logger)
    {
        ActivitySource tracingSource = new("Example.Source");

        using Activity activity = tracingSource.StartActivity("Hello {name}");

        var result = new GetProductOutput($"{Guid.CreateVersion7()}", 42069);

        activity?.SetTag("name", result.Name);

        return Ok(result);
    }
}
