namespace OrdersAPI.DTOs
{
    public class MetricsDto
    {
        public int TotalOrders { get; set; }
        public decimal AverageOrderAmount { get; set; }
        public double AverageCreationTimeMs { get; set; }
    }
}