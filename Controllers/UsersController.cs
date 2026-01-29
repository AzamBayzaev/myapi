using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Interfaces;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var currentUserId = int.Parse(User.FindFirst("nameid")!.Value);
        var role = User.FindFirst("role")!.Value;
        if (currentUserId != id && role != "admin")
            return Forbid();

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        if (user == null) return BadRequest("Failed to create user");
        return Ok(user);
    }

   
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var currentUserId = int.Parse(User.FindFirst("nameid")!.Value);
        var role = User.FindFirst("role")!.Value;

        if (currentUserId != id && role != "admin")
            return Forbid();

        var user = await _userService.UpdateAsync(id, dto);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userService.DeleteAsync(id);
        if (user == null) return NotFound();

        return Ok(user);
    }

  
    [HttpPost("{id}/role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SetRole(int id, [FromQuery] string role)
    {
        var result = await _userService.SetRoleAsync(id, role);
        if (!result) return NotFound();
        return Ok(new { Message = $"User {id} role set to {role}" });
    }
}
