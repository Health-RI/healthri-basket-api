using healthri_basket_api.Controllers;
using healthri_basket_api.Controllers.DTOs;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace healthri_basket_api.test.Controller.Tests
{
    public class BasketsControllerTests
    {
        private readonly Mock<IBasketService> _basketServiceMock;
        private readonly BasketsController _basketController;
        private readonly CancellationToken _ct;

        public BasketsControllerTests()
        {
            _ct = CancellationToken.None;

            // Initialize mocks once for all tests
            _basketServiceMock = new Mock<IBasketService>();

            // Instantiate the controller using the mocks
            _basketController = new BasketsController(_basketServiceMock.Object);
        }

        private void SetAuthenticatedUser(Guid userId)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("sub", userId.ToString()),
                    },
                    "mock"));

            _basketController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private Basket CreateBasketWithItems(Guid? basketId)
        {
            Guid userId = Guid.NewGuid();
            var basket = new Basket(userId, "TestBasket", true);
            var items = new List<Item>
            {
                new Item("Item 1", "Description 1"),
                new Item("Item 2", "Description 2"),
                new Item("Item 3", "Description 3"),
            };
            foreach (var item in items)
            {
                basket.AddItem(item);
            }

            if (basketId.HasValue)
            {
                basket.Id = basketId.Value;
            }

            return basket;
        }

        [Fact]
        public async Task GetUserBaskets_WhenCalled_ReturnsOkWithUserBaskets()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            List<Basket> userBaskets = new List<Basket> { new Basket(userId, "User basket", true) };

            _basketServiceMock.Setup(s => s.GetByUserIdAsync(userId, _ct)).ReturnsAsync(userBaskets);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.GetUserBaskets(_ct);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userBaskets, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenBasketExists_ReturnsOkWithBasket()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);

            _basketServiceMock.Setup(s => s.GetByIdAsync(expectedBasket.UserId, expectedBasket.Id, _ct)).ReturnsAsync(expectedBasket);

            SetAuthenticatedUser(expectedBasket.UserId);

            // Act
            IActionResult result = await _basketController.Get(expectedBasket.Id, _ct);

            // Assert
            OkObjectResult actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedBasket, actionResult.Value);
        }

        [Fact]
        public async Task GetById_WhenBasketDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket? expectedBasket = null;

            _basketServiceMock.Setup(s => s.GetByIdAsync(userId, basketId, _ct)).ReturnsAsync(expectedBasket);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Get(basketId, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenValidInput_ReturnsCreatedAtActionWithBasket()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid basketId = Guid.NewGuid();
            CreateBasketDTO createBasketDTO = new CreateBasketDTO
            {
                Name = "My Basket",
                IsDefault = true
            };

            Basket expectedBasket = new Basket(userId, createBasketDTO.Name, createBasketDTO.IsDefault)
            {
                Id = basketId
            };

            SetAuthenticatedUser(userId);

            _basketServiceMock
                .Setup(s => s.CreateAsync(userId, createBasketDTO.Name, createBasketDTO.IsDefault, _ct))
                .ReturnsAsync(expectedBasket);

            // Act
            IActionResult result = await _basketController.Create(createBasketDTO, _ct);

            // Assert
            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Basket returnedBasket = Assert.IsType<Basket>(createdAtResult.Value);
            Assert.Equal(expectedBasket, returnedBasket);
        }


        [Fact]
        public async Task Rename_WhenBasketExists_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            string newName = "New basket name";
            bool expectedResponse = true;

            _basketServiceMock.Setup(s => s.RenameAsync(userId, basketId, newName, _ct)).ReturnsAsync(expectedResponse);

            // Mock authenticated user
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("sub", userId.ToString()),
            ], "mock"));

            _basketController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            IActionResult result = await _basketController.Rename(basketId, newName, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Rename_WhenBasketDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string newName = "New basket name";
            bool expectedResponse = false;

            _basketServiceMock.Setup(s => s.RenameAsync(userId, basketId, newName, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Rename(basketId, newName, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Archive_WhenBasketExists_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = true;

            _basketServiceMock.Setup(s => s.ArchiveAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Archive(basketId, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Restore_WhenBasketExists_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = true;

            _basketServiceMock.Setup(s => s.RestoreAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Restore(basketId, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_WhenBasketExists_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = true;

            _basketServiceMock.Setup(s => s.DeleteAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            // Mock authenticated user
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("sub", userId.ToString()),
            ], "mock"));

            _basketController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            IActionResult result = await _basketController.Delete(basketId, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_WhenBasketDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = false;

            _basketServiceMock.Setup(s => s.DeleteAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Delete(basketId, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Clear_WhenBasketExists_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = true;

            _basketServiceMock.Setup(s => s.ClearAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Clear(basketId, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Clear_WhenBasketDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            bool expectedResponse = false;

            _basketServiceMock.Setup(s => s.ClearAsync(userId, basketId, _ct)).ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.Clear(basketId, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddItem_WhenItemIsAddedSuccessfully_ReturnsOkWithUpdatedBasket()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            Item item = new Item("title a", "description a");
            expectedBasket.ClearItems(); // Start with an empty basket for this test
            expectedBasket.AddItem(item);

            _basketServiceMock
                .Setup(s => s.AddItemAsync(expectedBasket.UserId, expectedBasket.Id, item.Id, BasketItemSource.CatalogPage, _ct))
                .ReturnsAsync(expectedBasket);

            SetAuthenticatedUser(expectedBasket.UserId);

            // Act
            IActionResult result = await _basketController.AddItem(expectedBasket.Id, item.Id, _ct);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBasket = Assert.IsType<Basket>(okResult.Value);

            Assert.Equal(expectedBasket, returnedBasket);

            Item firstItem = returnedBasket.Items[0].Item;
            Assert.Equal(firstItem.Id, item.Id);
            Assert.Equal(firstItem.Title, item.Title);
            Assert.Equal(firstItem.Description, item.Description);
        }


        [Fact]
        public async Task AddItem_WhenItemOrBasketDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _basketServiceMock.Setup(s => s.AddItemAsync(userId, basketId, itemId, BasketItemSource.CatalogPage, _ct));

            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.AddItem(basketId, itemId, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveItem_WhenItemIsRemovedSuccessfully_ReturnsOk()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Basket expectedBasket = CreateBasketWithItems(basketId);
            Item itemToRemove = expectedBasket.Items[0].Item;
            bool expectedResponse = true;
            expectedBasket.RemoveItem(itemToRemove.Id);


            _basketServiceMock
                .Setup(s => s.RemoveItemAsync(expectedBasket.UserId, basketId, itemToRemove.Id, BasketItemSource.CatalogPage, _ct)) 
                .ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(expectedBasket.UserId);

            // Act
            IActionResult result = await _basketController.RemoveItem(basketId, itemToRemove.Id, _ct);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(2, expectedBasket.Items.Count);
            Assert.DoesNotContain(expectedBasket.Items, bi => bi.Item.Id == itemToRemove.Id);
        }


        [Fact]
        public async Task RemoveItem_WhenItemDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            bool expectedResponse = false;

            _basketServiceMock.Setup(s => s.RemoveItemAsync(userId, basketId, itemId, BasketItemSource.CatalogPage, _ct)).ReturnsAsync(expectedResponse);
            
            SetAuthenticatedUser(userId);

            // Act
            IActionResult result = await _basketController.RemoveItem(basketId, itemId, _ct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
