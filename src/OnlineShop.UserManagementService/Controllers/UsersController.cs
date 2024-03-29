﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Constants;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.UserManagementService.Requests;

namespace OnlineShop.UserManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost(RepoActions.Add)] //+
        public async Task<IdentityResult> Add(CreateUserRequest request)
        {
            var result = await _userManager.CreateAsync(request.User, request.Password);
            return result;
        }

        [HttpPost(RepoActions.Update)] //+
        public async Task<IdentityResult> Update(ApplicationUser user)
        {
            var userToBeUpdated = await _userManager.FindByNameAsync(user.UserName);
            if (userToBeUpdated == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user.UserName} was not found." });

            userToBeUpdated.DefaultAddress = user.DefaultAddress;
            userToBeUpdated.DeliveryAddress = user.DeliveryAddress;
            userToBeUpdated.FirstName = user.FirstName;
            userToBeUpdated.LastName = user.LastName;
            userToBeUpdated.PhoneNumber = user.PhoneNumber;
            userToBeUpdated.Email = user.Email;

            var result = await _userManager.UpdateAsync(userToBeUpdated);
            return result;
        }

        [HttpPost(RepoActions.Remove)] //+
        public async Task<IdentityResult> Remove(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        [HttpGet] //+
        public async Task<ApplicationUser?> Get(string userName)
        {
            var result = await _userManager.Users
                .Include(u => u.DefaultAddress)
                .Include(u => u.DeliveryAddress)
                .FirstOrDefaultAsync(u => u.UserName == userName); //improve in the future

            return result;
        }

        [HttpGet(RepoActions.GetAll)] //+
        public IEnumerable<ApplicationUser> Get()
        {
            var result = _userManager.Users
                .Include(u => u.DefaultAddress)
                .Include(u => u.DeliveryAddress)
                .AsEnumerable();
            return result;
        }

        [HttpPost(UsersControllerRoutes.ChangePassword)] //+
        public async Task<IdentityResult> ChangePassword(UserPasswordChangeRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user?.UserName} was not found." });

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result;
        }

        [HttpPost(UsersControllerRoutes.AddToRole)]
        public async Task<IdentityResult> AddToRole(AddRemoveRoleRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user?.UserName} was not found." });

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            return result;
        }

        [HttpPost(UsersControllerRoutes.AddToRoles)]
        public async Task<IdentityResult> AddToRoles(AddRemoveRolesRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user?.UserName} was not found." });

            var result = await _userManager.AddToRolesAsync(user, request.RoleNames);
            return result;
        }

        [HttpPost(UsersControllerRoutes.RemoveFromRole)]
        public async Task<IdentityResult> RemoveFromRole(AddRemoveRoleRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user?.UserName} was not found." });

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            return result;
        }

        [HttpPost(UsersControllerRoutes.RemoveFromRoles)]
        public async Task<IdentityResult> RemoveFromRoles(AddRemoveRolesRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"User {user?.UserName} was not found." });

            var result = await _userManager.RemoveFromRolesAsync(user, request.RoleNames);
            return result;
        }
    }
}
