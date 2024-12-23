namespace Licensing.Common
{
    public class PaginatedResults
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 0;
        public int Count { get; set; } = 0;
        public Object? Results { get; set; }
    }
}
