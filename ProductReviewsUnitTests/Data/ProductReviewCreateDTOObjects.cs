using System;
using System.Collections.Generic;
using System.Text;

namespace ProductReviewsUnitTests.Data
{
    /// <summary>
    /// Class to serve as a data provider for Unit Tests.
    /// </summary>
    public class ProductReviewCreateDTOObjects
    {
        public ProductReviewCreateDTOObjects(){}

        /// <summary>
        /// Used to provide a list of test objects.
        /// </summary>
        /// <returns>
        /// Array Args 1: ProductReviewHeader - string
        /// Array Args 2: ProductReviewContent - string
        /// Array Args 3: ProductReviewDate - DateTime
        /// Array Args 3: ProductID - Any Int32 > 0
        /// Array Args 3: ProductReviewIsHidden - bool
        /// </returns>
        public static IEnumerable<object[]> GetProductReviewCreateDTOObjects()
        {
            return new List<Object[]>
            {
                new object[] { "", "TEST Deleting my account.", new System.DateTime(2010, 10, 01, 8, 5, 3), 2, false },
                new object[] { "", "TEST Horrendous Store.", new System.DateTime(2012, 01, 02, 10, 3, 45), 3, false },
                new object[] { "", "TEST Prefer brick over click.", new System.DateTime(2013, 02, 03, 12, 2, 40), 4, false },
                new object[] { "", "TEST Too buggy.", new System.DateTime(2014, 03, 04, 14, 1, 35), 4, false },
                new object[] { "", "TEST Just found Wish.", new System.DateTime(2007, 04, 05, 16, 50, 30), 5, false }
            };
        }
    }
}
