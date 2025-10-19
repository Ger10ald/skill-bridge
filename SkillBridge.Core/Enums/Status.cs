using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Enums
{
    public enum Status
    {
        Requested = 0,
        PendingHold = 1,
        Accepted = 2,
        Captured = 3,
        Declined = 4,
        Expired = 5,
        Canceled = 6
    }
}
