using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Service.interfaces;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] Page_SortDto sort)
    {
        var res = await _userService.GetProducts(sort);
        return Ok(res);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] Page_SortDto sort)
    {
        var res = await _userService.GetUsers(sort);
        return Ok(res);
    }
}
