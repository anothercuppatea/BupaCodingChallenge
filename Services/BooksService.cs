using BupaCodingChallenge.Interfaces;
using BupaCodingChallenge.Models;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Web.Http;

namespace BupaCodingChallenge.Services
{
    public class BooksService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url = "https://digitalcodingtest.bupa.com.au/api/v1/bookowners";

        public BooksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Owner>> GetOwnersAsync()
        {
            var response = await _httpClient.GetAsync(_url);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var content = await response.Content.ReadAsStringAsync();
                    if(string.IsNullOrEmpty(content))
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<List<Owner>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch (Exception)
                        {
                            throw new HttpResponseException(HttpStatusCode.UnprocessableEntity);                            
                        }
                    }
                case HttpStatusCode.TooManyRequests:
                    throw new HttpRequestException("Too many requests.", null, HttpStatusCode.TooManyRequests);
                case HttpStatusCode.BadRequest:
                    throw new HttpRequestException("Bad request.", null, HttpStatusCode.BadRequest);
                default:
                    throw new HttpRequestException($"Failed with status: {response.StatusCode}", null, response.StatusCode);
            }
        }
    }
}
