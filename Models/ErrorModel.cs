using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProductReviews.Models
{
    public class ErrorModel
    {
        [Required]
        public int StatusCode { get; set; }
        [Required]
        public string ErrorMessage { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
