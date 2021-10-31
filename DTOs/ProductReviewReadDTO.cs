using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.DTOs
{
    public class ProductReviewReadDTO
    {
        public int ProductReviewID { get; set; }
        public string ProductReviewHeader { get; set; }
        public string ProductReviewContent { get; set; }
        public DateTime ProductReviewDate { get; set; }
    }
}
