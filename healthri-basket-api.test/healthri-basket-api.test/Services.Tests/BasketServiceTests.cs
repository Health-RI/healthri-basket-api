using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Services;
using Moq;

/*

namespace healthri_basket_api.test.Services.Tests
{
    public class BasketServiceTests 
    {
        private readonly ItemService _itemService;
        private readonly BasketService _basketService;
        
        public BasketServiceTests()
        {
            var basketRepository = new Mock<IBasketRepository>();
            var itemRepository = new Mock<IItemRepository>();
            var logger = new Mock<ITransactionLogger>();

            _itemService = new ItemService(itemRepository.Object); 
            _basketService = new BasketService(basketRepository.Object, _itemService, logger.Object);
        }

        private Basket CreateDefaultBasket()
        {
            List<Item> items = CreateDefaultBasketItems();

            Guid userId = Guid.NewGuid();
            Basket basket = new Basket(userId, "DefaultBasketName", true);
            

            // Mock the repository to return this basket when queried by its ID
            _basketService.Setup(r => r.GetByIdAsync(basket.Id)).ReturnsAsync(basket);

            return basket;
        }

        private List<Item> CreateDefaultBasketItems()
        {
            return new List<Item>
            {
                new Item("item 1", "description 1"),
                new Item("item 2", "description 2"),
                new Item("item 2", "description 3"),
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
            Assert.Equal(testUserUuid, createdBasket.UserId);
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
            bool success = await _basketService.AddItemAsync(basket.Id, basketItem.Id, basketItem.Source);

            // Assert
            Assert.True(basket.Items.Count == expectedItems);
            Assert.True(basket.UpdatedAt >= startTime);
            Assert.Equal(basket.Items.First().Id, basketItem.Id);
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
            Guid basketItemId = basket.Items[0].Id;
            int expectedItems = 2;

            // Act 
            bool success = await _basketService.RemoveItemFromBasketAsync(basket.Id, basketItemId);

            // Assert
            Assert.True(basket.Items.Count == expectedItems);
            Assert.True(basket.UpdatedAt >= startTime);
        }
    }
}
*/