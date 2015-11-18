using Core.Domain;
using Core.Domain.dhcp;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Dhcp
{
    public class DhcpinstallationService : ScriptGeneratorService<Dhcpinstallation>, IDhcpinstallationService
    {
        public DhcpinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
