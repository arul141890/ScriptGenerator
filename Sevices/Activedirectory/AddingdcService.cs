using Core.Domain;
using Core.Domain.Activedirectory;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Activedirectory
{
    public class AddingdcService : ScriptGeneratorService<Addingdc>, IAddingdcCreationService
    {
        public AddingdcService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
