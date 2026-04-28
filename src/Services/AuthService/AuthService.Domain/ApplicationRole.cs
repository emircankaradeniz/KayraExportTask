using Microsoft.AspNetCore.Identity;
using System;

namespace AuthService.Domain;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}