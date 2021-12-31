using AutoMapper;
using ProductReviews.DomainModels;
using ProductReviews.DTOs;

namespace ProductReviews.Profiles
{
    /// <summary>
    /// This class is used to map all objects for AutoMapper's reference.
    /// </summary>
    public class ProductReviewsProfile : Profile
    {
        public ProductReviewsProfile()
        {
            CreateMap<ProductReviewModel, ProductReviewReadDTO>();
            CreateMap<ProductReviewCreateDTO, ProductReviewModel>();
            CreateMap<ProductReviewModel, ProductReviewUpdateDTO>();
            CreateMap<ProductReviewUpdateDTO, ProductReviewModel>();
        }
    }
}
