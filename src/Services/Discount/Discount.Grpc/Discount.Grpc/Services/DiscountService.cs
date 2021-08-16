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
        private readonly IMapper _mapper;   // cupon is returned from the Discount.API repository we have to map that to cuponModel in the discount.proto file
                                            // these are nearly identical less capitolization...
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
