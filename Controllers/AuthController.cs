using Microsoft.AspNetCore.Mvc;
using MyApi.Service.interfaces;
using MyApi.Dtos;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, error) = await _auth.RegisterAsync(dto);
        if (!success) return BadRequest(error);
        return Ok("Registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (success, token, error) = await _auth.LoginAsync(dto);
        if (!success) return Unauthorized(error);
        return Ok(new { accessToken = token });
    }
}