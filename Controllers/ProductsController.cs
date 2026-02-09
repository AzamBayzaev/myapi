using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MyApi.Dtos;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService) => _productService = productService;

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Page_SortDto dto)
    {
        var products = await _productService.GetProductAsync(dto);
        return Ok(products);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var product = await _productService.UpdateAsync(userId, id, dto);
        if (product == null)
            return NotFound("Product not found or does not belong to you");
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (deletedProduct, message) = await _productService.DeleteAsync(userId, id);
        if (deletedProduct == null)
            return NotFound("Product not found or does not belong to you");
        return Ok(new { deletedProduct, message });
    }
}
