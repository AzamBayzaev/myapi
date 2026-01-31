namespace MyApi.Dtos;
public record ProductQueryDto(
    string? Search = null,  
    string SortBy = "name",  
    string SortDir = "asc",  
    int Page = 1,            
    int PageSize = 10        
);