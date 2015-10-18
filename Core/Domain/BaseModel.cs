using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    using CommonUtilitiesModule;

    public abstract class BaseModel
    {

        protected BaseModel()
        {
            this.CreatedDate = DateTimeHelper.Now;
        }

        public DateTime CreatedDate { get; set; }

        public int Id { get; set; }

    }
}
