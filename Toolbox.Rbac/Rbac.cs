﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Threading;

namespace Toolbox.Rbac
{
    public class Rbac
    {
        public class _User
        {
            private readonly IRbacSession _session;
            internal _User(IRbacSession session)
            {
                _session = session;
            }
            public UserRole Is(string role)
            {
                return new UserRole(_session, role);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class UserRole
            {
                internal string Role { get; private set; }
                internal IRbacSession Session { get; private set; }
                internal UserRole(IRbacSession session, string role)
                {
                    Session = session;
                    Role = role;
                }

                public void If(Predicate<IPrincipal> predicate)
                {
                    Session.UserIsInRoleIf(Role, predicate);
                }

                public UserRoleResource<T> Of<T>()
                {
                    var session = Session as ISpecializedRbacSession;
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
                private readonly UserRole _userRole;

                internal UserRoleResource(UserRole role)
                {
                    _userRole = role;
                }
                public void If(Func<IPrincipal, T, bool> pred)
                {
                    var session = _userRole.Session as ISpecializedRbacSession;
                    if (session == null)
                    {
                        throw new NotSupportedException();
                    }
                    session.AddUserRoleForTypeIf<T>(_userRole.Role, (user, resource) => pred(user, (T)resource));
                }
            }

        }

        public class _Is
        {
            private readonly IRbacSession _session;
            internal _Is(IRbacSession session)
            {
                _session = session;
            }
            public Principal User(IPrincipal principal)
            {
                return new Principal(_session, principal);
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class Principal
            {
                internal IRbacSession Session { get; private set; }
                internal IPrincipal User { get; private set; }
                internal Principal(IRbacSession session, IPrincipal user)
                {
                    Session = session;
                    User = user;
                }
                public UserRole A(string role)
                {
                    return new UserRole(this, role) { Result = Session.Query.IsUserInRole(User, role) };
                }

                [Browsable(false)]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public class UserRole
                {
                    private readonly Principal _principal;
                    private readonly string _role;
                    internal UserRole(Principal principal, string role)
                    {
                        _principal = principal;
                        _role = role;
                    }
                    public bool Result { get; set; }

                    public bool Of<T>(T resource)
                    {
                        var session = _principal.Session as ISpecializedRbacSession;
                        if (session == null)
                        {
                            throw new NotSupportedException();
                        }
                        Result = session.Query.IsUserInRole(_principal.User, _role, resource);
                        return Result;
                    }
                }
            }
        }
        
        public class _Do
        {
            private readonly IRbacSession _session;
            internal _Do(IRbacSession session)
            {
                _session = session;
            }
            public _Action A(string name)
            {
                return new _Action(_session, name);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class _Action
            {
                internal string Action { get; private set; }
                internal IRbacSession Session { get; private set; }

                internal _Action(IRbacSession session, string action)
                {
                    Session = session;
                    Action = action;
                }

                public void Requires(string role)
                {
                    Session.AddPermission(role, Action);
                }
                
            }
        }

        public class _Can
        {
            private readonly IRbacSession _session;
            internal _Can(IRbacSession session)
            {
                _session = session;
            }
            public UserAction User(IPrincipal principal = null)
            {
                return new UserAction(_session, principal ?? Thread.CurrentPrincipal);
            }
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public class UserAction
            {
                internal IRbacSession Session { get; private set; }
                internal IPrincipal Principal { get; private set; }
                internal UserAction(IRbacSession session, IPrincipal principal)
                {
                    Session = session;
                    Principal = principal;
                }

                public ActionResource Do(string action)
                {
                    return new ActionResource(this, action)
                    {
                        Result = Session.Query.IsUserAbleTo(Principal, action)
                    };
                }
                [Browsable(false)]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public class ActionResource
                {
                    private readonly UserAction _userAction;
                    private readonly string _action;
                    internal ActionResource(UserAction userAction, string action)
                    {
                        _userAction = userAction;
                        _action = action;
                    }

                    public bool Result { get; set; }

                    public bool The<T>(T resource)
                    {
                        var session = _userAction.Session as ISpecializedRbacSession;
                        if (session == null)
                        {
                            throw new NotSupportedException();
                        }
                        Result = session.Query.IsUserAbleTo(_userAction.Principal, _action, resource);
                        return Result;
                    }
                }
            }
        }

        public Rbac(IRbacSession session)
        {
            User = new _User(session);
            Is = new _Is(session);
            Do = new _Do(session);
            Can = new _Can(session);
        }
        public _User User { get; private set; }
        public _Is Is { get; private set; }
        public _Do Do { get; private set; }
        public _Can Can { get; private set; }
    }

}