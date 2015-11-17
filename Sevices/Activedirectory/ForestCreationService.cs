using Core.Domain;
using Core.Domain.Activedirectory;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Activedirectory
{
    public class ForestCreationService : ScriptGeneratorService<Forestcreation>, IForestCreationService
    {
        public ForestCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
