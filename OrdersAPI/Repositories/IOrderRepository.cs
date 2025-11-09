using OrdersAPI.Models;

namespace OrdersAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync(int page, int pageSize, string? customerName);
        Task<Order?> UpdateAsync(Order order);
        Task<int> CountAsync();
        Task<decimal> AverageAmountAsync();
    }
}