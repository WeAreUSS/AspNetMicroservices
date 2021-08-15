namespace Basket.API.Entities
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string ProductId { get; set; } // same as catalogId in Catalog.API\Data\CatalogContextSeed Product.Id
        public string ProductName { get; set; }
    }
}
