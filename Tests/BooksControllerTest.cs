using Xunit;
using Moq;
using BupaCodingChallenge.Controllers;
using BupaCodingChallenge.Models;
using BupaCodingChallenge.Services;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using BupaCodingChallenge.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit.Sdk;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Web.Http;

namespace BupaCodingChallengeTest
{
    public class BooksControllerTest
    {
        private readonly Mock<IBookService> _mockService;
        private readonly BooksController _booksController;
        private List<Owner> _ownersMock;

        public BooksControllerTest()
        {
            _mockService = new Mock<IBookService>();
            _booksController = new BooksController(_mockService.Object);
            _ownersMock = new List<Owner>
            {
                new Owner
                {
                    Name = "Jim",
                    Age = 27,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "PaperBook1",
                            Type = "Paperback"
                        },
                        new Book
                        {
                            Name= "HardBook1",
                            Type = "Hardcover"
                        }
                    }
                },
                new Owner
                {
                    Name = "Dwight",
                    Age = 30,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "PaperBook2",
                            Type = "Paperback"
                        },
                        new Book
                        {
                            Name= "HardBook2",
                            Type = "Hardcover"
                        }
                    }
                },
                new Owner
                {
                    Name = "Cecilia",
                    Age = 6,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "PaperBook3",
                            Type = "Paperback"
                        }
                    }
                },
                new Owner
                {
                    Name = "Creed",
                    Age = 15,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "HardBook3",
                            Type = "Hardcover"
                        }
                    }
                }
            };
        }

        [Fact]
        public async Task ControllerReturnsAllBooks()
        {
            _mockService.Setup(s => s.GetOwnersAsync()).ReturnsAsync(_ownersMock);
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            List<string> childBooks = new List<string> { "PaperBook3", "HardBook3" };
            List<string> adultBooks = new List<string> { "PaperBook1", "HardBook1", "PaperBook2", "HardBook2" };
            childBooks.Sort();
            adultBooks.Sort();
            values.Add("child", childBooks);
            values.Add("adult", adultBooks);

            var result = await _booksController.GetBooklistAsync();
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Dictionary<string, List<string>>>(okResult.Value);
            Assert.Equal(values, returnValue);
        }
        [Fact]
        public async Task ControllerReturnsHardCover()
        {
            _mockService.Setup(s => s.GetOwnersAsync()).ReturnsAsync(_ownersMock);
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            List<string> childBooks = new List<string> { "HardBook3" };
            List<string> adultBooks = new List<string> { "HardBook1", "HardBook2" };
            childBooks.Sort();
            adultBooks.Sort();
            values.Add("child", childBooks);
            values.Add("adult", adultBooks);

            var result = await _booksController.GetBookListHardcoverAsync();
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Dictionary<string, List<string>>>(okResult.Value);
            Assert.Equal(values, returnValue);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "Bad request.")]
        [InlineData(HttpStatusCode.TooManyRequests, "Too many requests.")]
        [InlineData(HttpStatusCode.Gone, "The requested resource is gone.")]
        public async Task ServiceReturnsExceptionAllBooks(HttpStatusCode status, string message)
        {
            _mockService.Setup(s => s.GetOwnersAsync()).ThrowsAsync(new HttpRequestException(message, null, status));
            var result = await _booksController.GetBooklistAsync();
            var resultValue = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)status, resultValue.StatusCode);
            Assert.Equal("Error fetching data : " + message, resultValue.Value);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "Bad request.")]
        [InlineData(HttpStatusCode.TooManyRequests, "Too many requests.")]
        [InlineData(HttpStatusCode.Gone, "The requested resource is gone.")]
        public async Task ServiceReturnsExceptionHardCover(HttpStatusCode status, string message)
        {
            _mockService.Setup(s => s.GetOwnersAsync()).ThrowsAsync(new HttpRequestException(message, null, status));
            var result = await _booksController.GetBookListHardcoverAsync();
            var resultValue = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)status, resultValue.StatusCode);
            Assert.Equal("Error fetching data : " + message, resultValue.Value);
        }
        [Fact]
        public async Task ServiceReturnsResponseExceptionAllBooks()
        {
            _mockService.Setup(s => s.GetOwnersAsync()).ThrowsAsync(new HttpResponseException(HttpStatusCode.UnprocessableEntity));
            var result = await _booksController.GetBooklistAsync();
            var resultValue = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.UnprocessableEntity, resultValue.StatusCode);
            Assert.Equal("Error fetching data : Processing of the HTTP request resulted in an exception. Please see the HTTP response returned by the 'Response' property of this exception for details.", resultValue.Value);
        }
    }
}