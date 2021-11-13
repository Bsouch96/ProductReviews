﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        public readonly IProductReviewsRepository _productReviewsRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ProductReviews(IProductReviewsRepository productReviewsRepository, IMapper mapper, IMemoryCache memoryCache)
        {
            _productReviewsRepository = productReviewsRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReviewReadDTO>>> GetAllProductReviews()
        {
            if (_memoryCache.TryGetValue("ProductReviews", out List<ProductReviewModel> productReviewValues))
                return Ok(_mapper.Map<IEnumerable<ProductReviewModel>>(productReviewValues));

            var productReviews = await _productReviewsRepository.GetAllProductReviewsAsync();
            _memoryCache.Set("ProductReviews", productReviews, GetMemoryCacheEntryOptions());
            return Ok(_mapper.Map<IEnumerable<ProductReviewReadDTO>>(productReviews));
        }

        [Route("Visible")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReviewReadDTO>>> GetAllVisibleProductReviews()
        {
            if (_memoryCache.TryGetValue("ProductReviews", out List<ProductReviewModel> productReviewValues))
                return Ok(_mapper.Map<IEnumerable<ProductReviewReadDTO>>(productReviewValues.Where(pr => !pr.ProductReviewIsHidden)));

            var productReviews = await _productReviewsRepository.GetAllProductReviewsAsync();
            _memoryCache.Set("ProductReviews", productReviews, GetMemoryCacheEntryOptions());
            return Ok(_mapper.Map<IEnumerable<ProductReviewReadDTO>>(productReviews));
        }

        [HttpGet("{ID}")]
        public async Task<ActionResult<ProductReviewReadDTO>> GetProductReview(int ID)
        {
            ProductReviewModel productReview;
            //If cache exists and we find the entity.
            if (_memoryCache.TryGetValue("ProductReviews", out List<ProductReviewModel> productReviewValues))
            {
                //Return the entity if we find it in the cache.
                productReview = productReviewValues.Find(pr => pr.ProductReviewID == ID);
                if (productReview != null)
                    return Ok(_mapper.Map<ProductReviewReadDTO>(productReview));

                //Otherwise, get the entity from the DB, add it to the cache and return it.
                productReview = await _productReviewsRepository.GetProductReviewAsync(ID);
                if (productReview != null)
                {
                    productReviewValues.Add(productReview);
                    return Ok(_mapper.Map<ProductReviewReadDTO>(productReview));
                }

                return NotFound();
            }

            productReview = await _productReviewsRepository.GetProductReviewAsync(ID);

            if (productReview != null)
                return Ok(_mapper.Map<ProductReviewReadDTO>(productReview));

            return NotFound();
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult> CreateProductReview([FromBody] ProductReviewCreateDTO productReviewCreateDTO)
        {
            if (productReviewCreateDTO == null)
                return BadRequest();

            ProductReviewModel productReviewModel = _mapper.Map<ProductReviewModel>(productReviewCreateDTO);
            productReviewModel.ProductReviewDate = System.DateTime.Now;
            productReviewModel.ProductReviewIsHidden = false;

            ProductReviewModel newProductReviewModel = _productReviewsRepository.CreateProductReview(productReviewModel);
            await _productReviewsRepository.SaveChangesAsync();

            if (_memoryCache.TryGetValue("ProductReviews", out List<ProductReviewModel> productReviewValues))
                productReviewValues.Add(newProductReviewModel);

            ProductReviewReadDTO productReviewReadDTO = _mapper.Map<ProductReviewReadDTO>(newProductReviewModel);

            return CreatedAtAction(nameof(GetProductReview), new { ID = productReviewReadDTO.ProductReviewID }, productReviewReadDTO);
        }

        [Route("Visibility/{ID}")]
        [HttpPatch]
        public async Task<ActionResult> UpdateProductReview(int ID, JsonPatchDocument<ProductReviewUpdateDTO> productReviewUpdatePatch)
        {
            ProductReviewModel productReviewModel = await _productReviewsRepository.GetProductReviewAsync(ID);
            if (productReviewModel == null)
                return NotFound();

            ProductReviewUpdateDTO newproductReviewRequest = _mapper.Map<ProductReviewUpdateDTO>(productReviewModel);
            productReviewUpdatePatch.ApplyTo(newproductReviewRequest, ModelState);

            if (!TryValidateModel(newproductReviewRequest))
                return ValidationProblem(ModelState);

            _mapper.Map(newproductReviewRequest, productReviewModel);

            _productReviewsRepository.UpdateProductReview(productReviewModel);
            await _productReviewsRepository.SaveChangesAsync();

            if (_memoryCache.TryGetValue("ProductReviews", out List<ProductReviewModel> productReviewValues))
            {
                productReviewValues.RemoveAll(pr => pr.ProductReviewID == productReviewModel.ProductReviewID);
                productReviewValues.Add(productReviewModel);
            }

            return NoContent();
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                SlidingExpiration = new TimeSpan(0, 10, 0),
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 20, 0),
                Priority = CacheItemPriority.Normal,
                Size = 1028
            };
        }
    }
}
