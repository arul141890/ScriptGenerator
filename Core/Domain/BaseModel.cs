using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CreatedBy { get; set; }

    }
}
