using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApi.Dtos
{
    public class ProductDto
    {
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; } 

        public string Name { get; set; }
        public decimal Price { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public int UserId { get; set; }     
    }
}