namespace healthri_basket_api.Controllers.DTOs;

public class CreateBasketDTO
{
    required public string Name { get; set; }
    required public bool IsDefault { get; set; }
}
