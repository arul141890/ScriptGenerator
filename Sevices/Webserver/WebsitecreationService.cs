using Core.Domain;
using Core.Domain.Webserver;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Webserver
{
    public class WebsitecreationService : ScriptGeneratorService<Websitecreation>, IWebsitecreationService
    {
        public WebsitecreationService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
