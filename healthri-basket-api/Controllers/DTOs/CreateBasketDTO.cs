namespace healthri_basket_api.Controllers.DTOs;

public class CreateBasketDTO
{
    public required string Name { get; init; }
    public required bool IsDefault { get; init; }
}
