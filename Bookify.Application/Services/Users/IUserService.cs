using Microsoft.AspNetCore.Identity;

namespace Bookify.Application.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<IEnumerable<IdentityRole>> GetRolesAsync();
        Task<ApplicationUser?> GetUsersByIdAsync(string userId);
        Task<IList<string>> GetUsersRolesAsync(ApplicationUser user);

        Task<ManageUserResponseDto> CreateUserAsync(CreateUserDto dto, string createdById);
        Task<ManageUserResponseDto> UpdateUserAsync(ApplicationUser user, IList<string> selectedRoles, string updatedById);
        Task<ManageUserResponseDto> ResetPasswordAsync(ApplicationUser user, string password, string updatedById);
        Task<ApplicationUser?> ToggleStatus(string id, string updatedById);
        Task<ApplicationUser?> UnlockUserAsync(string id);

        Task<bool> AllowUserNameAsync(string? id, string username);
        Task<bool> AllowEmailAsync(string? id, string email);


    }
}
