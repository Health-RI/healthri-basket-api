using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Services;
using Moq;

namespace healthri_basket_api.test.Services.Tests
{
    public class BasketServiceTests 
    {
        private readonly Mock<IBasketRepository> _basketRepository;
        private readonly Mock<ITransactionLogger> _logger;
        private readonly BasketService _basketService;
        
        public BasketServiceTests()
        {
            _basketRepository = new Mock<IBasketRepository>();
            _logger = new Mock<ITransactionLogger>();
            _basketService = new BasketService(_basketRepository.Object, _logger.Object);
        }

        private Basket CreateDefaultBasket()
        {
            List<BasketItem> basketItems = CreateDefaultBasketItems();

            Basket basket = new Basket
            {
                Id = Guid.NewGuid(),
                UserUuid = Guid.NewGuid(),
                Name = "DefaultBasketName",
                IsDefault = false,
                Status = BasketStatus.Active,
                Items = basketItems,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ArchivedAt = null,
                DeletedAt = null
            };

            // Mock the repository to return this basket when queried by its ID
            _basketRepository.Setup(r => r.GetByIdAsync(basket.Id)).ReturnsAsync(basket);

            return basket;
        }

        private List<BasketItem> CreateDefaultBasketItems()
        {
            return new List<BasketItem>
            {
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "bi1", ItemId = new Guid()},
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "bi2", ItemId = new Guid()},
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "bi3", ItemId = new Guid()},
            };
        }


        // Should return newly created basket with correct properties
        [Fact]
        public async Task CreateBasket()
        {
            // Arrange
            Guid testUserUuid = Guid.NewGuid();
            string name = "Test Basket";
            bool isDefault = false;
            var expectedStatus = BasketStatus.Active;

            // Act 
            Basket createdBasket = await _basketService.CreateBasketAsync(testUserUuid, name, isDefault);

            // Assert
            Assert.NotNull(createdBasket);
            Assert.Equal(name, createdBasket.Name);
            Assert.Equal(isDefault, createdBasket.IsDefault);
            Assert.Equal(testUserUuid, createdBasket.UserUuid);
            Assert.Equal(expectedStatus, createdBasket.Status);
            Assert.Empty(createdBasket.Items);
        }

        // Should rename the basket name and return true
        [Fact]
        public async Task RenameBasket()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;
            string newName = "NewBasketName";

            // Act 
            bool success = await _basketService.RenameBasketAsync(basket.Id, newName);

            // Assert
            Assert.True(success);
            Assert.Equal(newName, basket.Name);
            Assert.True(basket.UpdatedAt >= startTime);
        }

        [Fact]
        // Should return true if basket is successfully deleted
        public async Task DeleteBasket()
        {
            // Arrange 
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // Act
            bool success = await _basketService.DeleteBasketAsync(basket.Id);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Deleted, basket.Status);
            Assert.True(basket.UpdatedAt >= startTime);
            Assert.True(basket.DeletedAt >= startTime);
        }

        [Fact]
        // Should return true if BasketStatus is successfully restored (e.g: from any status, to Active)
        public async Task RestoreBasket()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // Act 
            bool success = await _basketService.RestoreBasketAsync(basket.Id);

            // Assert
            Assert.True(success);
            Assert.NotNull(basket);
            Assert.True(basket.UpdatedAt >= startTime);
        }

        [Fact]
        // Should return true if BasketStatus is successfully archived (e.g: from any state, to Archived)
        public async Task ArchiveBasket()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // Act 
            bool success = await _basketService.ArchiveBasketAsync(basket.Id);

            // Assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Archived, basket.Status);
            Assert.True(basket.ArchivedAt >= startTime);
            Assert.True(basket.UpdatedAt >= startTime);

        }

        [Fact]
        // Should return true if basket is successfully cleared of all items
        public async Task ClearBasketItems()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // Act 
            bool success = await _basketService.ClearBasketAsync(basket.Id);

            // Assert
            Assert.Empty(basket.Items);
            Assert.True(basket.UpdatedAt >= startTime);
        }

        [Fact]
        // Should return true if item is successfully added to basket
        public async Task AddItemToBasket()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;
            BasketItem basketItem = new BasketItem();
            basketItem.Source = "bi4";
            int expectedItems = 1;

            // Act 
            basket.Items = [];
            bool success = await _basketService.AddItemAsync(basket.Id, basketItem.ItemId, basketItem.Source);

            // Assert
            Assert.True(basket.Items.Count == expectedItems);
            Assert.True(basket.UpdatedAt >= startTime);
            Assert.Equal(basket.Items.First().ItemId, basketItem.ItemId);
            Assert.Equal(basket.Items.First().Source, basketItem.Source);
            Assert.True(basket.Items.First().AddedAt >= startTime);
        }

        [Fact]
        // Should return true if item is successfully removed from basket
        public async Task RemoveItemFromBasket()
        {
            // Arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;
            Guid basketItemId = basket.Items[0].ItemId;
            int expectedItems = 2;

            // Act 
            bool success = await _basketService.RemoveItemAsync(basket.Id, basketItemId);

            // Assert
            Assert.True(basket.Items.Count == expectedItems);
            Assert.True(basket.UpdatedAt >= startTime);
        }
    }
}
