using Microsoft.EntityFrameworkCore;
using ProductReviews.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Context
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public virtual DbSet<ProductReviewModel> _productReviews { get; set; }
        public DbContext(DbContextOptions<DbContext> options) : base(options){}
        
        public DbContext() : base(){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductReviewModel>()
                .HasData(
                new ProductReviewModel { ProductReviewID = 1, ProductReviewHeader = "Wow!", ProductReviewContent = "Lovely Shoes.", ProductReviewDate = System.DateTime.Now, ProductID = 1, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 2, ProductReviewHeader = "Amazing!", ProductReviewContent = "Best shirt since sliced bread.", ProductReviewDate = System.DateTime.Now, ProductID = 1, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 3, ProductReviewHeader = "Terrible!", ProductReviewContent = "Did not receive order...", ProductReviewDate = System.DateTime.Now, ProductID = 2, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 4, ProductReviewHeader = "Lovely Jubbly!", ProductReviewContent = "Great Service.", ProductReviewDate = System.DateTime.Now, ProductID = 2, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 5, ProductReviewHeader = "WrongSize!!!!", ProductReviewContent = "I wanted a schmedium but I received a Large. I'm so mad.", ProductReviewDate = System.DateTime.Now, ProductID = 3, ProductReviewIsHidden = false }
            );
        }
    }
}
