using BupaCodingChallenge.Interfaces;
using Moq;
using BupaCodingChallenge.Models;
using BupaCodingChallenge.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BupaCodingChallengeSpecflow.StepDefinitions
{
    [Binding]
    public class BookListStepDefinitions
    {
        private readonly Mock<IBookService> _bookService = new Mock<IBookService>();
        private List<Owner> _owners;

        private Dictionary<string, List<string>> _result; 

        [Given(@"the book service has the following books")]
        public void GivenTheBookServiceHasTheFollowingBooks(string json)
        {
            
            _owners = JsonSerializer.Deserialize<List<Owner>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _bookService.Setup(s => s.GetOwnersAsync()).ReturnsAsync(_owners);
        }

        [When(@"I request the categorised books list")]
        public async void WhenIRequestTheCategorisedBooksList()
        {
            var controller = new BooksController(_bookService.Object);
            IActionResult actionResult = await controller.GetBooklistAsync();

            var okResult = actionResult as OkObjectResult;
            _result = okResult.Value as Dictionary<string, List<string>>;
        }

        [Then(@"I should recieve the following")]
        public void ThenIShouldRecieveTheFollowing(Table table)
        {
            Dictionary<string, List<string>> expected = table.Rows.ToDictionary(
                row => row["Category"],
                row => row["Books"].Split(',').ToList()
                );

            Assert.NotNull(_result);
            Assert.Equal(expected.Count, _result.Count);

        }

    }
}
