using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Interfaces;
namespace MyApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _userService.RegisterAsync(dto);
        if (user == null) return BadRequest("User already exists");
        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponseDto(token));
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.ValidateUserAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized("Invalid credentials");
        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponseDto(token));
    }
}