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
                var rolesForType = Session._roleAssignment.TryGetOrEmpty(typeof(T));
                var assignment = rolesForType?.TryGetOrEmpty(role);
                if (assignment == null)
                {
                    return false;
                }
                return assignment(user, resource);
            }
        }
        private readonly IDictionary<Type, IDictionary<string, IsUserInRole>> _roleAssignment =
            new DoubleKeyDictionary<Type, string, IsUserInRole>(null, StringComparer.InvariantCultureIgnoreCase);
        
        IDictionary<Type, GetUserRoles> ISpecializedRbacSession.UserRolesForType
        {
            get
            {
                IDictionary<Type, GetUserRoles> userRoles = new Dictionary<Type, GetUserRoles>();
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

        public IDictionary<Type, IDictionary<string, IsUserInRole>> RoleAssignment
        {
            get { return _roleAssignment; }
        }

        public void AddUserRoleForTypeIf<T>(string role, IsUserInRole predicate)
        {
            IDictionary<string, IsUserInRole> roleAssign;
            if (!_roleAssignment.ContainsKey(typeof(T)))
            {
                roleAssign = new Dictionary<string, IsUserInRole>();
                _roleAssignment.Add(typeof(T), roleAssign);
            }
            else
            {
                roleAssign = _roleAssignment[typeof(T)];
            }
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