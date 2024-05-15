using Microsoft.AspNetCore.Mvc;
using TodoAPIAssignment.API.Models.RequestModels;
using TodoAPIAssignment.AuthenticationLibrary;

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
            string result = await _authenticationDataAccess.SignUpAsync(signUpRequestModel.Username!, 
                signUpRequestModel.Password!, signUpRequestModel.Email!)!;
            
            if(result == "DuplicateUsernameError")
                return BadRequest(new {ErrorMessage = "DuplicateUsernameError"});
            else if (result == "DuplicateEmailError")
                return BadRequest(new { ErrorMessage = "DuplicateEmailError" });

            return Ok(new { Token = result });
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error"); ;
        }
    }

    [HttpPost("LogIn")]
    public async Task<IActionResult> LogIn([FromBody] LogInRequestModel logInRequestModel)
    {
        try
        {
            string result = await _authenticationDataAccess.LogInAsync(logInRequestModel.Username!, logInRequestModel.Password!)!;

            if (result == "InvalidCredentials")
                return BadRequest(new { ErrorMessage = "InvalidCredentialsError" });

            return Ok(new { Token = result });
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error"); ;
        }
    }
}
