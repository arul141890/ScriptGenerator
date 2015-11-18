using Core.Domain;
using Core.Domain.Webserver;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Webserver
{
    public class WebserverinstallationService : ScriptGeneratorService<Webserverinstallation>, IWebserverinstallationService
    {
        public WebserverinstallationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
