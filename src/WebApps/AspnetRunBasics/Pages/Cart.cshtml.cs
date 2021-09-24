using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspnetRunBasics.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AspnetRunBasics
{
    [Authorize]
    public class CartModel : PageModel
    {
        private readonly IBasketService _basketService;

        public CartModel(IBasketService basketService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            //ToDo: Get User from Claims
            var userName = "swn";
            Cart = await _basketService.GetBasket(userName);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            //ToDo: Get User from Claims
            var userName = "swn";
            var basket = await _basketService.GetBasket(userName);

            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage();
        }
    }
}