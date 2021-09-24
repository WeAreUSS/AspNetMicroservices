using System;
using System.Collections.Generic;
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
    public class CheckOutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public CheckOutModel(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [BindProperty]
        public BasketCheckoutModel Order { get; set; }

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

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            //ToDo: Get User from Claims
            var userName = "swn";
            Cart = await _basketService.GetBasket(userName);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = userName;
            Order.TotalPrice = Cart.TotalPrice;

            await _basketService.CheckoutBasket(Order);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}