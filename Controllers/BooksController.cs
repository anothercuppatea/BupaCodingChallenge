using BupaCodingChallenge.Interfaces;
using BupaCodingChallenge.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace BupaCodingChallenge.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _booksService;
        private static List<Owner>? _owners;

        public BooksController(IBookService booksService)
        {
            _booksService = booksService;
        }

        public async Task GetBooks()
        {
            if (_owners == null) {
                _owners = await _booksService.GetOwnersAsync();
            }            
        }


        [HttpGet]
        public async Task<IActionResult> GetBooklistAsync()
        {
            try
            {
                await GetBooks();
                List<string> childBooks = new List<string>();
                List<string> adultBooks = new List<string>();
                Dictionary<string, List<string>> toReturn = new Dictionary<string, List<string>>();
                if (_owners == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                foreach (var owner in _owners)
                {
                    var targetList = owner.Age < 18 ? childBooks : adultBooks;
                    targetList.AddRange(owner.Books.Select(b => b.Name));
                }
                childBooks.Sort();
                adultBooks.Sort();

                toReturn.Add("child", childBooks);
                toReturn.Add("adult", adultBooks);
                return Ok(toReturn);
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Error fetching data : {ex.Message}")
                {
                    StatusCode = ex.StatusCode == null ? 500 : (int)ex.StatusCode
                };
            }
            catch (HttpResponseException ex)
            {
                return new ObjectResult($"Error fetching data : {ex.Message}")
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
            catch (Exception)
            {
                return StatusCode(500, "There was an internal error fetching data.");
            }
        }

        [HttpGet("hardcover")]
        public async Task<IActionResult> GetBookListHardcoverAsync()
        {
            try
            {
                await GetBooks();
                List<string> childBooks = new List<string>();
                List<string> adultBooks = new List<string>();
                Dictionary<string, List<string>> toReturn = new Dictionary<string, List<string>>();
                if(_owners == null) {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                foreach (var owner in _owners)
                {
                    var targetList = owner.Age < 18 ? childBooks : adultBooks;
                    targetList.AddRange(owner.Books.Where(b => b.Type.Equals("Hardcover", StringComparison.OrdinalIgnoreCase)).Select(b => b.Name));
                }
                childBooks.Sort();
                adultBooks.Sort();

                toReturn.Add("child", childBooks);
                toReturn.Add("adult", adultBooks);
                return Ok(toReturn);
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Error fetching data : {ex.Message}")
                {
                    StatusCode = ex.StatusCode == null ? 500 : (int)ex.StatusCode
                };
            }
            catch (HttpResponseException ex)
            {
                return new ObjectResult($"Error fetching data : {ex.Message}")
                {
                    StatusCode = ex.Response.StatusCode == null ? 500 : (int)ex.Response.StatusCode
                };
            }

            catch (Exception)
            {
                return StatusCode(500, "There was an internal error fetching data.");
            }
        }
    }
}
