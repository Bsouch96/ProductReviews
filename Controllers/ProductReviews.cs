using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductReviews.DTOs;
using ProductReviews.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Controllers
{
    [Route("api/ProductReviews")]
    [ApiController]
    public class ProductReviews : ControllerBase
    {
        public IProductReviewsRepository _productReviewsRepository;
        private readonly IMapper _mapper;

        public ProductReviews(IProductReviewsRepository productReviewsRepository,
            IMapper mapper)
        {
            _productReviewsRepository = productReviewsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReviewReadDTO>>> GetAllProductReviews()
        {
            var productReviews = await _productReviewsRepository.GetAllProductReviewsAsync();

            return Ok(_mapper.Map<IEnumerable<ProductReviewReadDTO>>(productReviews));
        }

        [HttpGet("{ID}")]
        public async Task<ActionResult<ProductReviewReadDTO>> GetProductReview(int ID)
        {
            var productReview = await _productReviewsRepository.GetProductReviewAsync(ID);

            if (productReview != null)
                return Ok(_mapper.Map<ProductReviewReadDTO>(productReview));

            return NotFound();
        }
    }
}
