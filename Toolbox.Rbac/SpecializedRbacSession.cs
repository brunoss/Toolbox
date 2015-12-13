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
                get { return (SpecializedRbacSession)_session; }
            }

            public override bool IsUserInRole<T>(IPrincipal user, string role, T resource)
            {
                var rolesForType = Session._roleAssignment.TryGetOrEmpty(typeof(T));
                var assignment = rolesForType?.TryGetOrEmpty(role);
                if (assignment == null)
                {
                    return base.IsUserInRole(user, role, resource);
                }
                return assignment(user, resource) || base.IsUserInRole(user, role, resource);
            }
        }
        private readonly IDictionary<Type, IDictionary<string, IsUserInRole>> _roleAssignment =
            new Dictionary<Type, IDictionary<string, IsUserInRole>>();
        
        IDictionary<Type, GetUserRoles> ISpecializedRbacSession.UserRolesForType
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