using AutoMapper;
using ProductReviews.DomainModels;
using ProductReviews.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Profiles
{
    public class ProductReviewsProfile : Profile
    {
        public ProductReviewsProfile()
        {
            CreateMap<ProductReviewModel, ProductReviewReadDTO>();
        }
    }
}
