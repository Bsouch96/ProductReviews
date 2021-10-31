using Microsoft.EntityFrameworkCore;
using ProductReviews.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.Context
{
    public class Context : DbContext
    {
        public DbSet<ProductReviewModel> _productReviews { get; set; }
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
    }
}
