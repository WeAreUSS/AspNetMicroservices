using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        // Injection of Redis cache from startup.cs
        public BasketRepository(IDistributedCache cache)
        {
            _redisCache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            // Get basket associated with username from cache (basket is a ShoppingCart object)
            var basket = await _redisCache.GetStringAsync(userName);

            if (String.IsNullOrEmpty(basket))
                return null;            

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }
        
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            // Save username and basket to cache
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            
            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string userName)
        {
            // Remove username and thus basket from cache
            await _redisCache.RemoveAsync(userName);
        }
    }
}
