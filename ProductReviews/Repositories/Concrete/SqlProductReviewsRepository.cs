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

        public async Task<List<ProductReviewModel>> GetAllProductReviewsAsync()
        {
            return await _context._productReviews.AsNoTracking().ToListAsync();
        }

        public async Task<List<ProductReviewModel>> GetAllVisibleProductReviewsForProductAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            return await _context._productReviews.AsNoTracking().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToListAsync();
        }

        public async Task<ProductReviewModel> GetProductReviewAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            return await _context._productReviews.FirstOrDefaultAsync(d => d.ProductReviewID == ID) ?? throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist."); ;
        }

        public ProductReviewModel CreateProductReview(ProductReviewModel productReviewModel)
        {
            if(productReviewModel == null)
                throw new ArgumentNullException(nameof(productReviewModel), "The product review used to update cannot be null.");

            return _context._productReviews.Add(productReviewModel).Entity;
        }

        public void UpdateProductReview(ProductReviewModel productReviewModel)
        {
            if(productReviewModel == null)
                throw new ArgumentNullException(nameof(productReviewModel), "The product review used to update cannot be null.");

            _context._productReviews.Update(productReviewModel);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
