namespace Licensing.Common
{
    /// <summary>
    /// Used to return paginated results from the API.
    /// </summary>
    public class PaginatedResults
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 0;
        public int Count { get; set; } = 0;
        public Object? Results { get; set; }
    }
}
