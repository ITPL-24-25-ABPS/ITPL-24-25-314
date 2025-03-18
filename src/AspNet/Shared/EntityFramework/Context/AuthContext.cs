using System.Data.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shared.EntityFramework.Entities;

namespace Shared.EntityFramework.Context;

public class AuthContext : IdentityDbContext<SystemUser>
{
    public DbSet<SystemUser> SystemUsers { get; set; }

    public AuthContext(DbContextOptions options) : base(options)
    {

    }
}