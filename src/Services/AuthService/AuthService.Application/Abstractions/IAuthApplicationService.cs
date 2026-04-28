using AuthService.Application.Auth;
using SharedKernel;
using System.Threading.Tasks;
using System.Threading;

namespace AuthService.Application.Abstractions;

public interface IAuthApplicationService
{
    Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);

    Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken);
}