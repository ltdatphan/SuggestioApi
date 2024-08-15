using Microsoft.AspNetCore.Mvc;

namespace SuggestioApi.Controllers;

[Route("")]
[ApiController]
public class Base : ControllerBase
{
    public Base()
    {
        
    }

    [HttpGet]
    public async Task<IActionResult> Hello()
    {
        return Ok("Hello from Suggestio Api");
    }
}