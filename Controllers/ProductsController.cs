using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyApi.Interfaces;
using MyApi.Dtos;
namespace MyApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service)
    {
        _service = service;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
    {
        var products = await _service.GetProductsAsync(query);
        return Ok(products);
    }
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }
    [HttpPost]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var sellerId = int.Parse(User.FindFirst("nameid")!.Value);
        var product = await _service.CreateAsync(dto, sellerId);

        return Ok(product);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> Update(int id, ProductCreateDto dto)
    {
        var sellerId = int.Parse(User.FindFirst("nameid")!.Value);
        var result = await _service.UpdateAsync(id, dto, sellerId);

        return result == null ? Forbid() : Ok(result);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = int.Parse(User.FindFirst("nameid")!.Value);
        var success = await _service.DeleteAsync(id, sellerId);

        return success ? Ok() : Forbid();
    }
}