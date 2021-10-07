using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NavigusWebApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUid(this IHttpContextAccessor http)
        {
            var uid = http.HttpContext.User.Claims.
                    First(x => x.Type.EndsWith("nameidentifier")).Value;
            return uid;
        }
    }
}
