using Basket.API.Entities;


namespace Basket.API.Data.Interfaces
{
    public interface IBasketSeed
    {
        static ShoppingCart  Basket { get; set; }
    }
}
