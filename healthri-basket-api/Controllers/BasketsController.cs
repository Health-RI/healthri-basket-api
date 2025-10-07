using healthri_basket_api.Controllers.DTOs;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace healthri_basket_api.Controllers;

[ApiController]
[Route("api/v1/baskets")]
public class BasketsController(IBasketService service) : ControllerBase
{







    [Authorize]
    [HttpGet("auth")]
    public IActionResult TestAuth(CancellationToken ct)
    {
        return Ok($"This is a private basket area for:");
    }










    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserBaskets(Guid userId, CancellationToken ct)
    {
        IEnumerable<Basket> baskets = await service.GetByUserIdAsync(userId, ct);
        return Ok(baskets);
    }
    
    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> Get(Guid basketId, CancellationToken ct)
    {
        var basket = await service.GetByIdAsync(basketId, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> Create(Guid userId, [FromBody] CreateBasketDTO createBasketDTO, CancellationToken ct)
    {
        try
        {
            Basket basket = await service.CreateAsync(userId, createBasketDTO.Name, createBasketDTO.IsDefault, ct);
            return CreatedAtAction(
                nameof(Get),
                new { basketId = basket.Id },
                basket);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
        return result != null ? Ok(result) : NotFound();
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