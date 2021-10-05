namespace NavigusWebApi.Manager
{
    public interface IJwtAuthManager
    {
        string Authenticate(string email,string role);
    }
}
