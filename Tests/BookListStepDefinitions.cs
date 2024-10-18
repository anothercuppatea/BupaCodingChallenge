using System;
using System.Buffers;
using TechTalk.SpecFlow;
using BupaCodingChallenge;
using BupaCodingChallenge.Interfaces;
using Moq;
using BupaCodingChallenge.Models;
using TechTalk.SpecFlow.Assist;
using BupaCodingChallenge.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace BupaCodingChallengeSpecflow.StepDefinitions
{
    [Binding]
    public class BookListStepDefinitions
    {
        private readonly Mock<IBookService> _bookService = new Mock<IBookService>();
        private List<Owner> _owners;

        private Dictionary<string, List<string>> _result; 

        [Given(@"the book service has the following books")]
        public void GivenTheBookServiceHasTheFollowingBooks(Table table)
        {
            _owners = table.CreateSet<Owner>().ToList();
            _bookService.Setup(s => s.GetOwnersAsync()).ReturnsAsync(_owners);
        }

        [When(@"I request the categorised books list")]
        public async void WhenIRequestTheCategorisedBooksList()
        {
            var controller = new BooksController(_bookService.Object);
            var actionResult = await controller.GetBooklistAsync();
            _result = (actionResult as OkObjectResult)?.Value as Dictionary<string, List<string>>;
        }

        [Then(@"I should recieve the following")]
        public void ThenIShouldRecieveTheFollowing(Table table)
        {
            foreach (var item in _result) {
                Console.Write(item);
            }
            var expected = table.Rows.ToDictionary(
                row => row["Category"],
                row => row["Books"].Split(',').Select(b => b.Trim()).ToList());
            foreach (var key in expected.Keys) {
                Assert.Equal(expected[key], _result[key]);
            }
        }
    }
}
