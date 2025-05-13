using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models
{
    public class AuthSession
    {
        public User? CurrentUser { get; set; }
        public bool IsActive => CurrentUser != null;
    }
}
