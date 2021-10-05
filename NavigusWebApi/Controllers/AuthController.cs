using Microsoft.AspNetCore.Mvc;
using NavigusWebApi.Models;

namespace NavigusWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] UserModel user)
        {
            return Ok(user);
        }
    }
}
