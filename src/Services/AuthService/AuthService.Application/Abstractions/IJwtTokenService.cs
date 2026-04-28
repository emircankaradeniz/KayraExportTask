using AuthService.Domain;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace AuthService.Application.Abstractions;

public interface IJwtTokenService
{
    Task<string> CreateAccessTokenAsync(
        ApplicationUser user,
        IReadOnlyList<string> roles,
        CancellationToken cancellationToken);

    string CreateRefreshToken();

    DateTime GetAccessTokenExpirationUtc();

    DateTime GetRefreshTokenExpirationUtc();
}