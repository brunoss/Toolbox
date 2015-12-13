using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ToolBox;

namespace Toolbox.Rbac
{
    public class SimpleRbacQuery : IRbacQuery
    {
        protected readonly IRbacSession _session;

        public SimpleRbacQuery(IRbacSession session)
        {
            _session = session;
        }

        public virtual IEnumerable<string> GetUserRoles(IPrincipal user)
        {
            var userRoles = _session.RolePermissions
                .Where(r => user.IsInRole(r.Name))
                .Select(r => r.Name);
            return _session.UserRoles
                .Where(role => role.Value(user))
                .Select(role => role.Key)
                .Union(userRoles);
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
            var userRole = _session.UserRoles.TryGetOrEmpty(role);
            bool roleExists = userRole != null ||
                _session.RolePermissions
                .Any(r => r.Name.Equals(role, StringComparison.OrdinalIgnoreCase));
            if (!roleExists)
            {
                throw new KeyNotFoundException($"The roleName {role} is not defined");
            }
            if (userRole == null)
            {
                return false;
            }
            return userRole(user);
        }

        public bool IsUserAbleTo(IPrincipal principal, string action)
        {
            return GetUserPermissions(principal).Contains(action, StringComparer.OrdinalIgnoreCase);
        }

        public virtual IEnumerable<string> GetRolePermissions(string roleName)
        {
            var role = _session.RolePermissions.SingleOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                return role.Actions;
            }
            return Enumerable.Empty<string>();
        }
    }
}
