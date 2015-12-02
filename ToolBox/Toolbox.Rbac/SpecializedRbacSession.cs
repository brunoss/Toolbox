using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ToolBox;

namespace Toolbox.Rbac
{
    public class SpecializedRbacSession : SimpleRbacSession, ISpecializedRbacSession
    {
        private class MyQuery : SpecializedRbacQuery
        {
            public MyQuery(SpecializedRbacSession session)
                : base(session)
            {
            }

            private SpecializedRbacSession Session
            {
                get { return (SpecializedRbacSession)session; }
            }

            public override bool IsUserInRole<T>(IPrincipal user, string role, T resource)
            {
                var rolesForType = Session.roleAssignment.TryGetOrEmpty(typeof(T));
                var assignment = rolesForType?.TryGetOrEmpty(role);
                if (assignment == null)
                {
                    return false;
                }
                return assignment(user, resource);
            }
        }
        private readonly IDictionary<Type, IDictionary<string, IsUserInRole>> roleAssignment =
            new DoubleKeyDictionary<Type, string, IsUserInRole>(null, StringComparer.InvariantCultureIgnoreCase);

        private readonly IDictionary<Type, ICollection<Role>> permissionAssignment = new Dictionary<Type, ICollection<Role>>();
        IDictionary<Type, GetUserRoles> ISpecializedRbacSession.UserRolesForType
        {
            get { return GetUserRolesFromAssignment(); }
        }

        IDictionary<Type, GetUserPermissions> ISpecializedRbacSession.UserPermissions
        {
            get
            {
                return GetUserPermissionsFromAssignment();
            }
        }

        public IDictionary<Type, IDictionary<string, IsUserInRole>> RoleAssignment
        {
            get { return roleAssignment; }
        }

        public void AddUserRoleForTypeIf<T>(string role, IsUserInRole predicate)
        {
            IDictionary<string, IsUserInRole> roleAssign;
            if (!roleAssignment.ContainsKey(typeof(T)))
            {
                roleAssign = new Dictionary<string, IsUserInRole>();
                roleAssignment.Add(typeof(T), roleAssign);
            }
            else
            {
                roleAssign = roleAssignment[typeof(T)];
            }
            roleAssign.Add(role, predicate);
        }

        public void AddPermission<T>(string roleName, string action)
        {
            ICollection<Role> roles = permissionAssignment.TryGetOrAdd(typeof(T), new HashSet<Role>());
            Role role = roles.SingleOrDefault(r => r.Name == action);
            if (role != null)
            {
                role.Actions.Add(action);
            }
            else
            {
                role = new Role(roleName);
                role.Actions.Add(action);
                roles.Add(role);
            }
        }

        private IDictionary<Type, GetUserPermissions> GetUserPermissionsFromAssignment()
        {
            IDictionary<Type, GetUserPermissions> userPermissions = new Dictionary<Type, GetUserPermissions>();
            IDictionary<Type, GetUserRoles> userRole = GetUserRolesFromAssignment();
            foreach (var permission in permissionAssignment)
            {
                var permissionContext = permission;
                GetUserPermissions getUserPermissions = (user, resource) =>
                {
                    //for every roleName that user have check their permissions
                    return userRole[permissionContext.Key](user, resource)
                        .SelectMany(role => permissionContext.Value.SelectMany(r => r.Actions));
                };
                userPermissions.Add(permission.Key, getUserPermissions);
            }
            return userPermissions;
        }

        private IDictionary<Type, GetUserRoles> GetUserRolesFromAssignment()
        {
            IDictionary<Type, GetUserRoles> userRoles = new Dictionary<Type, GetUserRoles>();
            foreach (var assignment in roleAssignment)
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

        private ISpecializedRbacQuery _query;
        public new ISpecializedRbacQuery Query
        {
            get
            {
                if (_query != null)
                {
                    return _query;
                }
                return _query = new MyQuery(this);
            }
        }
    }
}