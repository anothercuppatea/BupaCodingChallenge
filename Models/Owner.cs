using System.Linq;

namespace BupaCodingChallenge.Models
{
    public class Owner
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public List<Book> Books { get; set; } = new List<Book>();

        public override bool Equals(object? obj)
        {
            //Custom equals override as SequenceEqual does not work during testing.
            return obj is Owner owner && Name == owner.Name && Age == owner.Age && Books.SequenceEqual(owner.Books);
        }

        public override int GetHashCode() {
            return Name.GetHashCode() ^ Age.GetHashCode() ^ Books.GetHashCode();
        }
    }
}
