using Core.Domain;
using Core.Domain.Activedirectory;
using Data.UnitOfWorks;
using Sevices.Users;

namespace Sevices.Activedirectory
{
    public class AddingrodcService : ScriptGeneratorService<Addingrodc>, IAddingrodcService
    {
        public AddingrodcService(IScriptGeneratorUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
