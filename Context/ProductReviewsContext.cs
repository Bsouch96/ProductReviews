using Microsoft.EntityFrameworkCore;
using ProductReviews.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Context
{
    public class ProductReviewsContext : DbContext
    {
        private DbSet<ProductReviewModel> _productReviewsContext { get; set; }
        public ProductReviewsContext(DbContextOptions<ProductReviewsContext> options) : base(options)
        {

        }
    }
}
