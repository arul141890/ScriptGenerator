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

namespace Sevices.Users
{
    public class Changepassword : ScriptGeneratorService<User>, IChangepassword
    {
        public Changepassword(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }

      
    }
}
