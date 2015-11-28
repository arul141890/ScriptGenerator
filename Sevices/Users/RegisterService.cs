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
    public class RegisterService : ScriptGeneratorService<User>, IRegisterService
    {
        public RegisterService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }

      
    }
}
