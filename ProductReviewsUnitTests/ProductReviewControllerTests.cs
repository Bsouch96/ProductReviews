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
        /// <returns>A list of DeletionRequestModel</returns>
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
        /// <returns>A list of DeletionRequestModel from the _memoryCacheMock</returns>
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
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsCount()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>();
            var repoExpected = GetProductReviews();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value);
            Assert.Equal(repoExpected.Count, model.Count());

            mockDeletionRequestRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsContent()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsContentFromCache()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviewsForMemoryCache();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllProductReviews();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllProductReviewsAsync(), Times.Never);
        }

        [Fact]
        public async void GetAll_ThrowsException()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllProductReviewsAsync()).ThrowsAsync(new Exception()).Verifiable();
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

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
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden).ToList();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllDeletionRequestsCount(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>();
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value);
            Assert.Equal(repoExpected.Count, model.Count());
            mockDeletionRequestRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllDeletionRequestsContent(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviews().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAllVisibleProductReviewsForProduct_ShouldReturnAllDeletionRequestsContentFromCache(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var repoExpected = GetProductReviewsForMemoryCache().Where(pr => !pr.ProductReviewIsHidden && pr.ProductID == ID).ToList();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID)).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = GetProductReviewsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<ProductReviewReadDTO> expected = _mapper.Map<IEnumerable<ProductReviewReadDTO>>(repoExpected).ToList();

            //Act
            var result = await productReviewController.GetAllVisibleProductReviewsForProduct(ID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<ProductReviewReadDTO> model = Assert.IsAssignableFrom<IEnumerable<ProductReviewReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllVisibleProductReviewsForProductAsync(ID), Times.Never);
        }

        [Fact]
        public async void GetAllVisibleProductReviewsForProduct_ThrowsException()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllVisibleProductReviewsForProductAsync(1)).ThrowsAsync(new Exception()).Verifiable();
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

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
            var mockDeletionRequestRepo = new Mock<IProductReviewsRepository>(MockBehavior.Strict);
            var productReviewController = new ProductReviewController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await productReviewController.GetAllVisibleProductReviewsForProduct(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }
    }
}
