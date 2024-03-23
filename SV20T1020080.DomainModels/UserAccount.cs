using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.DomainModels
{
    public class UserAccount
    {
        public String UserID { get; set; }
        public String UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public String Email { get; set; } = "";

        public String Photo { get; set; } = "";

        public String Password { get; set; } = "";

        public String RoleNames { get; set; } = "";
    }
}
