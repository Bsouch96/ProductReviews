using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProductReviews;
using ProductReviews.DomainModels;
using ProductReviews.DTOs;
using ProductReviews.Models;
using ProductReviewsIntegrationTests.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProductReviewsIntegrationTests
{
    public class ProductReviewsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configurationSecrets;
        private readonly IConfiguration _configurationJson;
        private readonly IConfigurationSection _auth0Settings;

        public ProductReviewsIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();

            _configurationSecrets = new ConfigurationBuilder()
                .AddUserSecrets<ProductReviewsIntegrationTests>()
                .Build();

            _configurationJson = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _auth0Settings = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Auth0");
        }

        /// <summary>
        /// Acquire the access token via the Auth0 api to comply with authorisation and authentication rules.
        /// </summary>
        /// <returns>An access token that can be used to access the product review's API endpoints</returns>
        private async Task<string> GetAccessToken()
        {
            var auth0Client = new AuthenticationApiClient(_auth0Settings["Domain"]);

            var clientID = _configurationJson["AuthClientID"];
            if (String.IsNullOrWhiteSpace(clientID))
                clientID = _configurationSecrets["AuthClientID"];

            var clientSecret = _configurationJson["AuthClientSecret"];
            if (String.IsNullOrWhiteSpace(clientSecret))
                clientSecret = _configurationSecrets["AuthClientSecret"];

            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = clientID,
                ClientSecret = clientSecret,
                Audience = _auth0Settings["Audience"]
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
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

        [Fact]
        public async void GetAllProductReviews_ReturnsUnauthorisedAccessWithNoToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/ProductReviews");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void GetAllProductReviews_ReturnsUnauthorisedAccessWithInvalidToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/ProductReviews");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void GetAllProductReviews_ReturnsOKResponseWithValidToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/ProductReviews");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetAllVisibleProductReviewsForProduct_ReturnsUnauthorisedAccessWithNoToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ProductReviews/Visible/{ID}");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetAllVisibleProductReviewsForProduct_ReturnsUnauthorisedAccessWithInvalidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/Visible/{ID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetAllVisibleProductReviewsForProduct_ReturnsOKResponseWithValidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/Visible/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetProductReview_ReturnsUnauthorisedAccessWithNoToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetProductReview_ReturnsUnauthorisedAccessWithInvalidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetProductReview_ReturnsOKResponseWithValidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetProductReview_ReturnsCorrectDeletionRequest(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var productReview = JsonConvert.DeserializeObject<ProductReviewReadDTO>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(productReview.ProductReviewID == ID);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetProductReview_ThrowsArgumentOutOfRangeException_HandledByGlobalExceptionHandler(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = "IDs cannot be less than 1. (Parameter 'ID')";

            //Act and Assert
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(300)]
        [InlineData(int.MaxValue)]
        public async void GetProductReview_ThrowsResourceNotFoundException_HandledByGlobalExceptionHandler(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProductReviews/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = $"A resource for ID: {ID} does not exist.";

            //Act and Assert
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.NotFound, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO productReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/ProductReviews/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(productReviewCreateDTO), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO productReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/ProductReviews/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(productReviewCreateDTO), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsCreatedResponseWithValidToken(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO productReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/ProductReviews/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(productReviewCreateDTO), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory, MemberData(nameof(ProductReviewCreateDTOObjects.GetProductReviewCreateDTOObjects),
                 MemberType = typeof(ProductReviewCreateDTOObjects))]
        public async void CreateDeletionRequest_CanCreateDeletionRequest(string ProductReviewHeader, string ProductReviewContent,
            DateTime ProductReviewDate, int ProductID, bool ProductReviewIsHidden)
        {
            //Arrange
            ProductReviewCreateDTO productReviewCreateDTO = new ProductReviewCreateDTO()
            {
                ProductReviewHeader = ProductReviewHeader,
                ProductReviewContent = ProductReviewContent,
                ProductReviewDate = ProductReviewDate,
                ProductID = ProductID,
                ProductReviewIsHidden = ProductReviewIsHidden
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/ProductReviews/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(productReviewCreateDTO), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var returnedProductReview = JsonConvert.DeserializeObject<ProductReviewReadDTO>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal($"/api/ProductReviews/{returnedProductReview.ProductReviewID}", response.Headers.Location.AbsolutePath);
            Assert.Equal(productReviewCreateDTO.ProductReviewHeader, returnedProductReview.ProductReviewHeader);
            Assert.Equal(productReviewCreateDTO.ProductReviewContent, returnedProductReview.ProductReviewContent);
        }

        [Theory, MemberData(nameof(ProductReviewUpdateDTOObjects.GetProductReviewUpdateDTOObjects),
                 MemberType = typeof(ProductReviewUpdateDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(int ProductReviewID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO deletionRequestApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ProductReviews/Visibility/{ProductReviewID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, MemberData(nameof(ProductReviewUpdateDTOObjects.GetProductReviewUpdateDTOObjects),
                 MemberType = typeof(ProductReviewUpdateDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(int ProductReviewID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO deletionRequestApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ProductReviews/Visibility/{ProductReviewID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, MemberData(nameof(ProductReviewUpdateDTOObjects.GetProductReviewUpdateDTOObjects),
                 MemberType = typeof(ProductReviewUpdateDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsNoContent(int ProductReviewID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO deletionRequestApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ProductReviews/Visibility/{ProductReviewID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(-10, true)]
        [InlineData(int.MinValue, true)]
        public async void ApproveDeletionRequest_ThrowsArgumentOutOfRangeException_HandledByGlobalExceptionHandler(int ProductReviewID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO deletionRequestApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ProductReviews/Visibility/{ProductReviewID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = "IDs cannot be less than 1. (Parameter 'ID')";

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }

        [Theory]
        [InlineData(50, true)]
        [InlineData(100, true)]
        [InlineData(int.MaxValue, true)]
        public async void ApproveDeletionRequest_ThrowsResourceNotFoundException_HandledByGlobalExceptionHandler(int ProductReviewID, bool ProductReviewIsHidden)
        {
            //Arrange
            JsonPatchDocument<ProductReviewUpdateDTO> jsonPatchDocument = new JsonPatchDocument<ProductReviewUpdateDTO>();
            ProductReviewUpdateDTO deletionRequestApproveDTO = new ProductReviewUpdateDTO()
            {
                ProductReviewIsHidden = ProductReviewIsHidden
            };
            jsonPatchDocument.Replace<bool>(dr => dr.ProductReviewIsHidden, ProductReviewIsHidden);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ProductReviews/Visibility/{ProductReviewID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = $"A resource for ID: {ProductReviewID} does not exist."; ;

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.NotFound, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }
    }
}
