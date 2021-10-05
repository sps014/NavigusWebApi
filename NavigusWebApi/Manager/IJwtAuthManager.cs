namespace NavigusWebApi.Manager
{
    public interface IJwtAuthManager
    {
        string Authenticate(string uid,string role);
    }
}
