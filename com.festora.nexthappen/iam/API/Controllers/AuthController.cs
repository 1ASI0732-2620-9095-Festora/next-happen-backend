using com.festora.nexthappen.iam.Application.DTOs;
using com.festora.nexthappen.iam.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace com.festora.nexthappen.iam.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUser _registerUser;
    private readonly LoginUser _loginUser;

    public AuthController(RegisterUser registerUser, LoginUser loginUser)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _registerUser.HandleAsync(request);
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _loginUser.HandleAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}