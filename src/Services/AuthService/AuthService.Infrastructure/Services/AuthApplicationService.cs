using AuthService.Application.Abstractions;
using AuthService.Application.Auth;
using AuthService.Domain;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace AuthService.Infrastructure.Services;

public sealed class AuthApplicationService : IAuthApplicationService
{
    private static readonly string[] AllowedRoles = ["Admin", "Manager", "User"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthApplicationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role.Trim();

        if (!AllowedRoles.Contains(role))
        {
            return Result.Failure<AuthResponse>("Invalid role.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Result.Failure<AuthResponse>("A user with the same email already exists.");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new ApplicationRole(role));
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            UserName = request.Email.Trim(),
            EmailConfirmed = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var error = string.Join(" ", createResult.Errors.Select(identityError => identityError.Description));
            return Result.Failure<AuthResponse>(error);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            var error = string.Join(" ", roleResult.Errors.Select(identityError => identityError.Description));
            return Result.Failure<AuthResponse>(error);
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthResponse>("Invalid email or password.");
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordIsValid)
        {
            return Result.Failure<AuthResponse>("Invalid email or password.");
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var users = _userManager.Users.Where(user => user.RefreshToken == request.RefreshToken);
        var user = users.FirstOrDefault();

        if (user is null)
        {
            return Result.Failure<AuthResponse>("Invalid refresh token.");
        }

        if (user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure<AuthResponse>("Refresh token has expired.");
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    private async Task<Result<AuthResponse>> CreateAuthResponseAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _jwtTokenService.CreateAccessTokenAsync(user, roles.ToList(), cancellationToken);
        var refreshToken = _jwtTokenService.CreateRefreshToken();
        var accessTokenExpiresAtUtc = _jwtTokenService.GetAccessTokenExpirationUtc();
        var refreshTokenExpiresAtUtc = _jwtTokenService.GetRefreshTokenExpirationUtc();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            roles.ToList(),
            accessToken,
            refreshToken,
            accessTokenExpiresAtUtc,
            refreshTokenExpiresAtUtc);

        return Result.Success(response);
    }
}