using System.Collections.Generic;
using System.Security.Principal;

namespace Toolbox.Rbac
{
    public interface IRbacQuery
    {
        IEnumerable<string> GetUserRoles(IPrincipal user);
        IEnumerable<string> GetUserPermissions(IPrincipal user);
        bool IsUserInRole(IPrincipal user, string role);
        bool IsUserAbleTo(IPrincipal user, string action);
        IEnumerable<string> GetRolePermissions(string roleName);
        bool IsRoleAbleTo(string roleName, string action);
    }
}