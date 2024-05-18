using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoAPIAssignment.API.Models.AuthenticationControllerModels.RequestModels;
using TodoAPIAssignment.AuthenticationLibrary;
using TodoAPIAssignment.AuthenticationLibrary.Enums;
using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationDataAccess _authenticationDataAccess;

    public AuthenticationController(IAuthenticationDataAccess authenticationDataAccess)
    {
        _authenticationDataAccess = authenticationDataAccess;
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel signUpRequestModel)
    {
        try
        {
            AuthenticationResult result = await _authenticationDataAccess.SignUpAsync(signUpRequestModel.Username!, 
                signUpRequestModel.Password!, signUpRequestModel.Email!)!;
            
            if(result.ErrorCode == ErrorCode.DuplicateUsername)
                return BadRequest(new {ErrorMessage = "DuplicateUsernameError"});
            else if (result.ErrorCode == ErrorCode.DuplicateEmail)
                return BadRequest(new { ErrorMessage = "DuplicateEmailError" });

            return Ok(new { Token = result.Token });
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpPost("LogIn")]
    public async Task<IActionResult> LogIn([FromBody] LogInRequestModel logInRequestModel)
    {
        try
        {
            AuthenticationResult result = await _authenticationDataAccess.LogInAsync(logInRequestModel.Username!, logInRequestModel.Password!)!;

            if (result.ErrorCode == ErrorCode.InvalidCredentials)
                return BadRequest(new { ErrorMessage = "InvalidCredentialsError" });

            return Ok(new { Token = result.Token });
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpPost("LogOut")]
    public async Task<IActionResult> LogOut()
    {
        try
        {
            string authorizationHeader = Request.Headers["Authorization"]!;
            if(authorizationHeader.IsNullOrEmpty() || !authorizationHeader.StartsWith("Bearer "))
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            ErrorCode errorCode = await _authenticationDataAccess.LogOutAsync(token)!;

            if (errorCode == ErrorCode.InvalidAccessToken)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

}
