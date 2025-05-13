using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models
{
    public enum ApplicationStatus
    {
        IN_PROGRESS, COMPLETED, REJECTED, CANCELED
    }

    public enum UserRole
    {
        CITIZEN,
        ADMIN,
        CIVILSERVANT
    }
}
