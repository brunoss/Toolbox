﻿using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Toolbox.Rbac
{
    public delegate IEnumerable<string> GetUserPermissions(IPrincipal user, object resource);
    public delegate IEnumerable<string> GetUserRoles(IPrincipal user, object resource);
    public delegate bool IsUserInRole(IPrincipal user, object resource);

    public interface IRbacSession
    {
        IDictionary<string, Predicate<IPrincipal>> UserRoles { get; }
        IEnumerable<Role> RolePermissions { get; }
        IRbacQuery Query { get; }
        void AddPermission(string roleName, string action);
        void UserIsInRoleIf(string role, Predicate<IPrincipal> predicate);
        IDictionary<Type, GetUserRoles> UserRolesForType { get; }
        void AddUserRoleForTypeIf<T>(string role, IsUserInRole predicate);
    }
}
