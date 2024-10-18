namespace BupaCodingChallenge.Models
{
    public class Book
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            //Custom equals override as SequenceEqual does not work during testing.
            return obj is Book book && Name == book.Name && Type == book.Type;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Type.GetHashCode();
        }
    }
}
