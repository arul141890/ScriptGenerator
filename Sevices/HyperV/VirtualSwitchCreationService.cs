using Core.Domain.Hyperv;
using Data.UnitOfWorks;


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
