using Core.Domain;
using Core.Domain.Hyperv;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.HyperV
{
    public class VirtualSwitchCreationService : ScriptGeneratorService<VirtualSwitchCreation>, IVirtualSwitchCreationService
    {
        public VirtualSwitchCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
