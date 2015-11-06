using Core.Domain;
namespace Sevices.Users
{
    public interface IUserService : IScriptGeneratorService<User>
    {
        bool AuthenticateUser(string text, string md5);
    }
}
