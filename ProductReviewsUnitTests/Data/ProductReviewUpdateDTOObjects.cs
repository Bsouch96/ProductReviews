using System;
using System.Collections.Generic;
using System.Text;

namespace ProductReviewsUnitTests.Data
{
    /// <summary>
    /// Class to serve as a data provider for Unit Tests.
    /// </summary>
    public class ProductReviewUpdateDTOObjects
    {
        public ProductReviewUpdateDTOObjects(){}

        /// <summary>
        /// Used to provide a list of test objects.
        /// Array Args 1: ProductReviewID - Any Int32 > 0
        /// Array Args 2: ProductReviewIsHidden - bool
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetProductReviewUpdateDTOObjects()
        {
            return new List<Object[]>
            {
                new object[] { 1, true },
                new object[] { 2, true },
                new object[] { 3, true },
                new object[] { 4, true },
                new object[] { 5, true }
            };
        }
    }
}
