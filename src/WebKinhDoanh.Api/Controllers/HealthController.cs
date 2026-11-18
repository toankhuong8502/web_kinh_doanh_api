using Microsoft.AspNetCore.Mvc;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/health")]
public sealed class HealthController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get() => Success(new { status = "healthy" });
}
