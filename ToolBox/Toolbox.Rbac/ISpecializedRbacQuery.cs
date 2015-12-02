using System.Collections.Generic;
using System.Security.Principal;

namespace Toolbox.Rbac
{
    public interface ISpecializedRbacQuery : IRbacQuery
    {
        IEnumerable<string> GetUserRoles<T>(IPrincipal user, T resource);
        IEnumerable<string> GetUserPermissions<T>(IPrincipal user, T resource);
        bool IsUserInRole<T>(IPrincipal user, string role, T resource);
        bool IsUserAbleTo<T>(IPrincipal user, string action, T resource);
    }
}