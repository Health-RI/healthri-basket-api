using System.Security.Claims;

namespace healthri_basket_api.Interfaces;

public interface IUserIdProvider
{
    Guid GetUserId(ClaimsPrincipal user);
}
