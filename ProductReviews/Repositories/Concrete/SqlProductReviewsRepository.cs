using Microsoft.EntityFrameworkCore;
using ProductReviews.CustomExceptionMiddleware;
using ProductReviews.DomainModels;
using ProductReviews.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Repositories.Concrete
{
    public class SqlProductReviewsRepository : IProductReviewsRepository
    {
        private readonly Context.DbContext _context;
        public SqlProductReviewsRepository(Context.DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously get all product reviews.
        /// </summary>
        /// <returns>A list of Product Review Models representing reviews.</returns>
        public async Task<List<ProductReviewModel>> GetAllProductReviewsAsync()
        {
            return await _context._productReviews.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Asynchronously get all visible product reviews for a specified product.
        /// </summary>
        /// <param name="ID">The ID of the product to get reviews for.</param>
        /// <returns>A list of Product Review Models for the specified product.</returns>
        public async Task<List<ProductReviewModel>> GetAllVisibleProductReviewsForProductAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            return await _context._productReviews.AsNoTracking().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToListAsync();
        }

        /// <summary>
        /// Asynchronously get the specified product review.
        /// </summary>
        /// <param name="ID">The ID that represents the required prodcut review.</param>
        /// <returns>A Product Review Model representing the data of a product review.</returns>
        public async Task<ProductReviewModel> GetProductReviewAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            return await _context._productReviews.FirstOrDefaultAsync(d => d.ProductReviewID == ID) ?? throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist."); ;
        }

        /// <summary>
        /// Create a product review for a customer.
        /// </summary>
        /// <param name="productReviewModel">The object that contains parameters used to create an product review.</param>
        /// <returns>The newly created Product Review Model with a product review ID.</returns>
        public ProductReviewModel CreateProductReview(ProductReviewModel productReviewModel)
        {
            if(productReviewModel == null)
                throw new ArgumentNullException(nameof(productReviewModel), "The product review used to update cannot be null.");

            return _context._productReviews.Add(productReviewModel).Entity;
        }

        /// <summary>
        /// Update an existing product review with new parameters.
        /// </summary>
        /// <param name="productReviewModel">The object used to update en existing product review.</param>
        public void UpdateProductReview(ProductReviewModel productReviewModel)
        {
            if(productReviewModel == null)
                throw new ArgumentNullException(nameof(productReviewModel), "The product review used to update cannot be null.");

            _context._productReviews.Update(productReviewModel);
        }

        /// <summary>
        /// Save all currently tracked changes.
        /// </summary>
        /// <returns>A completed Task.</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
