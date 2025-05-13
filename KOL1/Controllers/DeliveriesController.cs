using KOL1.Services;
using Microsoft.AspNetCore.Mvc;

namespace KOL1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveriesController : ControllerBase
{
    private readonly IDbService _dbService;

    public DeliveriesController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveries(int id)
    {
        var res = await _dbService.GetDelivery(id);
        
        
        return Ok(res);
    }
}