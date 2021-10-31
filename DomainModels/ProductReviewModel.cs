using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductReviews.DomainModels
{
    public class ProductReviewModel
    {
        [Key]
        public int ReviewID { get; set; }
        [Required]
        public string ReviewHeader { get; set; }
        [Required]
        public string ReviewContent { get; set; }
        [Required]
        public DateTime ReviewDate { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public bool ReviewIsHidden { get; set; }
    }
}
