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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        IEnumerable<Basket> baskets = await service.GetByUserIdAsync(userId, ct);
        return Ok(baskets);
    }
    
    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var basket = await service.GetBySlugAsync(userId, slug, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketDTO createBasketDTO, CancellationToken ct)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Basket basket = await service.CreateAsync(userId, createBasketDTO.Name, createBasketDTO.IsDefault, createBasketDTO.Slug, ct);
            return CreatedAtAction(
                nameof(Get),
                new { slug = basket.Slug },
                basket);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{slug}/rename")]
    public async Task<IActionResult> Rename(string slug, [FromBody] string name, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.RenameAsync(userId, slug, name, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/archive")]
    public async Task<IActionResult> Archive(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.ArchiveAsync(userId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/restore")]
    public async Task<IActionResult> Restore(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.RestoreAsync(userId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.DeleteAsync(userId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/items")]
    public async Task<IActionResult> AddItem(string slug, [FromBody] Guid itemId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.AddItemAsync(userId, slug, itemId, BasketItemSource.CatalogPage, ct);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpDelete("{slug}/items")]
    public async Task<IActionResult> Clear(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.ClearAsync(userId, slug, ct);
        return result ? Ok() : NotFound();
    }


    [HttpDelete("{slug}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(string slug, Guid itemId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.RemoveItemAsync(userId, slug, itemId, BasketItemSource.CatalogPage, ct);
        return result ? Ok() : NotFound();
    }
}
