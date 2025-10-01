using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Common.Helpers
{
    public static class RoleHelper
    {
        public static bool UserHasAnyRole(IEnumerable<string> userRoles, IEnumerable<string> requiredRoles)
        {
            return userRoles.Intersect(requiredRoles).Any();
        }
    }
}
