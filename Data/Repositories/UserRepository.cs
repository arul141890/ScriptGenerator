using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessModule.Core;
using DataAccessModule.Data.Repositories;

namespace Data.Repositories
{
    public class UserRepository : Repository, IUserRepository
    {

    }

    public interface IUserRepository : IRepository
    {
    }
}
