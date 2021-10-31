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
                return null;

            return await Task.FromResult(_productReviews.FirstOrDefault(d => d.ProductReviewID == ID));
        }
        
        public async Task<IEnumerable<ProductReviewModel>> GetAllProductReviewsAsync()
        {
            return await Task.FromResult(_productReviews.AsEnumerable());
        }

        public void CreateProductReviewAsync(ProductReviewModel productReviewModel)
        {
            int productReviewID = (_productReviews.Count + 1);
            productReviewModel.ProductReviewID = productReviewID;

            _productReviews.Add(productReviewModel);
        }

        public void UpdateProductReview(ProductReviewModel productReviewModel)
        {
            //EF tracks the changes of updates. It pushes them to the DB when SaveChangesAsync() has been called.
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
