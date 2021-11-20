using ProductReviews.DomainModels;
using ProductReviews.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Repositories.Concrete
{
    public class FakeProductReviewsRepository : IProductReviewsRepository
    {
        public List<ProductReviewModel> _productReviews = new List<ProductReviewModel>
        {
            new ProductReviewModel { ProductReviewID = 1, ProductReviewHeader = "Wow!", ProductReviewContent = "Lovely Shoes.", ProductReviewDate = System.DateTime.Now, ProductID = 1, ProductReviewIsHidden = false },
            new ProductReviewModel { ProductReviewID = 2, ProductReviewHeader = "Amazing!", ProductReviewContent = "Best shirt since sliced bread.", ProductReviewDate = System.DateTime.Now, ProductID = 1, ProductReviewIsHidden = false },
            new ProductReviewModel { ProductReviewID = 3, ProductReviewHeader = "Terrible!", ProductReviewContent = "Did not receive order...", ProductReviewDate = System.DateTime.Now, ProductID = 2, ProductReviewIsHidden = false },
            new ProductReviewModel { ProductReviewID = 4, ProductReviewHeader = "Lovely Jubbly!", ProductReviewContent = "Great Service.", ProductReviewDate = System.DateTime.Now, ProductID = 2, ProductReviewIsHidden = false },
            new ProductReviewModel { ProductReviewID = 5, ProductReviewHeader = "WrongSize!!!!", ProductReviewContent = "I wanted a schmedium but I received a Large. I'm so mad.", ProductReviewDate = System.DateTime.Now, ProductID = 3, ProductReviewIsHidden = false }
        };

        public FakeProductReviewsRepository()
        {

        }

        public async Task<ProductReviewModel> GetProductReviewAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException("IDs cannot be less than 0.", nameof(ArgumentOutOfRangeException));

            ProductReviewModel productReviewModel = _productReviews.FirstOrDefault(d => d.ProductReviewID == ID);

            if (productReviewModel == null)
                throw new ArgumentNullException("The product review used to update cannot be null.", nameof(ArgumentNullException));

            ProductReviewModel returnableProductReviewModel = new ProductReviewModel()
            {
                ProductID = productReviewModel.ProductID,
                ProductReviewID = productReviewModel.ProductReviewID,
                ProductReviewContent = productReviewModel.ProductReviewContent,
                ProductReviewDate = productReviewModel.ProductReviewDate,
                ProductReviewHeader = productReviewModel.ProductReviewHeader,
                ProductReviewIsHidden = productReviewModel.ProductReviewIsHidden
            };

            return await Task.FromResult(returnableProductReviewModel);
        }
        
        public async Task<List<ProductReviewModel>> GetAllProductReviewsAsync()
        {
            return await Task.FromResult(new List<ProductReviewModel>(_productReviews));
        }

        public async Task<List<ProductReviewModel>> GetAllVisibleProductReviewsAsync()
        {
            return await Task.FromResult(new List<ProductReviewModel>(_productReviews.Where(pr => !pr.ProductReviewIsHidden)));
        }

        public ProductReviewModel CreateProductReview(ProductReviewModel productReviewModel)
        {
            int productReviewID = (_productReviews.Count + 1);
            productReviewModel.ProductReviewID = productReviewID;

            _productReviews.Add(productReviewModel);

            return productReviewModel;
        }

        /// <summary>
        /// This function fakes an update occuring in the Database. It removes the existing and replaces it with the new, updated entity.
        /// </summary>
        /// <param name="productReviewModel">The new, updated entity.</param>
        public void UpdateProductReview(ProductReviewModel productReviewModel)
        {
            if(productReviewModel == null)
                throw new ArgumentNullException("The product review used to update cannot be null.", nameof(ArgumentNullException));

            var productReviewModelOld = _productReviews.FirstOrDefault(r => r.ProductReviewID == productReviewModel.ProductReviewID);
            _productReviews.Remove(productReviewModelOld);
            _productReviews.Add(productReviewModel);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
