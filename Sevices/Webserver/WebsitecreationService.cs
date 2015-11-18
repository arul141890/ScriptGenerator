using Core.Domain;
using Core.Domain.Webserver;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Webserver
{
    public class WebsiteCreationService : ScriptGeneratorService<Websitecreation>, IWebsitecreationService
    {
        public WebsiteCreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
