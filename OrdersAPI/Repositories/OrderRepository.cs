using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Models;

namespace OrdersAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<List<Order>> GetAllAsync(int page, int pageSize, string? customerName)
        {
            var query = _context.Orders.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(o => o.CustomerName.Contains(customerName));
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new Order 
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<Order?> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> AverageAmountAsync()
        {
            if (!await _context.Orders.AnyAsync())
                return 0;

            // Converter para double para o SQLite conseguir calcular
            var orders = await _context.Orders.ToListAsync();
            return orders.Any() ? (decimal)orders.Average(o => (double)o.TotalAmount) : 0;
        }
    }
}