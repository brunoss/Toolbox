using System.ComponentModel;
using System.Linq;
using System.Security.Principal;

namespace Toolbox.Rbac
{
    public class RbacPrincipal : IPrincipal, IIdentity
    {
        [Browsable(false)]
        public class CanDoAction
        {
            private readonly RbacPrincipal _principal;
            public CanDoAction(RbacPrincipal principal)
            {
                _principal = principal;
            }

            public bool this[string action]
            {
                get
                {
                    return _principal._session.Query.IsUserAbleTo(_principal, action);
                }
            }

            public bool this[string action, object resource]
            {
                get
                {
                    return _principal._session.Query.IsUserAbleTo(_principal, action, resource);
                }
            }
        }

        [Browsable(false)]
        public class IsUserInRole
        {
            private readonly RbacPrincipal _principal;
            
            public IsUserInRole(RbacPrincipal principal)
            {
                _principal = principal;
            }

            public bool this[string role]
            {
                get
                {
                    return _principal._session.Query.IsUserInRole(_principal, role);
                }
            }

            public bool this[string role, object resource]
            {
                get
                {
                    return _principal._session.Query.IsUserInRole(_principal, role, resource);
                }
            }
        }

        private readonly IPrincipal _principal;
        private readonly IRbacSession _session;
        public RbacPrincipal(IPrincipal principal, IRbacSession session)
        {
            _principal = principal;
            _session = session;
            CanDo = new CanDoAction(this);
            Is = new IsUserInRole(this);
        }

        public bool IsInRole(string role)
        {
            //check role on the decorated principal se we avoid stackoverflow
            return _session.Query.IsUserInRole(_principal, role);
        }

        public CanDoAction CanDo { get; private set; }
        public IsUserInRole Is { get; private set; }

        IIdentity IPrincipal.Identity { get { return _principal.Identity; } }
        public string Name { get { return _principal.Identity.Name; } }
        public string AuthenticationType { get { return _principal.Identity.AuthenticationType; } }
        public bool IsAuthenticated { get { return _principal.Identity.IsAuthenticated; } }
    }
}
