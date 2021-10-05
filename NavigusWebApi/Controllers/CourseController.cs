using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApi.Extensions;

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

        [Authorize(Roles ="Student,Teacher")]
        [HttpGet("list")]
        public IActionResult List()
        {
            return Ok(Accessor.GetUid());
        }
    }
}
