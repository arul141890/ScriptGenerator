using Core.Domain;
using Core.Domain.Remotedesktopservices;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Remotedesktopservices
{
    public class CollectioncreationService : ScriptGeneratorService<Collectioncreation>, ICollectioncreationService
    {
        public CollectioncreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
