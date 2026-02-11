using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Controllers.DTOs;

public class CreateBasketDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public required string Name { get; init; }
    
    [StringLength(100, ErrorMessage = "Slug must not exceed 100 characters")]
    public string? Slug { get; init; }
    
    public required bool IsDefault { get; init; }
}
