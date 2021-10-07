using NavigusWebApi.Models;

namespace NavigusWebApp.Client.Data
{
    public static class CredsData
    {
        public static string UID { get; set; }
        public static string JWT { get; set; }
        public static Roles Role {  get; set; }
        public static bool IsLogin => UID != null && JWT != null;
        
    }
}
