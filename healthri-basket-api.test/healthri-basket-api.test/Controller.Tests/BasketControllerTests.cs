using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using healthri_basket_api.Controllers;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace healthri_basket_api.Tests.Controllers
{
    public class BasketsControllerTests
    {
        private readonly Mock<IBasketService> _mockService;
        private readonly BasketsController _controller;

        public BasketsControllerTests()
        {
            _mockService = new Mock<IBasketService>();
            _controller = new BasketsController(_mockService.Object);
        }

        [Fact]
        // Should return 200 status code and all baskets owned by user
        public async Task GetUserBaskets()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            List<Basket> userBaskets = new List<Basket> { new() { Id = Guid.NewGuid(), Name = "Test" } };
            _mockService.Setup(s => s.GetBasketsAsync(userId)).ReturnsAsync(userBaskets);

            // Act
            var result = await _controller.GetUserBaskets(userId);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userBaskets, okResult.Value);
        }

        [Fact]
        // Should return 200 status code and basket found by id
        public async Task GetBasketByIdTest()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Basket basket = new() { Id = id, Name = "Sample" };
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(basket);

            // Act
            var result = await _controller.Get(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(basket, okResult.Value);
        }

        [Fact]
        // Should return 404 status code because basket doesn't exist
        public async Task GetBasketById_BasketNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Basket? basket = null;
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(basket);

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Should return 201 status code, creating and returning the newly created basket
        public async Task CreateNewBasket()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "New Basket";
            Basket basket = new Basket { Id = Guid.NewGuid(), Name = name };

            _mockService.Setup(s => s.CreateBasketAsync(userId, name, false)).ReturnsAsync(basket);

            // Act
            var result = await _controller.Create(userId, name);

            // Assert
            var contentResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(basket, contentResult.Value);
        }

        [Fact]
        // Should return 200 status code, renaming the given basket.
        public async Task RenameBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string name = "Updated Name";

            _mockService.Setup(s => s.RenameBasketAsync(id, name)).ReturnsAsync(true);

            // Act
            var result = await _controller.Rename(id, name);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 404 status code, because basket doesn't exist.
        public async Task RenameBasket_BasketNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string name = "Doesn't Exist";
            _mockService.Setup(s => s.RenameBasketAsync(id, name)).ReturnsAsync(false);

            // Act
            var result = await _controller.Rename(id, name);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully archiving the basket.
        public async Task ArchiveBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            _mockService.Setup(s => s.ArchiveBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Archive(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully restoring the basket.
        public async Task RestoreBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            _mockService.Setup(s => s.RestoreBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Restore(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully deleting the basket.
        public async Task DeleteBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            _mockService.Setup(s => s.DeleteBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should retunr 404 status code, because basket doesn't exist.
        public async Task DeleteBasket_BasketNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = false;
            _mockService.Setup(s => s.DeleteBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully clearing all items from the basket.
        public async Task ClearBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            _mockService.Setup(s => s.ClearBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Clear(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 404 status code, because basket doesn't exist.
        public async Task ClearBasket_BasketNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = false;
            _mockService.Setup(s => s.ClearBasketAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Clear(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully adding basketItem to the basket.
        public async Task AddItemToBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            BasketItem item = new BasketItem { ItemId = "abc123", Source = "sourceA" };
            _mockService.Setup(s => s.AddItemAsync(id, item.ItemId, item.Source)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddItem(id, item);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 404 status code, because basketItem doesn't exist.
        public async Task AddItemToBasket_ItemNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = false;
            BasketItem item = new BasketItem { ItemId = "notfound", Source = "sourceB" };
            _mockService.Setup(s => s.AddItemAsync(id, item.ItemId, item.Source)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddItem(id, item);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Should return 200 status code, successfully removing the item from the basket.
        public async Task RemoveItemFromBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = true;
            string itemId = "item123";
            _mockService.Setup(s => s.RemoveItemAsync(id, itemId)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.RemoveItem(id, itemId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        // Should return 404 status code, because basketItem to remove was not found in the basket.
        public async Task RemoveItemFromBasket_ItemNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            bool expectedResponse = false;
            string itemId = "missingItem";
            _mockService.Setup(s => s.RemoveItemAsync(id, itemId)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.RemoveItem(id, itemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
