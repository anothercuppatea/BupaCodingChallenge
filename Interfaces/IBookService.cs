using BupaCodingChallenge.Models;

namespace BupaCodingChallenge.Interfaces
{
    public interface IBookService
    {
        Task<List<Owner>> GetOwnersAsync();
    }
}
