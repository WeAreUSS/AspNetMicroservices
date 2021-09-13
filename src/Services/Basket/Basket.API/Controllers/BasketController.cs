using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices; // enabled after DiscountGrpcService.cs was developed under Grpc folder
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService; // client
        private readonly IPublishEndpoint _publishEndpoint; // publishing to RabbitMQ via MassTransit
        private readonly IMapper _mapper;

        // First implementation - Basket repository only
        //public BasketController(IBasketRepository repository)
        //{
        //    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        //}

        // Second implementation - After DiscountGrpcService.cs was developed
        //public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService,  IMapper mapper)
        //{
        //    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        //    _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        //    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        //}

        // Final implementation
        // ======================
        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            // ?? - means that if it is the first time user is getting a basket, we want to provide a new basket for the user
            // else, the preexisting repository basket is returned - as the repository is using Redis, the data comes from our cache
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // Communicate with Discount.Grpc, an injected dependency, and calculate latest prices of products into ShoppingCart ~"Basket"
            foreach (var item in basket.Items)
            {
                // NOTE: as we are indirectly acting upon the return from gprc, we are acting upon CouponModel, not Coupon at this time...
               var coupon = await _discountGrpcService.GetDiscount(item.ProductName); // enabled after DiscountGrpcService.cs was developed; used to indirectly access the grpc service
               // reducing price of each item by discount provided through coupon for said
               item.Price -= coupon.Amount; // enabled after DiscountGrpcService.cs was developed
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]        
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // IMPLEMENTATION of MassTransit & RabbitMQ Message Publishing
            //=============================================================
            // ToDoDone
            // get existing basket with total price            
            // Set TotalPrice on basketCheckout eventMessage
            // send checkout event to RabbitMQ
            // remove the basket

            // get existing basket with total price - returns ShoppingCart Object
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
            {
                return BadRequest();
            }

            // ToDoDone
            // send checkout event to RabbitMQ & MassTransit
            //==============================================
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout); // convert this ShoppingCart Object to a BasketCheckoutEvent Object
            eventMessage.TotalPrice = basket.TotalPrice;
            // Publish msg to MassTransit buss
            await _publishEndpoint.Publish<BasketCheckoutEvent>(eventMessage);

            // remove the basket because we do not use it after sending it off to the EventBus
            await _repository.DeleteBasket(basket.UserName);

            return Accepted();
        }
    }
}
