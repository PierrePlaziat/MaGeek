using PlaziatTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaziatIdentity
{
    public interface IUserService
    {
        public bool RegisterUser(string name, string pass);
        public string? IdentifyUser(string name, string pass);
        public bool CheckToken(string token);

    }
}
