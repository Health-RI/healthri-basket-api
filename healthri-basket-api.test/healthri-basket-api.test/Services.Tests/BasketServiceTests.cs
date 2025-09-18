using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Services;
using Moq;

// TODO: Add repository reset
// TODO: AddItem
// TODO: RemoveItem
// TODO: ClearItems

namespace healthri_basket_api.test.Services.Tests
{
    public class BasketServiceTests 
    {
        private readonly Mock<IBasketRepository> basketRepository;
        private readonly Mock<ITransactionLogger> logger;
        private readonly BasketService basketService;
        
        public BasketServiceTests()
        {
            basketRepository = new Mock<IBasketRepository>();
            logger = new Mock<ITransactionLogger>();
            basketService = new BasketService(basketRepository.Object, logger.Object);
        }

        private Basket CreateDefaultBasket()
        {
            List<BasketItem> basketItems = CreateDefaultBasketItems();

            return new Basket
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
        }

        private List<BasketItem> CreateDefaultBasketItems()
        {
            return new List<BasketItem>
            {
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "tp1", ItemId = "111"},
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "tp2", ItemId = "222"},
                new BasketItem { AddedAt = DateTime.UtcNow, Source = "tp3", ItemId = "333"},
            };
        }


        // Should return newly created basket with correct properties /done
        [Fact]
        public async Task CreateBasketAsyncTest()
        {
            // arrange
            Guid testUserUuid = Guid.NewGuid();
            string name = "Test Basket";
            bool isDefault = false;

            // act 
            Basket createdBasket = await basketService.CreateBasketAsync(testUserUuid, name, isDefault);

            // assert
            Assert.NotNull(createdBasket);
            Assert.Equal(name, createdBasket.Name);
            Assert.Equal(isDefault, createdBasket.IsDefault);
            Assert.Equal(testUserUuid, createdBasket.UserUuid);
            Assert.Equal(BasketStatus.Active, createdBasket.Status);
            Assert.Empty(createdBasket.Items);
        }

        // Should rename the basket and return true /done
        [Fact]
        public async Task RenameBasketAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;
            string newName = "NewBasketName";

            // act 
            bool success = await basketService.RenameBasketAsync(basket.Id, newName);

            // assert
            Assert.True(success);
            Assert.Equal(newName, basket.Name);
            Assert.True(basket.UpdatedAt >= startTime);
        }

        [Fact]
        // Should return true if basket is successfully deleted /done
        public async Task DeleteBasketAsync()
        {
            // arrange 
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // act
            bool success = await basketService.DeleteBasketAsync(basket.Id);

            // assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Deleted, basket.Status);
            Assert.True(basket.UpdatedAt >= startTime);
            Assert.True(basket.DeletedAt >= startTime);
        }

        [Fact]
        // Should return true if BasketStatus is successfully restored (e.g: from any status, to Active) / done
        public async Task RestoreBasketAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // act 
            bool success = await basketService.RestoreBasketAsync(basket.Id);

            // assert
            Assert.True(success);

            Assert.NotNull(basket);
            Assert.True(basket.UpdatedAt >= startTime);

        }

        [Fact]
        // Should return true if BasketStatus is successfully archived (e.g: from any state, to Archived) / done
        public async Task ArchiveBasketAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            // act 
            bool success = await basketService.ArchiveBasketAsync(basket.Id);

            // assert
            Assert.True(success);
            Assert.Equal(BasketStatus.Archived, basket.Status);
            Assert.True(basket.ArchivedAt >= startTime);
            Assert.True(basket.UpdatedAt >= startTime);

        }

        [Fact]
        // Should return true if basket is successfully cleared of all items / done
        public async Task ClearBasketAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            var testProduct = new BasketItem();

            // act 
            bool success = await basketService.ClearBasketAsync(basket.Id);

            // assert
            Assert.Empty(basket.Items);
            Assert.True(basket.UpdatedAt >= startTime);

        }

        [Fact]
        // Should return true if item is successfully added to basket
        public async Task AddItemAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;

            BasketItem basketItem = new BasketItem();
            basketItem.ItemId = "444";
            basketItem.Source = "tp4";

            // act 
            basket.Items = [];
            bool success = await basketService.AddItemAsync(basket.Id, basketItem.ItemId, basketItem.Source);

            // assert
            Assert.True(basket.Items.Count == 1);
            Assert.True(basket.UpdatedAt >= startTime);

            Assert.Equal(basket.Items[0].ItemId, basketItem.ItemId);
            Assert.Equal(basket.Items[0].Source, basketItem.Source);
            Assert.Equal(basket.Items[0].AddedAt, basketItem.AddedAt);
        }

        [Fact]
        // Should return true if item is successfully removed from basket
        public async Task RemoveItemAsync()
        {
            // arrange
            Basket basket = CreateDefaultBasket();
            DateTime startTime = DateTime.UtcNow;
            string basketItemId = "333";

            // act 
            bool success = await basketService.RemoveItemAsync(basket.Id, basketItemId);

            // assert
            Assert.True(basket.Items.Count == 2);
            Assert.True(basket.UpdatedAt >= startTime);
        }
    }
}
