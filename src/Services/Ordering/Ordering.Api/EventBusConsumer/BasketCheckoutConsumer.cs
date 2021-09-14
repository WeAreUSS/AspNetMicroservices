using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using System;
using System.Threading.Tasks;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent> // IConsumer in MassTransit - configured in StartUp.cs
    {
        private readonly IMediator _mediator; // make sure you do not use the MassTransit version of this as we want to call MediatR here to kick off our command
        private readonly IMapper _mapper;
        private readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger<BasketCheckoutConsumer> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // this is the required handler for MassTransit.IConsumer<>
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            // Ordering.API.Mapper.OrderingProfile: From: BasketCheckoutEvent (Model) to CheckoutOrderCommand (Model) as CheckOutOrderCommandHandler expects such
            var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
            // when we call the CheckoutOrderCommand Model, it kicks off the CheckOutOrderCommandHandler because of inheritance from IRequest<int> _mediator functionality
            var result = await _mediator.Send(command);

            _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id : {newOrderId}", result);
        }
    }
}
