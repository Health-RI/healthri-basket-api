using Microsoft.AspNetCore.Mvc;
using healthri_basket_api.Models;
using healthri_basket_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    public async Task<IActionResult> Get(Guid id)
    {
        var basket = await service.GetByIdAsync(id);
        return basket == null ? NotFound() : Ok(basket);
    }

    [HttpPost("{userUuid:guid}")]
    public async Task<IActionResult> Create(Guid userUuid, [FromBody] string name)
    {
        var basket = await service.CreateBasketAsync(userUuid, name);
        return CreatedAtAction(nameof(Get), new { id = basket.Id }, basket);
    }

    [HttpPut("{id:guid}/rename")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] string name)
    {
        var result = await service.RenameBasketAsync(id, name);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await service.ArchiveBasketAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await service.RestoreBasketAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await service.DeleteBasketAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}/clear")]
    public async Task<IActionResult> Clear(Guid id)
    {
        var result = await service.ClearBasketAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] BasketItem item)
    {
        var result = await service.AddItemAsync(id, item.ItemId, item.Source);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(Guid id, string itemId)
    {
        var result = await service.RemoveItemAsync(id, itemId);
        return result ? Ok() : NotFound();
    }
}