using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using ProductReviews.Controllers;
using ProductReviews.DomainModels;
using ProductReviews.DTOs;
using ProductReviews.Models;
using ProductReviews.Profiles;
using ProductReviews.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using ProductReviews.CustomExceptionMiddleware;
using ProductReviewsUnitTests.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductReviewsUnitTests
{
    public class ProductReviewControllerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly IOptions<MemoryCacheModel> _memoryCacheModel;

        /// <summary>
        /// Constructor to setup the reusable objects.
        /// </summary>
        public ProductReviewControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductReviewsProfile());
            });

            _memoryCacheMock = new Mock<IMemoryCache>();
            _memoryCacheModel = Options.Create(new MemoryCacheModel());
            _mapper = config.CreateMapper();
        }

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
        /// Gets the expected deletion requests used to populate the _memoryCacheMock.
        /// </summary>
        /// <returns>A list of ProductReviewModel from the _memoryCacheMock</returns>
        private List<ProductReviewModel> GetProductReviewsForMemoryCache()
        {
            return new List<ProductReviewModel>()
            {
                new ProductReviewModel { ProductReviewID = 1, ProductReviewHeader = "Wow!", ProductReviewContent = "Lovely Shoes.", ProductReviewDate = new System.DateTime(2010, 10, 01, 8, 5, 3), ProductID = 1, ProductReviewIsHidden = false },
                new ProductReviewModel { ProductReviewID = 3, ProductReviewHeader = "Terrible!", ProductReviewContent = "Did not receive order...", ProductReviewDate = new System.DateTime(2013, 02, 03, 12, 2, 40), ProductID = 2, ProductReviewIsHidden = true },
                new ProductReviewModel { ProductReviewID = 5, ProductReviewHeader = "WrongSize!!!!", ProductReviewContent = "I wanted a schmedium but I received a Large. I'm so mad.", ProductReviewDate = new System.DateTime(2007, 04, 05, 16, 50, 30), ProductID = 3, ProductReviewIsHidden = false }
            };
        }

        [Fact]
        public async void GetAll_ShouldReturnActionResultType()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            mockProductReviewRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async void GetAll_ShouldReturnAllProductReviewsCount()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>();
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value);
            Assert.Equal(repoExpected.Count, model.Count());

            mockProductReviewRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllProductReviewsContent()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockProductReviewRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllProductReviewsContentFromCache()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviewsForMemoryCache();
            mockProductReviewRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockProductReviewRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Never);
        }

        [Fact]
        public async void GetAll_ThrowsException()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ThrowsAsync(new Exception()).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await productReviewController.GetAllProductReviews());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnActionResultType(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden).ToList();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            mockProductReviewRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllProductReviewsCount(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>();
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockProductReviewRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value);
            Assert.Equal(repoExpected.Count, model.Count());
            mockProductReviewRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllProductReviewsContent(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockProductReviewRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockProductReviewRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllProductReviewsContentFromCache(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviewsForMemoryCache().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockProductReviewRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockProductReviewRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Never);
        }

        [Fact]
        public async void GetAllVisibleProductReviewsForProduct_ThrowsException()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(1)).ThrowsAsync(new Exception()).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await productReviewController.GetAllVisibleProductReviewsForProduct(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetAllVisibleProductReviewsForProduct_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await productReviewController.GetAllVisibleProductReviewsForProduct(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetProductReview_ThrowsArgumentOutOfRangeException_IDLessThanOne(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await productReviewController.GetProductReview(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async void GetProductReview_ShouldReturnFromMemoryCache(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetProductReview(ID);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
            mockProductReviewRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Never);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public async void GetProductReview_ShouldAddToMemoryCacheIfNotExists(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(pr => pr.ProductReviewID == ID)).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var expectedReturn = _mapper.Map<ProductReviewReadDTO>(repoExpected.Find(pr => pr.ProductReviewID == ID));
            //Perform GET to add to the memory cache.
            await productReviewController.GetProductReview(ID);
            mockProductReviewRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
            //Get the newly added ProductReview from the memoryCache.
            var action = await productReviewController.GetProductReview(ID);
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            ProductReviewReadDTO returnedModel = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
            mockProductReviewRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public async void GetProductReview_ShouldReturnFromDBWhenNotInAnExistingCache(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(pr => pr.ProductReviewID == ID)).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var expectedReturn = _mapper.Map<ProductReviewReadDTO>(repoExpected.Find(pr => pr.ProductReviewID == ID));
            var action = await productReviewController.GetProductReview(ID);

            //Assert
            mockProductReviewRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            ProductReviewReadDTO returnedModel = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public async void GetProductReview_ThrowsResourceNotFoundExceptionWhenIDNotInCacheOrDB(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(pr => pr.ProductReviewID == ID)).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await productReviewController.GetProductReview(ID));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public async void GetProductReview_ThrowsResourceNotFoundExceptionWhenIDNotDBWithExpiredCache(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(pr => pr.ProductReviewID == ID)).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await productReviewController.GetProductReview(ID));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async void GetProductReview_ShouldReturnFromDBWhenCacheExpired(int ID)
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(pr => pr.ProductReviewID == ID)).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var expectedReturn = _mapper.Map<ProductReviewReadDTO>(repoExpected.Find(pr => pr.ProductReviewID == ID));
            var action = await productReviewController.GetProductReview(ID);

            //Assert
            mockProductReviewRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            ProductReviewReadDTO returnedModel = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
        }

        [Fact]
        public async void GetProductReview_ThrowsException()
        {
            //Arrange
            var mockProductReviewRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewRepo.Setup(dr => dr.GetProductReviewAsync(1)).ThrowsAsync(new Exception()).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await productReviewController.GetProductReview(1));
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateProductReview_ShouldReturnCreatedAtAction(string ProductReviewHeader, string ProductReviewContent, DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO newProductReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            ProductReviewModel repoProductReviewModel = new ProductReviewModel()
            {
                 ProductReviewHeader = ProductReviewHeader,
                 ProductReviewContent = ProductReviewContent,
                 ProductReviewDate = ProductReviewDate,
                 ProductID = ProductID,
                 ProductReviewIsHidden = ProductReviewIsHidden,
                 ProductReviewID = (GetProductReviews().Count + 1)
            };

            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.CreateProductReview(It.IsAny<ProductReviewModel>())).Returns(repoProductReviewModel).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.CreateProductReview(newProductReviewCreateDTO);

            //Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            ProductReviewReadDTO model = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
        }

        [Fact]
        public async void CreateProductReview_ThrowsArgumentNullException()
        {
            //Arrange
            ProductReviewCreateDTO newProductReviewCreateDTO = null;

            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await productReviewController.CreateProductReview(newProductReviewCreateDTO));
            Assert.Equal("The product review used to update cannot be null. (Parameter 'productReviewCreateDTO')", exception.Message);
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateProductReview_ShouldCreateInDB(string ProductReviewHeader, string ProductReviewContent, DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO newProductReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            ProductReviewModel repoProductReviewModel = new ProductReviewModel()
            {
                 ProductReviewHeader = ProductReviewHeader,
                 ProductReviewContent = ProductReviewContent,
                 ProductReviewDate = ProductReviewDate,
                 ProductID = ProductID,
                 ProductReviewIsHidden = ProductReviewIsHidden,
                 ProductReviewID = (GetProductReviews().Count + 1)
            };

            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.CreateProductReview(It.IsAny<ProductReviewModel>())).Returns(repoProductReviewModel).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.CreateProductReview(newProductReviewCreateDTO);

            //Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            ProductReviewReadDTO model = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            mockProductReviewsRepo.Verify(dr => dr.CreateProductReview(It.IsAny<ProductReviewModel>()), Times.Once());
            mockProductReviewsRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateProductReview_ShouldAddToCache(string ProductReviewHeader, string ProductReviewContent, DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO newProductReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            ProductReviewModel repoProductReviewModel = new ProductReviewModel()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden,
                ProductReviewID = (GetProductReviews().Count + 1)
            };

            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.CreateProductReview(It.IsAny<ProductReviewModel>())).Returns(repoProductReviewModel).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            //Add to cache with create call.
            await productReviewController.CreateProductReview(newProductReviewCreateDTO);
            var result = productReviewController.GetProductReview(repoProductReviewModel.ProductReviewID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result.Result);
            ProductReviewReadDTO model = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            mockProductReviewsRepo.Verify(dr => dr.GetProductReviewAsync(repoProductReviewModel.ProductReviewID), Times.Never());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void UpdateProductReview_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();

            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await productReviewController.UpdateProductReview(ID, jsonPatchDocument));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Fact]
        public async void UpdateProductReview_ThrowsArgumentNullException()
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = null;
            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await productReviewController.UpdateProductReview(1, jsonPatchDocument));
            Assert.Equal("The product review used to update cannot be null. (Parameter 'productReviewUpdatePatch')", exception.Message);
        }

        [Theory]
        [InlineData(10, false)]
        [InlineData(30, false)]
        [InlineData(50, false)]
        public async void ApproveProductReview_ThrowsResourceNotFoundException(int ID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO ProductReviewApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var repoExpected = GetProductReviews();
            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.ProductReviewID == ID)).Verifiable();
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await productReviewController.UpdateProductReview(ID, jsonPatchDocument));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
            mockProductReviewsRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
        }

        [Theory, MemberData(nameof(ProductReviewUpdateDTOObjects.GetProductReviewUpdateDTOObjects),
                 MemberType = typeof(ProductReviewUpdateDTOObjects))]
        public async void ApproveProductReview_ReturnsNoContent(int ID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO ProductReviewApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var repoExpected = GetProductReviews();
            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.ProductReviewID == ID)).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.UpdateProductReview(It.IsAny<ProductReviewModel>())).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            productReviewController.ObjectValidator = objectValidator.Object;

            //Act
            var action = await productReviewController.UpdateProductReview(ID, jsonPatchDocument);

            //Assert
            var actionResult = Assert.IsType<NoContentResult>(action);
            mockProductReviewsRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
            mockProductReviewsRepo.Verify(dr => dr.UpdateProductReview(It.IsAny<ProductReviewModel>()), Times.Once());
            mockProductReviewsRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }

        [Theory, MemberData(nameof(ProductReviewUpdateDTOObjects.GetProductReviewUpdateDTOObjects),
                 MemberType = typeof(ProductReviewUpdateDTOObjects))]
        public async void ApproveProductReview_ShouldAddToCache(int ID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO ProductReviewApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var repoExpected = GetProductReviews();
            var mockProductReviewsRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockProductReviewsRepo.Setup(dr => dr.GetProductReviewAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.ProductReviewID == ID)).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.UpdateProductReview(It.IsAny<ProductReviewModel>())).Verifiable();
            mockProductReviewsRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockProductReviewsRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            productReviewController.ObjectValidator = objectValidator.Object;

            //Act
            //Perform GET to add to the memory cache.
            await productReviewController.UpdateProductReview(ID, jsonPatchDocument);
            //Get the newly added ProductReview from the memoryCache.
            var action = await productReviewController.GetProductReview(ID);
            mockProductReviewsRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            ProductReviewReadDTO returnedModel = Assert.IsAssignableFrom<ProductReviewReadDTO>(actionResult.Value);
            Assert.Equal(returnedModel.ProductReviewID, ID);
            mockProductReviewsRepo.Verify(dr => dr.GetProductReviewAsync(ID), Times.Once());
            mockProductReviewsRepo.Verify(dr => dr.UpdateProductReview(It.IsAny<ProductReviewModel>()), Times.Once());
            mockProductReviewsRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }
    }
}
