namespace AuthService.Application.Auth;

public sealed record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);