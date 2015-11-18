using Core.Domain;
using Core.Domain.Filestorage;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Filestorage
{
    public class RoleinstallationService : ScriptGeneratorService<Roleinstallation>, IRoleinstallationService
    {
        public RoleinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
