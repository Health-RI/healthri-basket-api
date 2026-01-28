#if DEBUG
using healthri_basket_api.Interfaces;
using System.Security.Claims;

namespace healthri_basket_api.Services;

public class DebugUserIdProvider : IUserIdProvider
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid GetUserId(ClaimsPrincipal user) => TestUserId;
}
#endif
