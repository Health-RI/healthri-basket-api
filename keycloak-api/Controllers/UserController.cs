using keycloak_api.DTOs;
using keycloak_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace keycloak_api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        try
        {
            var result = await _userService.RegisterAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var token = await _userService.LoginAsync(request);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
