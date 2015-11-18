using Core.Domain;
using Core.Domain.Remotedesktopservices;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Remotedesktopservices
{
    public class ApppublishService : ScriptGeneratorService<Apppublish>, IApppublishService
    {
        public ApppublishService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
