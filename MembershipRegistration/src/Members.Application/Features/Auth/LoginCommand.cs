using Members.Application.Common.Messaging;

namespace Members.Application.Features.Auth;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
