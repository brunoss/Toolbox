using System.Security.Principal;

namespace Toolbox.Rbac
{
    public interface IRbacQuery
    {
        bool IsUserInRole(IPrincipal user, string role);
        bool IsUserAbleTo(IPrincipal user, string action);
        bool IsUserInRole<T>(IPrincipal user, string role, T resource);
        bool IsUserAbleTo<T>(IPrincipal user, string action, T resource);
    }
}