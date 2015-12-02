using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Rbac
{
    public class Rbac
    {
        public class _User
        {
            private readonly IRbacSession session;
            internal _User(IRbacSession session)
            {
                this.session = session;
            }
            public UserRole Is(string role)
            {
                return new UserRole(session, role);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class UserRole
            {
                internal string Role { get; private set; }
                internal IRbacSession Session { get; private set; }
                internal UserRole(IRbacSession session, string role)
                {
                    this.Session = session;
                    this.Role = role;
                }

                public void If(Predicate<IPrincipal> predicate)
                {
                    Session.UserIsInRoleIf(Role, predicate);
                }

                public UserRoleResource<T> Of<T>()
                {
                    ISpecializedRbacSession session = Session as ISpecializedRbacSession;
                    if (session == null)
                    {
                        throw new NotSupportedException();
                    }
                    return new UserRoleResource<T>(this);
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class UserRoleResource<T>
            {
                private readonly UserRole userRole;

                internal UserRoleResource(UserRole role)
                {
                    this.userRole = role;
                }
                public void If(IsUserInRole predicate)
                {
                    ISpecializedRbacSession session = userRole.Session as ISpecializedRbacSession;
                    session.AddUserRoleForTypeIf<T>(userRole.Role, predicate);
                }
            }

        }
        
        public class _What
        {
            internal _What(IRbacSession session)
            {
                Can = new _Can(session);
            }

            public _Can Can { get; private set; }

            public class _Can
            {
                internal _Can(IRbacSession session)
                {
                    Role = new _Role(session);

                }
                public _Role Role { get; private set; }
                public class _Role
                {
                    private readonly IRbacSession session;
                    internal _Role(IRbacSession session)
                    {
                        this.session = session;
                    }
                    public IEnumerable<string> Do(string role)
                    {
                        return session.Query.GetRolePermissions(role);
                    }
                }
                public _User User { get; private set; }
                public class _User
                {
                    private readonly IRbacSession session;
                    internal _User(IRbacSession session)
                    {
                        this.session = session;
                    }
                    public IEnumerable<string> Do(IPrincipal principal)
                    {
                        return session.Query.GetUserPermissions(principal);
                    }
                }
            }

        }
        
        public class _Is
        {
            private readonly IRbacSession session;
            internal _Is(IRbacSession session)
            {
                this.session = session;
            }
            public Principal User(IPrincipal principal)
            {
                return new Principal(session, principal);
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class Principal
            {
                internal IRbacSession Session { get; private set; }
                internal IPrincipal User { get; private set; }
                internal Principal(IRbacSession session, IPrincipal user)
                {
                    this.Session = session;
                    this.User = user;
                }
                public UserRole A(string role)
                {
                    return new UserRole(this, role) { Result = Session.Query.IsUserInRole(User, role) };
                }

                [Browsable(false)]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public class UserRole
                {
                    private readonly Principal principal;
                    private readonly string role;
                    internal UserRole(Principal principal, string role)
                    {
                        this.principal = principal;
                        this.role = role;
                    }
                    public bool Result { get; set; }

                    public bool Of<T>(T resource)
                    {
                        ISpecializedRbacSession session = principal.Session as ISpecializedRbacSession;
                        if (session == null)
                        {
                            throw new NotSupportedException();
                        }
                        Result = session.Query.IsUserInRole(principal.User, role, resource);
                        return Result;
                    }
                }
            }
        }
        
        public class _Do
        {
            private readonly IRbacSession session;
            internal _Do(IRbacSession session)
            {
                this.session = session;
            }
            public _Action A(string name)
            {
                return new _Action(session, name);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class _Action
            {
                internal string Action { get; private set; }
                internal IRbacSession Session { get; private set; }

                internal _Action(IRbacSession session, string action)
                {
                    this.Session = session;
                    this.Action = action;
                }

                public void Requires(string role)
                {
                    Session.AddPermission(role, Action);
                }

                public ActionRequirements<T> Of<T>()
                {
                    return new ActionRequirements<T>(this);
                }

                [Browsable(false)]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public class ActionRequirements<T>
                {
                    private readonly _Action action;

                    internal ActionRequirements(_Action action)
                    {
                        this.action = action;
                    }

                    public void Requires(string role)
                    {
                        ISpecializedRbacSession session = action.Session as ISpecializedRbacSession;
                        if (session == null)
                        {
                            throw new NotSupportedException();
                        }
                        session.AddPermission<T>(role, action.Action);
                    }
                }
            }
        }

        public class _Can
        {
            private readonly IRbacSession session;
            internal _Can(IRbacSession session)
            {
                this.session = session;
            }
            public UserAction User(IPrincipal principal = null)
            {
                return new UserAction(session, principal ?? Thread.CurrentPrincipal);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class UserAction
            {
                internal IRbacSession Session { get; private set; }
                internal IPrincipal Principal { get; private set; }
                internal UserAction(IRbacSession session, IPrincipal principal)
                {
                    this.Session = session;
                    this.Principal = principal;
                }

                public ActionResource Do(string action)
                {
                    return new ActionResource(this, action) { Result = Session.Query.IsUserAbleTo(Principal, action) };
                }
                [Browsable(false)]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public class ActionResource
                {
                    private readonly UserAction userAction;
                    private readonly string action;
                    internal ActionResource(UserAction userAction, string action)
                    {
                        this.userAction = userAction;
                        this.action = action;
                    }

                    public bool Result { get; set; }

                    public bool The<T>(T resource)
                    {
                        ISpecializedRbacSession session = userAction.Session as ISpecializedRbacSession;
                        if (session == null)
                        {
                            throw new NotSupportedException();
                        }
                        Result = session.Query.IsUserAbleTo(userAction.Principal, action, resource);
                        return Result;
                    }
                }
            }
        }

        public Rbac(IRbacSession session)
        {
            User = new _User(session);
            What = new _What(session);
            Is = new _Is(session);
            Do = new _Do(session);
            Can = new _Can(session);
        }
        public _User User { get; private set; }
        public _What What { get; private set; }
        public _Is Is { get; private set; }
        public _Do Do { get; private set; }
        public _Can Can { get; private set; }
    }

}
