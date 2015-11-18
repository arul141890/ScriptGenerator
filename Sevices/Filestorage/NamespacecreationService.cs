using Core.Domain;
using Core.Domain.Filestorage;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Filestorage
{
    public class NamespacecreationService : ScriptGeneratorService<Namespacecreation>, INamespacecreationService
    {
        public NamespacecreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
