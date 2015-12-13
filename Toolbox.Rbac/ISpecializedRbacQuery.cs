using System.Security.Principal;

namespace Toolbox.Rbac
{
    public interface ISpecializedRbacQuery : IRbacQuery
    {
        bool IsUserInRole<T>(IPrincipal user, string role, T resource);
        bool IsUserAbleTo<T>(IPrincipal user, string action, T resource);
    }
}