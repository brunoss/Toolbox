using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Toolbox.Rbac
{
    public class SimpleRbacSession : IRbacSession
    {
        private readonly IDictionary<string, Predicate<IPrincipal>> _userRoles;
        private readonly ICollection<Role> _rolePermissions;
        private SimpleRbacQuery _query;
        public IRbacQuery Query
        {
            get
            {
                if (_query != null)
                {
                    return _query;
                }
                return _query = new SimpleRbacQuery(this);
            }
        }
        public SimpleRbacSession()
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
    }
}