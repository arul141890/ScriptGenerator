using Core.Domain;
using Core.Domain.Hyperv;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.HyperV
{
    public class VirtualDiskCreationService : ScriptGeneratorService<VirtualDiskCreation>, IVirtualDiskCreationService
    {
        public VirtualDiskCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
