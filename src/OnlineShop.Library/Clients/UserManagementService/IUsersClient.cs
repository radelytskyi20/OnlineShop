﻿using Microsoft.AspNetCore.Identity;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.UserManagementService.Requests;
using OnlineShop.Library.UserManagementService.Responses;

namespace OnlineShop.Library.Clients.UserManagementService
{
    public interface IUsersClient : IHttpClientContainer
    {
        Task<IdentityResult> Add(CreateUserRequest request);

        Task<IdentityResult> Update(ApplicationUser user);

        Task<IdentityResult> Remove(ApplicationUser user);

        Task<UserManagementServiceResponse<ApplicationUser>> Get(string name);

        Task<UserManagementServiceResponse<IEnumerable<ApplicationUser>>> GetAll();

        Task<IdentityResult> ChangePassword(UserPasswordChangeRequest request);

        Task<IdentityResult> AddToRole(AddRemoveRoleRequest request);

        Task<IdentityResult> AddToRoles(AddRemoveRolesRequest request);

        Task<IdentityResult> RemoveFromRole(AddRemoveRoleRequest request);

        Task<IdentityResult> RemoveFromRoles(AddRemoveRolesRequest request);
    }
}
