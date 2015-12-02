using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ToolBox;

namespace Toolbox.Rbac
{
    public class SpecializedRbacQuery : SimpleRbacQuery, ISpecializedRbacQuery
    {
        public SpecializedRbacQuery(ISpecializedRbacSession session) : base(session)
        {

        }

        private ISpecializedRbacSession Session
        {
            get { return (ISpecializedRbacSession)session; }
        }

        public virtual IEnumerable<string> GetUserRoles<T>(IPrincipal user, T resource)
        {
            var userRoles = Session.UserRolesForType.TryGetOrEmpty(typeof(T));
            if (userRoles == null)
            {
                return Enumerable.Empty<string>();
            }
            return userRoles(user, resource);
        }

        public virtual bool IsUserInRole<T>(IPrincipal user, string role, T resource)
        {
            return GetUserRoles(user, resource).Contains(role);
        }

        public virtual IEnumerable<string> GetUserPermissions<T>(IPrincipal user, T resource)
        {
            if (!Session.UserPermissions.ContainsKey(typeof(T)))
            {
                return Enumerable.Empty<string>();
            }
            return Session.UserPermissions[typeof(T)](user, resource);
        }

        public virtual bool IsUserAbleTo<T>(IPrincipal user, string action, T resource)
        {
            return GetUserPermissions(user, resource).Contains(action);
        }
    }
}