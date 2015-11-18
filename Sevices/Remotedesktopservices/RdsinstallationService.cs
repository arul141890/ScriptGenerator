using Core.Domain;
using Core.Domain.Remotedesktopservices;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Remotedesktopservices
{
    public class RdsinstallationService : ScriptGeneratorService<Rdsinstallation>, IRdsinstallationService
    {
        public RdsinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
