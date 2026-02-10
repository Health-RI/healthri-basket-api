using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using healthri_basket_api.Services;
using Moq;

namespace healthri_basket_api.test.Services.Tests
{
    public class BasketServiceTests
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IItemService> _itemServiceMock;
        private readonly Mock<ITransactionLogger> _loggerMock;
        private readonly BasketService _basketService;
        private readonly CancellationToken _ct;

        public BasketServiceTests()
        {
            _ct = CancellationToken.None;

            _basketRepositoryMock = new Mock<IBasketRepository>();
            _itemServiceMock = new Mock<IItemService>();
            _loggerMock = new Mock<ITransactionLogger>();

            _basketService = new BasketService(
                _basketRepositoryMock.Object,
                _itemServiceMock.Object,
                _loggerMock.Object
            );
        }

        private Basket CreateBasketWithItems(Guid? basketId)
        {
            Guid userId = Guid.NewGuid();
            var basket = new Basket(userId, "test-basket", "TestBasket", true);
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
        public async Task CreateAsync_WhenCalled_ReturnsActiveBasketWithCorrectDetails()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "Test Basket";
            bool isDefault = false;
            BasketStatus expectedStatus = BasketStatus.Active;

            // Act 
            Basket createdBasket = await _basketService.CreateAsync(userId, name, isDefault, null, _ct);

            // Assert
            Assert.NotNull(createdBasket);
            Assert.Equal(name, createdBasket.Name);
            Assert.Equal(isDefault, createdBasket.IsDefault);
            Assert.Equal(userId, createdBasket.UserId);
            Assert.Equal(expectedStatus, createdBasket.Status);
            Assert.Empty(createdBasket.Items);

            _basketRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Basket>(), _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(userId, createdBasket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidCustomSlug_UsesCustomSlug()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "Test Basket";
            string customSlug = "my-custom-slug";
            bool isDefault = false;

            // Act
            Basket createdBasket = await _basketService.CreateAsync(userId, name, isDefault, customSlug, _ct);

            // Assert
            Assert.NotNull(createdBasket);
            Assert.Equal(customSlug, createdBasket.Slug);
            Assert.Equal(name, createdBasket.Name);

            _basketRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Basket>(), _ct), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCustomSlug_ThrowsArgumentException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "Test Basket";
            string invalidSlug = "My Invalid Slug";
            bool isDefault = false;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _basketService.CreateAsync(userId, name, isDefault, invalidSlug, _ct));
            
