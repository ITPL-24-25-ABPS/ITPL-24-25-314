using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.ApiSchema.Auth;
using Shared.EntityFramework.Entities;

namespace AuthServer.Endpoints;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<SystemUser> _userManager;
    private readonly SignInManager<SystemUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<SystemUser> userManager,
        SignInManager<SystemUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return BadRequest(new { error = "Email already in use." });

        var user = new SystemUser { UserName = dto.Username, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Registered" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
            return Unauthorized(new { error = "Invalid credentials." });

        var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!signIn.Succeeded)
            return Unauthorized(new { error = "Invalid credentials." });

        // create JWT
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);
        var tokenDuration = int.Parse(_config["Jwt:DurationDays"]!);
        user = await _userManager.FindByNameAsync(dto.Username);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user!.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(tokenDuration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var newToken = tokenHandler.CreateToken(tokenDescription);
        var tokenString = tokenHandler.WriteToken(newToken);
        return Ok(new { token = tokenString });
    }
    [Authorize]
    [HttpGet("secret")]
    public async Task<IActionResult> Secret()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized(new { error = "Unauthorized" });

        return Ok(new { message = "This is a secret message." });
    }

    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(token);
        if(decodedToken.ValidTo < DateTime.UtcNow)
            return Unauthorized(new { error = "Token expired" });
        var user = await _userManager.FindByIdAsync(decodedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        if (user is null)
            return Unauthorized(new {error = "Invalid token"});

        return Ok(new { message = "Token is valid" });
    }
}