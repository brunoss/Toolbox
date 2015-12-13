using System.Security.Principal;

namespace Toolbox.Rbac
{
    public interface IRbacQuery
    {
        bool IsUserInRole(IPrincipal user, string role);
        bool IsUserAbleTo(IPrincipal user, string action);
    }
}