            Assert.Contains("not valid", exception.Message);
            Assert.Contains("lowercase", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateSlug_AppendsNumberSuffix()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "Test Basket";
            bool isDefault = false;
            
            // Mock existing basket with slug "test-basket"
            var existingBasket = new Basket(userId, "test-basket", "Existing", false);
            
            // First call returns existing basket, second call returns null (slug available)
            _basketRepositoryMock
                .SetupSequence(r => r.GetBySlugAsync(userId, It.IsAny<string>(), _ct))
                .ReturnsAsync(existingBasket)  // "test-basket" exists
                .ReturnsAsync((Basket?)null);   // "test-basket-1" is available

            // Act
            Basket createdBasket = await _basketService.CreateAsync(userId, name, isDefault, null, _ct);

            // Assert
            Assert.NotNull(createdBasket);
            Assert.Equal("test-basket-1", createdBasket.Slug);
            Assert.Equal(name, createdBasket.Name);

            _basketRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Basket>(), _ct), Times.Once);
        }

        [Fact]
        public async Task RenameAsync_WhenBasketExists_ReturnsTrueAndUpdatesName()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            string newName = "NewBasketName";
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.RenameAsync(basket.UserId, basket.Slug, newName, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(newName, basket.Name);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenBasketIsNotDefault_ReturnsTrueAndSetsStatusToDeleted()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            basket.IsDefault = false;
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act
            bool success = await _basketService.DeleteAsync(basket.UserId, basket.Slug, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Deleted, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.DeleteBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_WhenBasketExists_ReturnsTrueAndSetsStatusToActive()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            basket.Status = BasketStatus.Archived;
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.RestoreAsync(basket.UserId, basket.Slug, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Active, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RestoreBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task ArchiveAsync_WhenBasketExists_ReturnsTrueAndSetsStatusToArchived()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.ArchiveAsync(basket.UserId, basket.Slug, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Archived, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ArchiveBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task ClearAsync_WhenBasketExists_ReturnsTrueAndRemovesAllItems()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act 
            var success = await _basketService.ClearAsync(basket.UserId, basket.Slug, _ct);

            // Assert
            Assert.True(success);
            Assert.Empty(basket.Items);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ClearBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_WhenItemAndBasketExist_ReturnsUpdatedBasketAndAddsItem()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Item item = new Item("Item 4", "Description 4");
            BasketItemSource source = BasketItemSource.CatalogPage;
            bool expectedResponse = true;

            // Mock basket (start empty)
            var basket = CreateBasketWithItems(basketId);
            var basketItem = new BasketItem(basket, item);
            basket.Items.Clear();

            // Mock repository returns basket
            _basketRepositoryMock
                .Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct))
                .ReturnsAsync(basket);

            // Mock item service returns item
            _itemServiceMock
                .Setup(s => s.GetByIdAsync(item.Id, _ct))
                .ReturnsAsync(item);

            // Mock AddItemAsync returns true
            _basketRepositoryMock
                .Setup(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct))
                .ReturnsAsync(expectedResponse);


            // Act
            Basket? result = await _basketService.AddItemAsync(basket.UserId, basket.Slug, item.Id, source, _ct);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items); // Expect 1 item added
            Assert.Equal(item.Id, result.Items.First().ItemId);

            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.Is<BasketItem>(
                bi => bi.ItemId == item.Id && bi.BasketId == basket.Id
            ), _ct), Times.Once);

            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basketId, item.Id, BasketAction.AddItem, source), Times.Once);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenCalled_ReturnsBaskets()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var baskets = new List<Basket> { new Basket(userId, "basket-1", "Basket 1", false) };
            _basketRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, _ct)).ReturnsAsync(baskets);

            // Act
            var result = await _basketService.GetByUserIdAsync(userId, _ct);

            // Assert
            Assert.Equal(baskets, result);
        }

        [Fact]
        public async Task GetBySlugAsync_WhenBasketNotFound_ReturnsNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "missing-basket";
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync((Basket?)null);

            // Act
            var result = await _basketService.GetBySlugAsync(userId, slug, _ct);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBySlugAsync_WhenUserMismatch_ReturnsNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "basket";
            Basket basket = new Basket(Guid.NewGuid(), slug, "Basket", false);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync(basket);

            // Act
            var result = await _basketService.GetBySlugAsync(userId, slug, _ct);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RenameAsync_WhenBasketNotFound_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "missing-basket";
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync((Basket?)null);

            // Act
            var success = await _basketService.RenameAsync(userId, slug, "Name", _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenBasketIsDefault_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "default-basket";
            Basket basket = new Basket(userId, slug, "Basket", true);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync(basket);

            // Act
            var success = await _basketService.DeleteAsync(userId, slug, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task RestoreAsync_WhenUserMismatch_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "basket";
            Basket basket = new Basket(Guid.NewGuid(), slug, "Basket", false);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync(basket);

            // Act
            var success = await _basketService.RestoreAsync(userId, slug, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task ArchiveAsync_WhenBasketNotFound_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "missing-basket";
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync((Basket?)null);

            // Act
            var success = await _basketService.ArchiveAsync(userId, slug, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task ClearAsync_WhenUserMismatch_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "basket";
            Basket basket = new Basket(Guid.NewGuid(), slug, "Basket", false);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync(basket);

            // Act
            var success = await _basketService.ClearAsync(userId, slug, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task AddItemAsync_WhenBasketNotFound_ReturnsNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "missing-basket";
            Guid itemId = Guid.NewGuid();
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync((Basket?)null);

            // Act
            var result = await _basketService.AddItemAsync(userId, slug, itemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.Null(result);
            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task AddItemAsync_WhenUserMismatch_ReturnsNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "basket";
            Guid itemId = Guid.NewGuid();
            Basket basket = new Basket(Guid.NewGuid(), slug, "Basket", false);
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync(basket);

            // Act
            var result = await _basketService.AddItemAsync(userId, slug, itemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.Null(result);
            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task AddItemAsync_WhenItemAlreadyInBasket_ReturnsNull()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(basketId);
            Guid existingItemId = basket.Items.First().ItemId;
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act
            var result = await _basketService.AddItemAsync(basket.UserId, basket.Slug, existingItemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.Null(result);
            _itemServiceMock.Verify(s => s.GetByIdAsync(It.IsAny<Guid>(), _ct), Times.Never);
            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task AddItemAsync_WhenItemNotFound_ReturnsNull()
        {
            // Arrange
            Guid basketId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(basketId);
            Guid itemId = Guid.NewGuid();
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);
            _itemServiceMock.Setup(s => s.GetByIdAsync(itemId, _ct)).ReturnsAsync((Item?)null);

            // Act
            var result = await _basketService.AddItemAsync(basket.UserId, basket.Slug, itemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.Null(result);
            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task RemoveItemAsync_WhenItemExistsInBasket_ReturnsTrueAndRemovesItem()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            Guid itemToRemoveId = basket.Items.First().ItemId;
            BasketItemSource source = BasketItemSource.UserPage;

            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(basket.UserId, basket.Slug, _ct)).ReturnsAsync(basket);

            // Act 
            var success = await _basketService.RemoveItemAsync(basket.UserId, basket.Slug, itemToRemoveId, source, _ct);

            // Assert
            Assert.True(success);
            Assert.DoesNotContain(basket.Items, bi => bi.ItemId == itemToRemoveId);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, itemToRemoveId, BasketAction.RemoveItem, source), Times.Once);
        }

        [Fact]
        public async Task RemoveItemAsync_WhenBasketNotFound_ReturnsFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string slug = "missing-basket";
            Guid itemId = Guid.NewGuid();
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(userId, slug, _ct)).ReturnsAsync((Basket?)null);

            // Act
            var success = await _basketService.RemoveItemAsync(userId, slug, itemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

        [Fact]
        public async Task RemoveItemAsync_WhenUserMismatch_ReturnsFalse()
        {
            // Arrange
            Guid itemId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(Guid.NewGuid());
            string slug = basket.Slug;
            _basketRepositoryMock.Setup(r => r.GetBySlugAsync(It.IsAny<Guid>(), slug, _ct)).ReturnsAsync(basket);

            // Act
            var success = await _basketService.RemoveItemAsync(Guid.NewGuid(), slug, itemId, BasketItemSource.UserPage, _ct);

            // Assert
            Assert.False(success);
            _basketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Basket>(), _ct), Times.Never);
            _loggerMock.Verify(l => l.LogAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BasketAction>(), It.IsAny<BasketItemSource>()), Times.Never);
        }

    }
}
