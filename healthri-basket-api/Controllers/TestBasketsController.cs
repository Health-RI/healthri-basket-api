using healthri_basket_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace healthri_basket_api.Controllers;

#if DEBUG
[ApiController]
[Route("api/v1/test/baskets")]
public class TestBasketsController(IBasketService service, IUserIdProvider userIdProvider)
    : BasketsController(service, userIdProvider)
{
}
#endif
