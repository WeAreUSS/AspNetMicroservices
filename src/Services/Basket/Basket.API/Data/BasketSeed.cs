using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

using System.Collections.Generic;
using System.Linq;
using Basket.API.Data.Interfaces;

namespace Basket.API.Data
{
    public class BasketSeed : IBasketSeed
    {
		 private readonly IDistributedCache _redisCache;
         
        // Basket Property
         public static ShoppingCart Basket { get; set; }  
		 
        public BasketSeed(IDistributedCache cache)
        {
            GetPreconfiguredShoppingCart("swn");
        } 

        private static void GetPreconfiguredShoppingCart(string name)
        {
            //Basket = new ShoppingCart();
			ShoppingCart newCart = new ShoppingCart(name);

            ShoppingCartItem thisItem = new ShoppingCartItem();

            thisItem.Quantity = 1;
            thisItem.Color = "Red";
            thisItem.Price = (decimal)1.00;
            thisItem.ProductId = "";
            thisItem.ProductName = "";
            newCart.Items.Add(thisItem);

            thisItem.Quantity = 1;
            thisItem.Color = "Blue";
            thisItem.Price = (decimal)2.00;
            thisItem.ProductId = "";
            thisItem.ProductName = "";
            newCart.Items.Add(thisItem);

            Basket = newCart;


        }
    }
}
