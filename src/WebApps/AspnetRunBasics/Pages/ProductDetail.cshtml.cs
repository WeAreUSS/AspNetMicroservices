using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AspnetRunBasics
{
    [Authorize]
    public class ProductDetailModel : PageModel
    {
        #region Injection Variables

        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        #endregion Injection Variables

        public ProductDetailModel(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        #region Properties

        public CatalogModel Product { get; set; }

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        #endregion Properties

        #region Methods

        public async Task<IActionResult> OnGetAsync(string productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Product = await _catalogService.GetCatalog(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);
            
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            //ToDo: Get User from Claims
            var userName = "swn";
            var basket = await _basketService.GetBasket(userName);

            basket.Items.Add(new BasketItemModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = Quantity,
                Color = Color
            });

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage("Cart");
        }

        #endregion Methods
    }
}