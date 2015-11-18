using Core.Domain;
using Core.Domain.dns;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.dns
{
    public class DnsrecordCreationService : ScriptGeneratorService<Dnsrecordcreation>, IDnsrecordCreationService
    {
        public DnsrecordCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
