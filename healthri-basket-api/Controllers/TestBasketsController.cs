using healthri_basket_api.Controllers.DTOs;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace healthri_basket_api.Controllers;

#if DEBUG
[ApiController]
[Route("api/v1/test/baskets")]
public class TestBasketsController(IBasketService service) : ControllerBase
{
    // Hardcoded test user ID - ONLY FOR DEVELOPMENT
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [HttpGet]
    public async Task<IActionResult> GetUserBaskets(CancellationToken ct)
    {
        IEnumerable<Basket> baskets = await service.GetByUserIdAsync(_testUserId, ct);
        return Ok(baskets);
    }
    
    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug, CancellationToken ct)
    {
        var basket = await service.GetBySlugAsync(_testUserId, slug, ct);
        return basket != null ? Ok(basket) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketDTO createBasketDTO, CancellationToken ct)
    {
        try
        {
            Basket basket = await service.CreateAsync(_testUserId, createBasketDTO.Name, createBasketDTO.IsDefault, createBasketDTO.Slug, ct);
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
        var result = await service.RenameAsync(_testUserId, slug, name, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/archive")]
    public async Task<IActionResult> Archive(string slug, CancellationToken ct)
    {
        var result = await service.ArchiveAsync(_testUserId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/restore")]
    public async Task<IActionResult> Restore(string slug, CancellationToken ct)
    {
        var result = await service.RestoreAsync(_testUserId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug, CancellationToken ct)
    {
        var result = await service.DeleteAsync(_testUserId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpPost("{slug}/items")]
    public async Task<IActionResult> AddItem(string slug, [FromBody] Guid itemId, CancellationToken ct)
    {
        var result = await service.AddItemAsync(_testUserId, slug, itemId, BasketItemSource.CatalogPage, ct);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpDelete("{slug}/items")]
    public async Task<IActionResult> Clear(string slug, CancellationToken ct)
    {
        var result = await service.ClearAsync(_testUserId, slug, ct);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{slug}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(string slug, Guid itemId, CancellationToken ct)
    {
        var result = await service.RemoveItemAsync(_testUserId, slug, itemId, BasketItemSource.CatalogPage, ct);
        return result ? Ok() : NotFound();
    }
}
#endif
