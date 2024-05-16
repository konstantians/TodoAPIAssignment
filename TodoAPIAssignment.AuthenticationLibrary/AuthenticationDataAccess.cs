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

    public async Task<ErrorCode> LogOutAsync(string accessToken)
    {
        try
        {
            AppUser? appUser = await CheckAndDecodeAccessTokenAsync(accessToken);
            if (appUser is null)
                return ErrorCode.InvalidAccessToken;

            //black list all the other tokens
            appUser!.AccessTokenCountId++;
            await _authDbContext.SaveChangesAsync();

            return ErrorCode.None;
        }
        catch (Exception)
        {
            return ErrorCode.DatabaseError;
        }
    }

    //TODO maybe make it not return a tupple, but something more structured.
    public async Task<AppUser?> CheckAndDecodeAccessTokenAsync(string accessToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            //After this point just check the information. The relevant information is the userId, the username and the accessTokenCount
            //Retrieve them and check that all are correct
            var jwtToken = (JwtSecurityToken)validatedToken;
            string userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            string username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            int accessTokenCount = Convert.ToInt32(jwtToken.Claims.First(x => x.Type == "AccessTokenCount").Value);

            AppUser? user = await _authDbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
            if (user is null || user.Id != userId || user.AccessTokenCountId != accessTokenCount)
                return null;

            return user;
        }
        catch (SecurityTokenExpiredException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private string GenerateToken(AppUser user)
    {
        // Create claims for the appUser
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
