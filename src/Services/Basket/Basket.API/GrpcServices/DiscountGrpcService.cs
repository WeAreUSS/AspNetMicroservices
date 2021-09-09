using Discount.Grpc.Protos;
using System;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    // used to indirectly access GRPC Services
    // NOTE: As this is the client, we do not inherit from the base class of the service as we did on the server side of this client/server system...
    //========================================
    public class DiscountGrpcService
    {
        // notice we are using the "DiscountProtoServiceClient" as we are the client here and only the client side is exposed...
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;
        
        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };
            // call proto service and get discounts associated with the productName
            return await _discountProtoService.GetDiscountAsync(discountRequest);
        }
    }
}
