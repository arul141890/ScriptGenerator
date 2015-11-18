using Core.Domain;
using Core.Domain.dhcp;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Dhcp
{
    public class ScopeCreationService : ScriptGeneratorService<Scopecreation>, IScopeCreationService
    {
        public ScopeCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
