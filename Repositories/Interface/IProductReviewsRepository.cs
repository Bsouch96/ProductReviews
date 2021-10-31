using ProductReviews.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Repositories.Interface
{
    public interface IProductReviewsRepository
    {
        public Task<IEnumerable<ProductReviewModel>> GetAllProductReviewsAsync();
        public Task<ProductReviewModel> GetProductReviewAsync(int ID);
        public Task<ProductReviewModel> CreateProductReviewAsync(ProductReviewModel productReviewModel);
        public Task<ProductReviewModel> UpdateProductReview(ProductReviewModel productReviewModel);
        public Task SaveChangesAsync();
    }
}
