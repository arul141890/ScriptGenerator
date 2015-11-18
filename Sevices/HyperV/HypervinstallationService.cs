using Core.Domain;
using Core.Domain.Hyperv;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.HyperV
{
    public class HypervinstallationService : ScriptGeneratorService<Hypervinstallation>, IHypervinstallationService
    {
        public HypervinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
