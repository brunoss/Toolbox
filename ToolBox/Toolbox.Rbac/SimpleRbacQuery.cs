using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ToolBox;

namespace Toolbox.Rbac
{
    public class SimpleRbacQuery : IRbacQuery
    {
        protected readonly IRbacSession session;

        public SimpleRbacQuery(IRbacSession session)
        {
            this.session = session;
        }

        public virtual IEnumerable<string> GetUserRoles(IPrincipal user)
        {
            return session.UserRoles
                .Where(role => role.Value(user))
                .Select(role => role.Key);
        }

        public virtual IEnumerable<string> GetUserPermissions(IPrincipal user)
        {
            return GetUserRoles(user).SelectMany(GetRolePermissions);
        }

        public virtual bool IsUserInRole(IPrincipal user, string role)
        {
            if (user.IsInRole(role))
            {
                return true;
            }
            var userRole = session.UserRoles.TryGetOrEmpty(role);
            if (userRole == null)
            {
                throw new KeyNotFoundException($"The roleName {role} is not defined");
            }
            return userRole(user);
        }

        public bool IsUserAbleTo(IPrincipal principal, string action)
        {
            return GetUserPermissions(principal).Contains(action);
        }

        public virtual IEnumerable<string> GetRolePermissions(string roleName)
        {
            var role = session.RolePermissions.SingleOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                return role.Actions;
            }
            return new string[0];
        }

        public virtual bool IsRoleAbleTo(string roleName, string action)
        {
            return GetRolePermissions(roleName).Contains(action);
        }
    }
}
