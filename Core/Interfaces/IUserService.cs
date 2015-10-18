namespace Core.Interfaces
{
    public interface IUserService
    {
        bool AuthenticateUser(string text, string md5);
    }
}
