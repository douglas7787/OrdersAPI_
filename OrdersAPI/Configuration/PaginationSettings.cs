namespace OrdersAPI.Configuration
{
    public class PaginationSettings
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        public static int ValidatePageSize(int pageSize)
        {
            if (pageSize <= 0) return DefaultPageSize;
            if (pageSize > MaxPageSize) return MaxPageSize;
            return pageSize;
        }
    }
}