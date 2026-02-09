using System.ComponentModel.DataAnnotations;
namespace MyApi.Dtos;

public enum SortBy { Id, Name, Price,Email,Role }

public class Page_SortDto
{
    [Range(1,50)]
    public int Page { get; set; }
    public SortBy? SortBy { get; set; }
}