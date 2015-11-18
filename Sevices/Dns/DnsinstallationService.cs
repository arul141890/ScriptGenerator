using Core.Domain;
using Core.Domain.dns;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.dns
{
    public class DnsinstallationService : ScriptGeneratorService<Dnsinstallation>, IDnsinstallationService
    {
        public DnsinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
