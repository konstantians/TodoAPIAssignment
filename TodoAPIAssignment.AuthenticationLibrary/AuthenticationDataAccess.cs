using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAPIAssignment.AuthenticationLibrary.Enums;
using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.AuthenticationLibrary;

public class AuthenticationDataAccess : IAuthenticationDataAccess
{
    private readonly AuthDbContext _authDbContext;
    private readonly IConfiguration _config;

    public AuthenticationDataAccess(AuthDbContext authDbContext, IConfiguration config)
    {
        _authDbContext = authDbContext;
        _config = config;
    }

    public async Task<AuthenticationResult>? SignUpAsync(string username, string password, string email)
    {
        try
        {
            AppUser? user = await _authDbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
            if (user is not null)
                return new AuthenticationResult() { ErrorCode = ErrorCode.DuplicateUsername };

            user = await _authDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user is not null)
                return new AuthenticationResult() { ErrorCode = ErrorCode.DuplicateEmail };

            AppUser newUser = new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Password = password,
                Email = email,
                AccessTokenCountId = 1
            };

            await _authDbContext.Users.AddAsync(newUser);
            await _authDbContext.SaveChangesAsync();

            string token = GenerateToken(newUser);
            return new AuthenticationResult() { ErrorCode = ErrorCode.None, Token = token};
        }
        catch (Exception)
        {
            return new AuthenticationResult() { ErrorCode = ErrorCode.DatabaseError };
        }
    }

    public async Task<AuthenticationResult>? LogInAsync(string username, string password)
    {
        try
        {
            AppUser? user = await _authDbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
            if (user is null || user!.Password != password)
                return new AuthenticationResult() { ErrorCode = ErrorCode.InvalidCredentials };

            string token = GenerateToken(user);
            return new AuthenticationResult() { ErrorCode = ErrorCode.None, Token = token };
        }
        catch (Exception)
        {
            return new AuthenticationResult() { ErrorCode = ErrorCode.DatabaseError };
        }
    }

    private string GenerateToken(AppUser user)
    {
        // Create claims for the user
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim("AccessTokenCount", user.AccessTokenCountId!.ToString())
        };

        // Generate JWT token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
