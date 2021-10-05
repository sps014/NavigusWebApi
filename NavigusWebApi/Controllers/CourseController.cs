using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NavigusWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }

        public CourseController(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        [Authorize]
        [HttpGet("list")]
        public IActionResult List()
        {
            return Ok(string.Join("\n",Accessor.HttpContext.User.Claims.Select(x=>x.Subject+" : "+x.Value)));
        }
    }
}
