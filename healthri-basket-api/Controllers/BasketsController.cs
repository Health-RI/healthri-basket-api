using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using healthri_basket_api.Controllers.DTOs;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace healthri_basket_api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/baskets")]
public class BasketsController(IBasketService service) : ControllerBase
{
    private bool TryGetUserId(out Guid userId, out IActionResult? errorResult)
    {
        var rawUserId =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (rawUserId == null || !Guid.TryParse(rawUserId, out userId))
        {
            errorResult = Unauthorized("User ID in token invalid or not found.");
            userId = Guid.Empty;
            return false;
        }

        errorResult = null;
        return true;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUserBaskets(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        IEnumerable<Basket> baskets = await service.GetByUserIdAsync(userId, ct);
        return Ok(baskets);
    }
    
    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> Get(Guid basketId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var basket = await service.GetByIdAsync(userId, basketId, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketDTO createBasketDTO, CancellationToken ct)
    {
        try
        {
            if (!TryGetUserId(out var userId, out var error))
                return error!;

            Basket basket = await service.CreateAsync(userId, createBasketDTO.Name, createBasketDTO.IsDefault, ct);
            return CreatedAtAction(
                nameof(Get),
                new { basketId = basket.Id },
                basket);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{basketId:guid}/rename")]
    public async Task<IActionResult> Rename(Guid basketId, [FromBody] string name, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.RenameAsync(userId, basketId, name, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid basketId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.ArchiveAsync(userId, basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid basketId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.RestoreAsync(userId, basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{basketId:guid}")]
    public async Task<IActionResult> Delete(Guid basketId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.DeleteAsync(userId, basketId, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{basketId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid basketId, [FromBody] Guid itemId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.AddItemAsync(userId, basketId, itemId, BasketItemSource.CatalogPage, ct);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpDelete("{basketId:guid}/items")]
    public async Task<IActionResult> Clear(Guid basketId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.ClearAsync(userId, basketId, ct);
        return result ? Ok() : NotFound();
    }


    [HttpDelete("{basketId:guid}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(Guid basketId, Guid itemId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId, out var error))
            return error!;

        var result = await service.RemoveItemAsync(userId, basketId, itemId, BasketItemSource.CatalogPage, ct);
        return result ? Ok() : NotFound();
    }
}
