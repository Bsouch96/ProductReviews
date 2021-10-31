using AutoMapper;
using ProductReviews.DomainModels;
using ProductReviews.DTOs;

namespace ProductReviews.Profiles
{
    public class ProductReviewsProfile : Profile
    {
        public ProductReviewsProfile()
        {
            CreateMap<ProductReviewModel, ProductReviewReadDTO>();
            CreateMap<ProductReviewCreateDTO, ProductReviewModel>();
        }
    }
}
