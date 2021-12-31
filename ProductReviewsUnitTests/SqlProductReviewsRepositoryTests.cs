using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ProductReviews.CustomExceptionMiddleware;
using ProductReviews.DomainModels;
using ProductReviews.Repositories.Concrete;
using ProductReviewsUnitTests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProductReviewsUnitTests
{
    public class SqlProductReviewsRepositoryTests
    {
        public SqlProductReviewsRepositoryTests(){}

        /// <summary>
        /// Gets the expected product reviews used for repo returns to isolate the controller.
        /// </summary>
        /// <returns>A list of ProductReviewModel</returns>
        private List<ProductReviewModel> GetProductReviews()
        {
            return new List<ProductReviewModel>()
            {
                new ProductReviewModel { ProductReviewID = 1, ProductReviewHeader = "Wow!", ProductReviewContent = "Lovely Shoes.", ProductReviewDate = new System.DateTime(2010, 10, 01, 8, 5, 3), ProductID = 1, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 2, ProductReviewHeader = "Amazing!", ProductReviewContent = "Best shirt since sliced bread.", ProductReviewDate = new System.DateTime(2012, 01, 02, 10, 3, 45), ProductID = 1, ProductReviewIsHidden = true },
                new ProductReviewModel { ProductReviewID = 3, ProductReviewHeader = "Terrible!", ProductReviewContent = "Did not receive order...", ProductReviewDate = new System.DateTime(2013, 02, 03, 12, 2, 40), ProductID = 2, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 4, ProductReviewHeader = "Lovely Jubbly!", ProductReviewContent = "Great Service.", ProductReviewDate = new System.DateTime(2014, 03, 04, 14, 1, 35), ProductID = 2, ProductReviewIsHidden = true },
                new ProductReviewModel { ProductReviewID = 5, ProductReviewHeader = "WrongSize!!!!", ProductReviewContent = "I wanted a schmedium but I received a Large. I'm so mad.", ProductReviewDate = new System.DateTime(2007, 04, 05, 16, 50, 30), ProductID = 3, ProductReviewIsHidden = false }
            };
        }

        /// <summary>
        /// Create a mock of the ProductReviews.Context.DbContext class seeded with ProductReviewModels.
        /// </summary>
        /// <returns>A mock of type Invoices.Context.DbContext</returns>
        private Mock<ProductReviews.Context.DbContext> GetDbContext()
        {
            var context = new Mock<ProductReviews.Context.DbContext>();
            context.Object.AddRange(GetProductReviews());
            context.Object.SaveChanges();

            return context;
        }

        /// <summary>
        /// Create a mock DbSet of type ProductReviewModel.
        /// </summary>
        /// <returns>A mock DbSet of type ProductReviewModel</returns>
        private Mock<DbSet<ProductReviewModel>> GetMockDbSet()
        {
            return GetProductReviews().AsQueryable().BuildMockDbSet();
        }

        [Fact]
        public void GetAllProductReviewsAsync_ShouldReturnAllProductReviews()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            var expectedResult = GetProductReviews();

            //Act
            var result = sqlProductReviewsRepository.GetAllProductReviewsAsync();

            //Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<List<ProductReviewModel>>(result.Result);
            var model = Assert.IsAssignableFrom<List<ProductReviewModel>>(actionResult);
            Assert.Equal(expectedResult.Count(), model.Count());
            model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetAllVisibleProductReviewsForProductAsync_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            var expectedResult = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sqlProductReviewsRepository.GetAllVisibleProductReviewsForProductAsync(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetAllVisibleProductReviewsForProductAsync_ShouldReturnAllVisibleProductReviewsForProduct(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            var expectedResult = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID);

            //Act
            var result = sqlProductReviewsRepository.GetAllVisibleProductReviewsForProductAsync(ID);

            //Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<List<ProductReviewModel>>(result.Result);
            var model = Assert.IsAssignableFrom<List<ProductReviewModel>>(actionResult);
            Assert.Equal(expectedResult.Count(), model.Count());
            model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetDeletionRequestAsync_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sqlProductReviewsRepository.GetProductReviewAsync(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void GetDeletionRequestAsync_ShouldReturnDeletionRequest(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            var expectedResult = GetProductReviews().FirstOrDefault(dr => dr.ProductReviewID == ID);

            //Act
            var result = sqlProductReviewsRepository.GetProductReviewAsync(ID);

            //Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<ProductReviewModel>(result.Result);
            var model = Assert.IsAssignableFrom<ProductReviewModel>(actionResult);
            model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequestAsync_ThrowsResourceNotFoundException(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sqlProductReviewsRepository.GetProductReviewAsync(ID));
            Assert.Equal($"A resource for ID: {ID} does not exist.", exception.Message);
        }

        [Fact]
        public void CreateDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            ProductReviewModel productReviewModel = null;

            //Act and Assert
            var exception = Assert.Throws<ArgumentNullException>(() => sqlProductReviewsRepository.CreateProductReview(productReviewModel));
            Assert.Equal("The product review used to update cannot be null. (Parameter 'productReviewModel')", exception.Message);
        }

        [Theory, MemberData(nameof(ProductReviewModelObjects.GetProductReviewModelCreateObjects),
                 MemberType = typeof(ProductReviewModelObjects))]
        public async void CreateDeletionRequest_ShouldReturnCreatedDeletionRequest(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductReviews.Context.DbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;
            var dbContextMock = new ProductReviews.Context.DbContext(options);
            dbContextMock._productReviews.AddRange(GetProductReviews());
            await dbContextMock.SaveChangesAsync();
            ProductReviewModel productReviewModel = new ProductReviewModel()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            ProductReviewModel expectedProductReviewModel = new ProductReviewModel()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden,
                ProductReviewID = (GetProductReviews().Count + 1)
            };
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock);

            //Act
            var result = sqlProductReviewsRepository.CreateProductReview(productReviewModel);

            //Assert
            var actionResult = Assert.IsType<ProductReviewModel>(result);
            ProductReviewModel deletionRequestModelResult = Assert.IsAssignableFrom<ProductReviewModel>(actionResult);
            deletionRequestModelResult.Should().NotBeNull();
            deletionRequestModelResult.Should().BeEquivalentTo(expectedProductReviewModel);
        }

        [Fact]
        public void UpdateDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            ProductReviewModel productReviewModel = null;

            //Act and Assert
            var exception = Assert.Throws<ArgumentNullException>(() => sqlProductReviewsRepository.UpdateProductReview(productReviewModel));
            Assert.Equal("The product review used to update cannot be null. (Parameter 'productReviewModel')", exception.Message);
        }

        [Theory, MemberData(nameof(ProductReviewModelObjects.GetProductReviewModelUpdateObjects),
                 MemberType = typeof(ProductReviewModelObjects))]
        public void UpdateDeletionRequest_ShouldUpdateDeletionRequest(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden, int ProductReviewID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._productReviews).Returns(dbSetMock.Object);
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);
            ProductReviewModel productReviewModel = new ProductReviewModel()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden,
                ProductReviewID = ProductReviewID
            };

            //Act
            sqlProductReviewsRepository.UpdateProductReview(productReviewModel);

            //Assert
            dbSetMock.Verify(r => r.Update(It.IsAny<ProductReviewModel>()), Times.Once);
        }

        [Fact]
        public async void SaveChangesAsync_ShouldOnlySaveOnce()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1)).Verifiable();
            var sqlProductReviewsRepository = new SqlProductReviewsRepository(dbContextMock.Object);

            //Act
            await sqlProductReviewsRepository.SaveChangesAsync();

            //Assert
            dbContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
