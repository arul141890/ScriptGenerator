using Core.Domain;
using Core.Interfaces;
using Data.Repositories;
using DataAccessModule.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository = new UserRepository();

        public bool AuthenticateUser(string text, string md5)
        {
            var user =
                this._repository.SingleOrDefault<User>(
                    new TypeFilters<User>().And(x => x.UserId, ConditionType.EqualTo, text));

            return user != null && user.PasswordHash == md5;
        }
    }
}
