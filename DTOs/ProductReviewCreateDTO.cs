using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.DTOs
{
    public class ProductReviewCreateDTO
    {
        [Required]
        public string ProductReviewHeader { get; set; }
        [Required]
        public string ProductReviewContent { get; set; }
        [Required]
        public DateTime ProductReviewDate { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public bool ProductReviewIsHidden { get; set; }
    }
}
