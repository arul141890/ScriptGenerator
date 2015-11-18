using Core.Domain;
using Data.Repositories;
using Data.UnitOfWorks;
using Sevices;
using Sevices.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevices
{
    public class UserService : ScriptGeneratorService<User>, IUserService
    {
        public UserService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }

        public bool AuthenticateUser(string userId, string md5)
        {
            var user = Uow.Users.FirstOrDefault(x => x.UserId == userId);

            return user != null && user.PasswordHash == md5;
        }
    }
}
