using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ToolBox;

namespace Toolbox.Rbac
{
    public class RbacQuery : IRbacQuery
    {
        protected readonly IRbacSession _session;

        public RbacQuery(IRbacSession session)
        {
            _session = session;
        }

        public virtual IEnumerable<string> GetUserRoles<T>(IPrincipal user, T resource)
        {
            var userRoles = _session.UserRolesForType.TryGetOrEmpty(resource.GetType());
            if (userRoles == null)
            {
                return Enumerable.Empty<string>();
            }
            return userRoles(user, resource);
        }

        public virtual bool IsUserInRole<T>(IPrincipal user, string role, T resource)
        {
            return user.IsInRole(role) ||
                GetUserRoles(user, resource).Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public virtual IEnumerable<string> GetUserPermissions<T>(IPrincipal user, T resource)
        {
            var userRoles = _session.UserRolesForType.TryGetOrEmpty(typeof(T));
            if (userRoles == null)
            {
                return GetUserPermissions(user);
            }
            return userRoles(user, resource).Union(GetUserPermissions(user));
        }

        public virtual bool IsUserAbleTo<T>(IPrincipal user, string action, T resource)
        {
            return GetUserRoles(user, resource).Any(r => IsUserInRole(user, r, resource)) ||
            GetUserPermissions(user, resource).Contains(action, StringComparer.OrdinalIgnoreCase);
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
