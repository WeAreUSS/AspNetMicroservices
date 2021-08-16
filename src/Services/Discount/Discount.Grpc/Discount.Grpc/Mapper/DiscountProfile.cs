using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;

namespace Discount.Grpc.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            // ReverseMap because we would like to be able to map bidirectional: Coupon -> CouponModel AND Coupon <- CouponModel
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
