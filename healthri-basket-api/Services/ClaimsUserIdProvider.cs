using healthri_basket_api.Interfaces;
using System.Security.Claims;

namespace healthri_basket_api.Services;

public class ClaimsUserIdProvider : IUserIdProvider
{
    public Guid GetUserId(ClaimsPrincipal user)
    {
        // Safe to use ! operator since [Authorize] attribute guarantees authentication
        return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
