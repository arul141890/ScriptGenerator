using Core.Domain;
using Core.Domain.Filestorage;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Filestorage
{
    public class SmbsharecreationService : ScriptGeneratorService<Smbsharecreation>, ISmbsharecreationService
    {
        public SmbsharecreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
