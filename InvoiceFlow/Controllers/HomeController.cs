using Microsoft.AspNetCore.Mvc;

namespace InvoiceFlow.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("InvoiceFlow API is running");
    }
}