using OrdersAPI.DTOs;

namespace OrdersAPI.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<List<OrderResponseDto>> GetOrdersAsync(int page, int pageSize, string? customerName);
        Task<OrderResponseDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<MetricsDto> GetMetricsAsync();
    }
}