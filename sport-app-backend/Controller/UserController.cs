using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Data;

namespace sport_app_backend.Controller;
[Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
       private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext dbContext)
    {
        _context = dbContext;


    }
     [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("test");
    }

    
}
