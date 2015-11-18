using Core.Domain;
using Core.Domain.Hyperv;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.HyperV
{
    public class VMCreationService : ScriptGeneratorService<VMCreation>, IVMCreationService
    {
        public VMCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
