using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 角色服務實作
/// </summary>
public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<ApplicationRole>> GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ApplicationRole?> GetByIdAsync(int id)
    {
        return await _roleManager.FindByIdAsync(id.ToString());
    }

    /// <inheritdoc />
    public async Task<ApplicationRole?> GetByNameAsync(string name)
    {
        return await _roleManager.FindByNameAsync(name);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> CreateAsync(ApplicationRole role)
    {
        role.CreatedAt = DateTime.UtcNow;
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("角色 {RoleName} 已建立", role.Name);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> UpdateAsync(ApplicationRole role)
    {
        var existing = await _roleManager.FindByIdAsync(role.Id.ToString());
        if (existing == null) return (false, "角色不存在");

        if (existing.IsSystemRole)
        {
            return (false, "系統角色無法修改");
        }

        existing.Name = role.Name;
        existing.Description = role.Description;

        var result = await _roleManager.UpdateAsync(existing);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("角色 {RoleName} 已更新", role.Name);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return (false, "角色不存在");

        if (role.IsSystemRole)
        {
            return (false, "系統角色無法刪除");
        }

        // 檢查是否有使用者使用此角色
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            return (false, "此角色有使用者使用中，無法刪除");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("角色 {RoleName} 已刪除", role.Name);
        return (true, null);
    }

    /// <inheritdoc />
    public async Task<int> GetUserCountAsync(int roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return 0;

        var users = await _userManager.GetUsersInRoleAsync(role.Name!);
        return users.Count;
    }
}
