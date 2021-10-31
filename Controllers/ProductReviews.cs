using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProductReviews.DomainModels;
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

        [HttpPost]
        public async Task<ActionResult> CreateProductReview([FromBody] ProductReviewCreateDTO productReviewCreateDTO)
        {
            if (productReviewCreateDTO == null)
                return BadRequest();

            var productReviewModel = _mapper.Map<ProductReviewModel>(productReviewCreateDTO);
            productReviewModel.ProductReviewDate = System.DateTime.Now;
            productReviewModel.ProductReviewIsHidden = false;

            int newProductReviewID = _productReviewsRepository.CreateProductReview(productReviewModel);
            await _productReviewsRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductReview), new { ID = newProductReviewID });
        }

        [HttpPatch("{ID}")]
        public async Task<ActionResult> UpdateProductReview(int ID, JsonPatchDocument<ProductReviewUpdateDTO> productReviewUpdatePatch)
        {
            var productReviewModel = await _productReviewsRepository.GetProductReviewAsync(ID);
            if (productReviewModel == null)
                return NotFound();

            var newproductReviewRequest = _mapper.Map<ProductReviewUpdateDTO>(productReviewModel);
            productReviewUpdatePatch.ApplyTo(newproductReviewRequest, ModelState);

            if (!TryValidateModel(newproductReviewRequest))
                return ValidationProblem(ModelState);

            _mapper.Map(newproductReviewRequest, productReviewModel);

            _productReviewsRepository.UpdateProductReview(productReviewModel);
            await _productReviewsRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
