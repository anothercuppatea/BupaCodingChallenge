using Xunit;
using Moq;
using BupaCodingChallenge.Controllers;
using BupaCodingChallenge.Models;
using BupaCodingChallenge.Services;
using System.Net.Http;
using Moq.Protected;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Web.Http.Tracing;
using System.Text.Json;
using System.Web.Http;

namespace BupaCodingChallengeTest.Tests
{
    public class BooksServiceTest
    {
        private readonly BooksService _booksService;
        private readonly HttpClient _httpClient;
        private readonly Mock<HttpMessageHandler> _handler;

        public BooksServiceTest()
        {
            //handler mocks the responses.
            _handler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handler.Object);
            _booksService = new BooksService(_httpClient);
            
        }

        [Fact]
        public async Task MockBupaReturnsTooManyRequests()
        {
            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests
            });

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _booksService.GetOwnersAsync());
            Assert.Equal("Too many requests.", exception.Message);
        }

        [Fact]
        public async Task MockBupaReturnsBadRequest()
        {
            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _booksService.GetOwnersAsync());
            Assert.Equal("Bad request.", exception.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.Gone)]
        [InlineData(HttpStatusCode.Forbidden)]
        public async Task MockBupaReturnsOther(HttpStatusCode code)
        {
            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = code
            });

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _booksService.GetOwnersAsync());
            Assert.Contains($"Failed with status: {code}", exception.Message);
        }

        [Fact]
        public async Task MockBupaReturnsData()
        {
            var jsonResponse = "[{\"name\":\"Jane\",\"age\":23,\"books\":[{\"name\":\"Hamlet\",\"type\":\"Hardcover\"},{\"name\":\"Wuthering Heights\",\"type\":\"Paperback\"}]},{\"name\":\"Charlotte\",\"age\":14,\"books\":[{\"name\":\"Hamlet\",\"type\":\"Paperback\"}]}]";

            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

            var result = await _booksService.GetOwnersAsync();

            Assert.NotNull(result);
            Assert.Equal(JsonSerializer.Deserialize<List<Owner>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }), result);
        }

        [Fact]
        public async Task MockBupaReturnsEmptyData() {

            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            }) ;

            var exception = await Assert.ThrowsAsync<HttpResponseException>(() => _booksService.GetOwnersAsync());
            Assert.Equal(HttpStatusCode.NotFound, exception.Response.StatusCode);
        }

        [Fact]
        public async Task MockBupaReturnsInvalidData()
        { 
            _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                //unprocessable json
                Content = new StringContent("{ name: 'Jane', age: 23 ")
            });

            var exception = await Assert.ThrowsAsync<HttpResponseException>(() => _booksService.GetOwnersAsync());
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.Response.StatusCode);
        }
    }
}