using System;
using System.Collections.Generic;
using System.Text;

namespace ProductReviewsUnitTests.Data
{
    public class ProductReviewModelObjects
    {
        public ProductReviewModelObjects(){}

        /// <summary>
        /// Used to provide a list of test create objects.
        /// Array Args 1: ProductReviewHeader - string not null
        /// Array Args 2: ProductReviewContent - string not null
        /// Array Args 3: ProductReviewDate - DateTime
        /// Array Args 4: ProductID - Any Int32 > 0
        /// Array Args 5: ProductReviewIsHidden - bool
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetProductReviewModelCreateObjects()
        {
            return new List<Object[]>
            {
                new object[] { "Wow!", "Lovely Shoes.", new System.DateTime(2010, 10, 01, 8, 5, 3), 1, false },
                new object[] { "Amazing!", "Best shirt since sliced bread.", new System.DateTime(2012, 01, 02, 10, 3, 45), 1, false },
                new object[] { "Terrible!", "Did not receive order...", new System.DateTime(2013, 02, 03, 12, 2, 40), 2, false },
                new object[] { "Lovely Jubbly!", "Great Service.", new System.DateTime(2014, 03, 04, 14, 1, 35), 2, false },
                new object[] { "WrongSize!!!!", "I wanted a schmedium but I received a Large. I'm so mad.", new System.DateTime(2007, 04, 05, 16, 50, 30), 3, false }
            };
        }

        /// <summary>
        /// Used to provide a list of test update objects.
        /// Array Args 1: ProductReviewHeader - string not null
        /// Array Args 2: ProductReviewContent - string not null
        /// Array Args 3: ProductReviewDate - DateTime
        /// Array Args 4: ProductID - Any Int32 > 0
        /// Array Args 5: ProductReviewIsHidden - bool
        /// Array Args 6: ProductReviewID - Any Int32 > 0
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetProductReviewModelUpdateObjects()
        {
            return new List<Object[]>
            {
                new object[] { "Wow!", "Lovely Shoes.", new System.DateTime(2010, 10, 01, 8, 5, 3), 1, false, 1 },
                new object[] { "Amazing!", "Best shirt since sliced bread.", new System.DateTime(2012, 01, 02, 10, 3, 45), 1, false, 2 },
                new object[] { "Terrible!", "Did not receive order...", new System.DateTime(2013, 02, 03, 12, 2, 40), 2, false, 3 },
                new object[] { "Lovely Jubbly!", "Great Service.", new System.DateTime(2014, 03, 04, 14, 1, 35), 2, false, 4 },
                new object[] { "WrongSize!!!!", "I wanted a schmedium but I received a Large. I'm so mad.", new System.DateTime(2007, 04, 05, 16, 50, 30), 3, false, 5 }
            };
        }
    }
}
