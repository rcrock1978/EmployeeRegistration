namespace Members.Application.Features.Auth;

public sealed record LoginResponse(string Token, string Email, string Role);
