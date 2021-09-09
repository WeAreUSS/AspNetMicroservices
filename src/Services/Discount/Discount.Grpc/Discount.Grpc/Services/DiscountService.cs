using AutoMapper; // not used in initial version until quick change
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Interfaces;
namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;   // coupon is returned from the Discount.API repository
                                            // we have to map that to cuponModel in the discount.proto file
                                            // these are nearly identical less capitalization...
        private readonly ILogger<DiscountService> _logger;

        // Initial Version - development stopped when we needed to map componentModel in GetDiscount
        // Then, we added AutoMapper....
        // ===========================================================================================
        //public DiscountService(IDiscountRepository repository,  ILogger<DiscountService> logger)
        //{
        //    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        //    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //}

        public DiscountService(IDiscountRepository repository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            /*
             
            Note: Below is the method we had used in our Discount.Api controller,
                     We use the same patterns herein.... 
                     The same is true for all other methods.
                     The only signifigant difference is that we implement:
                     ~ exception handling
                     ~ logging
                     ~ AutoMapper - to map between Coupon and CouponModel in some cases before and after the call...
            CouponModel is the model used for gprc communications Coupon is the model we are using outside of gprc
            So, we know that when we begin to consume this, we will have to AutoMap it the other way on the other side of this call; whoever calls these methods....

            Also, Notice: We are using Requests which are defined in our .proto file
            Finally, "ServerCallContext" is derived from the GRPRC framework, this allows us to connect to GPRC

                Code from  Discount.Api controller
                ===================================
                var discount = await _repository.GetDiscount(productName);
                return Ok(discount);
                ===================================

            */

            // we are only using the CouponModel.ProductName here, so, we have no front end conversion required.
            var coupon = await _repository.GetDiscount(request.ProductName);   // note we are creating Coupon here
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            // conversion from Coupon to CouponModel performed by AutoMapper in Mapper/DiscountProfile.cs
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            // our request is presenting us with a CouponModel 
            // Our CreateDiscount call in Discount.API requires a Coupon entity
            // So, we have to do a conversion CouponModel -> Coupon prior to the call
            var coupon = _mapper.Map<Coupon>(request.Coupon); // note we are creating Coupon here

            await _repository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

            // conversion from Coupon to CouponModel performed by AutoMapper in Mapper/DiscountProfile.cs
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            
            // our request is presenting us with a CouponModel 
            // Our UpdateDiscount call in Discount.API requires a Coupon entity
            // So, we have to do a conversion CouponModel -> Coupon prior to the call
            var coupon = _mapper.Map<Coupon>(request.Coupon);  // note we are creating Coupon here

            await _repository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            // conversion from Coupon to CouponModel performed by AutoMapper in Mapper/DiscountProfile.cs
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            // we are only using the CouponModel.ProductName here, so, we have no front end conversion required.
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
