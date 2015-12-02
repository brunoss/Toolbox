using System;
using System.Collections.Generic;

namespace Toolbox.Rbac
{
    public interface ISpecializedRbacSession : IRbacSession
    {
        IDictionary<Type, GetUserRoles> UserRolesForType { get; }
        IDictionary<Type, GetUserPermissions> UserPermissions { get; }
        new ISpecializedRbacQuery Query { get; }
        void AddUserRoleForTypeIf<T>(string role, IsUserInRole predicate);
        void AddPermission<T>(string roleName, string action);
    }
}