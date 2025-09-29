using Microsoft.AspNetCore.Mvc;
using healthri_basket_api.Models;
using healthri_basket_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Controllers;

[ApiController]
[Route("api/v1/baskets")]
public class BasketsController(IBasketService service) : ControllerBase
{

    [Authorize(Roles = "admin")]
    [HttpGet("{userUuid}")]
    public async Task<IActionResult> GetUserBaskets()
    {
        if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userObjectGuid))
        {

            var userObjectId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var baskets = await service.GetBasketsAsync(userObjectGuid);
            return Ok(baskets);
        }

        return BadRequest();
    }
    
    [HttpGet("single/{id:guid}")]
    public async Task<IActionResult> Get(Guid basketId)
    {
        var basket = await service.GetByIdAsync(basketId);
        return basket == null ? NotFound() : Ok(basket);
    }

    [HttpPost("{userUuid:guid}")]
    public async Task<IActionResult> Create(Guid userUuid, [FromBody] string name)
    {
        var basket = await service.CreateBasketAsync(userUuid, name);
        return CreatedAtAction(nameof(Get), new { id = basket.Id }, basket);
    }

    [HttpPut("{basketId:guid}/rename")]
    public async Task<IActionResult> Rename(Guid basketId, [FromBody] string name)
    {
        var result = await service.RenameBasketAsync(basketId, name);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid basketId)
    {
        var result = await service.ArchiveBasketAsync(basketId);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid basketId)
    {
        var result = await service.RestoreBasketAsync(basketId);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{basketId:guid}")]
    public async Task<IActionResult> Delete(Guid basketId)
    {
        var result = await service.DeleteBasketAsync(basketId);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{basketId:guid}/clear")]
    public async Task<IActionResult> Clear(Guid basketId)
    {
        var result = await service.ClearBasketAsync(basketId);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid basketId, [FromBody] Guid itemId)
    {
        var result = await service.AddItemToBasketAsync(basketId, itemId, BasketItemSource.CatalogPage);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{basketId:guid}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(Guid basketId, Guid itemId)
    {
        var result = await service.RemoveItemFromBasketAsync(basketId, itemId, BasketItemSource.CatalogPage);
        return result ? Ok() : NotFound();
    }
}