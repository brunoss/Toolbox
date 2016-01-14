using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ToolBox;

namespace Toolbox.Rbac
{
    public class RbacSession : IRbacSession
    {
        protected readonly IDictionary<string, Predicate<IPrincipal>> _userRoles;
        protected readonly ICollection<Role> _rolePermissions;
        private RbacQuery _query;
        private readonly IDictionary<Type, IDictionary<string, IsUserInRole>> _roleAssignment =
            new Dictionary<Type, IDictionary<string, IsUserInRole>>();
        public IRbacQuery Query
        {
            get
            {
                if (_query != null)
                {
                    return _query;
                }
                return _query = new RbacQuery(this);
            }
        }
        public RbacSession()
        {
            _userRoles = new Dictionary<string, Predicate<IPrincipal>>();
            _rolePermissions = new HashSet<Role>();
        }

        IDictionary<string, Predicate<IPrincipal>> IRbacSession.UserRoles
        {
            get
            {
                return _userRoles;
            }
        }

        IEnumerable<Role> IRbacSession.RolePermissions
        {
            get
            {
                return _rolePermissions;
            }
        }

        public void AddPermission(string roleName, string action)
        {
            var role = _rolePermissions.SingleOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                role = new Role(roleName);
                _rolePermissions.Add(role);
            }
            role.Actions.Add(action);
        }

        public void UserIsInRoleIf(string role, Predicate<IPrincipal> predicate)
        {
            _userRoles.Add(role, predicate);
        }

        IDictionary<Type, GetUserRoles> IRbacSession.UserRolesForType
        {
            get
            {
                var userRoles = new Dictionary<Type, GetUserRoles>();
                foreach (var assignment in _roleAssignment)
                {
                    var assignmentContext = assignment;
                    GetUserRoles getUserRoles = (user, resource) =>
                    {
                        return assignmentContext.Value
                            .Where(e => e.Value(user, resource))
                            .Select(e => e.Key);
                    };
                    userRoles.Add(assignment.Key, getUserRoles);
                }
                return userRoles;
            }
        }

        public void AddUserRoleForTypeIf<T>(string role, IsUserInRole predicate)
        {
            var roleAssign = _roleAssignment.TryGetOrAdd(typeof(T), new Dictionary<string, IsUserInRole>(StringComparer.OrdinalIgnoreCase));
            roleAssign.Add(role, predicate);
        }
    }
}