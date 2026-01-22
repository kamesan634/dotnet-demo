using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 使用者服務實作
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Store)
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<ApplicationUser>> SearchAsync(string? searchText, int? storeId, bool? isActive)
    {
        var query = _context.Users
            .Include(u => u.Store)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(u =>
                u.UserName!.Contains(searchText) ||
                u.DisplayName.Contains(searchText) ||
                (u.Email != null && u.Email.Contains(searchText)));
        }

        if (storeId.HasValue)
        {
            query = query.Where(u => u.StoreId == storeId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        return await query.OrderBy(u => u.UserName).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Store)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> CreateAsync(ApplicationUser user, string password, IEnumerable<string> roles)
    {
        user.CreatedAt = DateTime.UtcNow;
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (roles.Any())
        {
            var roleResult = await _userManager.AddToRolesAsync(user, roles);
            if (!roleResult.Succeeded)
            {
                return (false, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        _logger.LogInformation("使用者 {UserName} 已建立", user.UserName);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> UpdateAsync(ApplicationUser user, IEnumerable<string>? roles = null)
    {
        var existing = await _userManager.FindByIdAsync(user.Id.ToString());
        if (existing == null) return (false, "使用者不存在");

        existing.DisplayName = user.DisplayName;
        existing.Email = user.Email;
        existing.PhoneNumber = user.PhoneNumber;
        existing.StoreId = user.StoreId;
        existing.IsActive = user.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(existing);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (roles != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(existing);
            await _userManager.RemoveFromRolesAsync(existing, currentRoles);
            if (roles.Any())
            {
                await _userManager.AddToRolesAsync(existing, roles);
            }
        }

        _logger.LogInformation("使用者 {UserName} 已更新", user.UserName);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation("使用者 {UserName} 已刪除", user.UserName);
        }
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> ChangePasswordAsync(int id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return (false, "使用者不存在");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("使用者 {UserName} 密碼已變更", user.UserName);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<IList<string>> GetRolesAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }

    /// <inheritdoc />
    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
