using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AuthServer.Databases.DbInitializer;
using AuthServer.Endpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.EntityFramework.Context;
using Shared.EntityFramework.Entities;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Key --------------------------

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);

// -------------------- DB --------------------------


IConfiguration config = builder.Configuration.GetRequiredSection("ConnectionString");

builder.Services.AddDbContext<AuthContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlite(config["SqLite"], sqliteOptions =>
        {
            sqliteOptions.MigrationsAssembly("AuthServer.Migrations.Sqlite");
        });
    else
        options.UseMySql(config["MySql"], new MariaDbServerVersion(new Version(8, 0, 21)), mySqlOptions =>
        {
            mySqlOptions.MigrationsAssembly("AuthServer.Migrations.MySql");
        });
});

builder.Services.AddIdentity<SystemUser, IdentityRole>(identityOptions =>
{
    identityOptions.SignIn.RequireConfirmedAccount = false;
    identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
    identityOptions.SignIn.RequireConfirmedEmail = false;
    identityOptions.User.RequireUniqueEmail = false;
    identityOptions.Password.RequiredUniqueChars = 0;
    identityOptions.Lockout.AllowedForNewUsers = false;
    identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero;
    identityOptions.Lockout.MaxFailedAccessAttempts = int.MaxValue; // TODO change later
})
.AddEntityFrameworkStores<AuthContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateActor = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
});


// -------------------- Database init --------------------------

builder.Services.AddHostedService<DbInitializer>();

// -------------------- Controllers --------------------------

builder.Services.AddControllers();

var app = builder.Build();


// -------------------- CORS --------------------------


app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());


// -------------------- Identity --------------------------

app.UseAuthentication();
app.UseAuthorization();

// -------------------- Endpoints --------------------------

app.MapControllers();

app.MapGet("/", () => "Hello World!");



// app.MapGet("/test", () =>
// {
//     return Results.Ok("Test");
// });
//
// app.MapGet("/token", () =>
// {
//     var tokenHandler = new JwtSecurityTokenHandler();
//     var tokenDescription = new SecurityTokenDescriptor
//     {
//         Subject = new ClaimsIdentity(),
//         Expires = DateTime.UtcNow.AddDays(2),
//         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//     };
//     var newToken = tokenHandler.CreateToken(tokenDescription);
//     var tokenString = tokenHandler.WriteToken(newToken);
//     return Results.Ok("Token: " + tokenString);
// });
//
// app.MapPost("/login", async (UserManager<SystemUser> userManager, SignInManager<SystemUser> signInManager, [FromBody] LoginModel model) =>
// {
//     var user = await userManager.FindByEmailAsync(model.Email);
//     if (user is not null)
//         return Results.Ok("Logged in");
//
//     user = new SystemUser()
//     {
//         Email = model.Email,
//         UserName = model.Username
//     };
//
//     var identityResult = await userManager.CreateAsync(user, model.Password);
//
//     if (!identityResult.Succeeded)
//         return Results.InternalServerError(identityResult.Errors);
//
//     user = await userManager.FindByEmailAsync(model.Email);
//
//     var claims = new[]
//     {
//         new Claim(ClaimTypes.NameIdentifier, user.Id),
//         new Claim(ClaimTypes.Email, user.Email!)
//     };
//
//
//     var tokenHandler = new JwtSecurityTokenHandler();
//     var tokenDescription = new SecurityTokenDescriptor
//     {
//         Subject = new ClaimsIdentity(claims),
//         Expires = DateTime.UtcNow.AddDays(2),
//         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//     };
//     var newToken = tokenHandler.CreateToken(tokenDescription);
//     var tokenString = tokenHandler.WriteToken(newToken);
//     return Results.Ok("Token: " + tokenString);
// });
//
//     app.MapGet("/secret", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]() =>
//     {
//         return Results.Ok("Secret");
//     });

app.Run();
