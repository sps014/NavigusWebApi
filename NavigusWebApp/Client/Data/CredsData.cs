using System.Net.Http.Headers;
using NavigusWebApp.Shared.Models;

namespace NavigusWebApp.Client.Data
{
    public static class CredsData
    {
        public static string JWT { get; set; }
        public static Roles Role {  get; set; }
        public static string UserName {  get; set; }
        public static bool IsLogin =>JWT != null;
        public static AuthenticationHeaderValue HeaderJWT => new AuthenticationHeaderValue("bearer",JWT);

    }
}
