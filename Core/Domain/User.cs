using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    public class User : BaseModel
    {
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string UserId { get; set; }

    }
}
