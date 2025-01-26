using Microsoft.AspNetCore.Identity;

namespace Bookify.Application.Services.Users
{
    internal class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> GetUsersByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IEnumerable<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
        public async Task<IList<string>> GetUsersRolesAsync(ApplicationUser user)
        {

            return await _userManager.GetRolesAsync(user);
        }
        public async Task<ManageUserResponseDto> CreateUserAsync(CreateUserDto dto, string createdById)
        {


            ApplicationUser user = new()
            {

                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                CreatedById = createdById
                // Here : Get User's Id (Id =NameIdentifier )
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, dto.SelectedRoles);


                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                return new ManageUserResponseDto(IsSucceeded: true, User: user, VerificationCode: code, Errors: null);
            }

            return new ManageUserResponseDto(IsSucceeded: false, User: null, VerificationCode: null, Errors: result.Errors.Select(e => e.Description));
        }

        public async Task<ManageUserResponseDto> UpdateUserAsync(ApplicationUser user, IList<string> selectedRoles, string updatedById)
        {
            user.LastUpdatedById = updatedById;

            user.LastUpdatedOn = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {

                var currentRoles = await _userManager.GetRolesAsync(user);
                var updatedRoles = !currentRoles.SequenceEqual(selectedRoles);
                if (updatedRoles)

                {

                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, selectedRoles);
                }

                await _userManager.UpdateSecurityStampAsync(user);

                return new ManageUserResponseDto(IsSucceeded: true, User: user, VerificationCode: null, Errors: null);
            }

            return new ManageUserResponseDto(IsSucceeded: false, User: null, VerificationCode: null, Errors: result.Errors.Select(e => e.Description));
        }

        public async Task<ManageUserResponseDto> ResetPasswordAsync(ApplicationUser user, string password, string updatedById)
        {

            var currentPasswordHash = user.PasswordHash;
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, password);


            if (result.Succeeded)
            {
                user.LastUpdatedById = updatedById;
                user.LastUpdatedOn = DateTime.Now;
                await _userManager.UpdateAsync(user);


                return new ManageUserResponseDto(IsSucceeded: true, User: user, VerificationCode: null, Errors: null);


            }


            user.PasswordHash = currentPasswordHash;
            await _userManager.UpdateAsync(user);
            return new ManageUserResponseDto(IsSucceeded: false, User: null, VerificationCode: null, Errors: result.Errors.Select(e => e.Description));


        }

        public async Task<ApplicationUser?> ToggleStatus(string id, string updatedById)
        {

            var user = await GetUsersByIdAsync(id);

            if (user is null)
                return null;

            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedById = updatedById;

            user.LastUpdatedOn = DateTime.Now;
            await _userManager.UpdateAsync(user); //same  _context.SaveChanges();

            if (user.IsDeleted)
                await _userManager.UpdateSecurityStampAsync(user);

            return user;


        }

        public async Task<ApplicationUser?> UnlockUserAsync(string id)
        {
            var user = await GetUsersByIdAsync(id);

            if (user is null)
                return null;

            var isLockedUser = await _userManager.IsLockedOutAsync(user);
            if (isLockedUser)
                await _userManager.SetLockoutEndDateAsync(user, null);

            return user;
        }
        public async Task<bool> AllowUserNameAsync(string? id, string username)
        {

            var user = await _userManager.FindByNameAsync(username);


            var isValid = user is null || user.Id.Equals(id);

            return isValid;
        }
        public async Task<bool> AllowEmailAsync(string? id, string email)
        {

            var user = await _userManager.FindByEmailAsync(email);

            var isValid = user is null || user.Id.Equals(id);

            return isValid;
        }

    }

}