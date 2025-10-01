using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using healthri_basket_api.Services;
using Moq;

namespace healthri_basket_api.test.Services.Tests
{
    public class BasketServiceTests
    {
        private BasketService _basketService;
        private Mock<IBasketRepository> _basketRepositoryMock;
        private Mock<IItemService> _itemServiceMock;
        private Mock<ITransactionLogger> _loggerMock;
        private CancellationToken _ct;

        public BasketServiceTests()
        {
            _ct = new CancellationToken();

            // Initialize mocks once for all tests
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _itemServiceMock = new Mock<IItemService>();
            _loggerMock = new Mock<ITransactionLogger>();

            // Instantiate the service using the mocks
            _basketService = new BasketService(
                _basketRepositoryMock.Object,
                _itemServiceMock.Object,
                _loggerMock.Object
            );
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
        public async Task CreateAsync_WhenCalled_ReturnsActiveBasketWithCorrectDetails()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string name = "Test Basket";
            bool isDefault = false;
            BasketStatus expectedStatus = BasketStatus.Active;

            // Act 
            Basket createdBasket = await _basketService.CreateAsync(userId, name, isDefault, _ct);

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
        public async Task RenameAsync_WhenBasketExists_ReturnsTrueAndUpdatesName()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            string newName = "NewBasketName";
            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.RenameAsync(basket.Id, newName, _ct);

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
            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act
            bool success = await _basketService.DeleteAsync(basket.Id, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Deleted, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_WhenBasketExists_ReturnsTrueAndSetsStatusToActive()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            basket.Status = BasketStatus.Archived;
            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.RestoreAsync(basket.Id, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Active, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task ArchiveAsync_WhenBasketExists_ReturnsTrueAndSetsStatusToArchived()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act 
            bool success = await _basketService.ArchiveAsync(basket.Id, _ct);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Archived, basket.Status);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task ClearAsync_WhenBasketExists_ReturnsTrueAndRemovesAllItems()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act 
            var success = await _basketService.ClearAsync(basket.Id, _ct);

            // Assert
            Assert.True(success);
            Assert.Empty(basket.Items);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage), Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_WhenItemAndBasketExist_ReturnsUpdatedBasketAndAddsItem()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            Item item = new Item("Sample Item", "A description of the sample item.");
            BasketItemSource source = BasketItemSource.CatalogPage;

            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);
            _itemServiceMock.Setup(s => s.GetByIdAsync(item.Id, _ct)).ReturnsAsync(item);
            _basketRepositoryMock.Setup(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct)).ReturnsAsync(true);

            // Act
            Basket? basketResult = await _basketService.AddItemAsync(basket.Id, item.Id, source, _ct);

            // Assert
            Assert.NotNull(basketResult);
            Assert.Single(basketResult.Items);
            Assert.Equal(item.Id, basketResult.Items.First().ItemId);

            _basketRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<BasketItem>(), _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, item.Id, BasketAction.AddItem, source), Times.Once);
        }

        [Fact]
        public async Task RemoveItemAsync_WhenItemExistsInBasket_ReturnsTrueAndRemovesItem()
        {
            // Arrange
            Guid baskedId = Guid.NewGuid();
            Basket basket = CreateBasketWithItems(baskedId);
            Guid itemToRemoveId = basket.Items.First().ItemId;
            BasketItemSource source = BasketItemSource.UserPage;

            _basketRepositoryMock.Setup(r => r.GetByIdAsync(basket.Id, _ct)).ReturnsAsync(basket);

            // Act 
            var success = await _basketService.RemoveItemAsync(basket.Id, itemToRemoveId, source, _ct);

            // Assert
            Assert.True(success);
            Assert.DoesNotContain(basket.Items, bi => bi.ItemId == itemToRemoveId);
            
            _basketRepositoryMock.Verify(r => r.UpdateAsync(basket, _ct), Times.Once);
            _loggerMock.Verify(l => l.LogAsync(basket.UserId, basket.Id, itemToRemoveId, BasketAction.RemoveItem, source), Times.Once);
        }
    }
}