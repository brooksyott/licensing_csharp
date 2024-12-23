namespace Licensing.Common
{
    public class BasicQueryFilter
    {
        public BasicQueryFilter()
        {
            Limit = 1000;
            Offset = 0;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
    }

}