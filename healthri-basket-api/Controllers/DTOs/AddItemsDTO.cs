using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Controllers.DTOs;

public class AddItemsDTO
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one item ID is required.")]
    public required IEnumerable<string> ItemIds { get; init; }
}
