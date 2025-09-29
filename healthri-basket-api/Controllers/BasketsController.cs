using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserBaskets(CancellationToken ct)
    {
        if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userObjectGuid))
        {

            var userObjectId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var baskets = await service.GetByUserIdAsync(userObjectGuid, ct);
            return Ok(baskets);
        }

        return BadRequest();
    }
    
    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> Get(Guid basketId, CancellationToken ct)
    {
        var basket = await service.GetByIdAsync(basketId, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> Create(Guid userId, [FromBody] string name, CancellationToken ct)
    {
        var basket = await service.CreateAsync(userId, name, false, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPut("{basketId:guid}/rename")]
    public async Task<IActionResult> Rename(Guid basketId, [FromBody] string name, CancellationToken ct)
    {
        var result = await service.RenameAsync(basketId, name, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid basketId, CancellationToken ct)
    {
        var result = await service.ArchiveAsync(basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid basketId, CancellationToken ct)
    {
        var result = await service.RestoreAsync(basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{basketId:guid}")]
    public async Task<IActionResult> Delete(Guid basketId, CancellationToken ct)
    {
        var result = await service.DeleteAsync(basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid basketId, [FromBody] Guid itemId, CancellationToken ct)
    {
        var result = await service.AddItemAsync(basketId, itemId, BasketItemSource.CatalogPage, ct);
        return result == null ? Ok(result) : NotFound();
    }

    [HttpDelete("{basketId:guid}/items")]
    public async Task<IActionResult> Clear(Guid basketId, CancellationToken ct)
    {
        var result = await service.ClearAsync(basketId, ct);
        return result ? Ok() : NotFound();
    }


    [HttpDelete("{basketId:guid}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(Guid basketId, Guid itemId, CancellationToken ct)
    {
        var result = await service.RemoveItemAsync(basketId, itemId, BasketItemSource.CatalogPage, ct);
        return result ? Ok() : NotFound();
    }
}