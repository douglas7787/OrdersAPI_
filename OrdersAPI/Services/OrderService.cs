using System.Diagnostics;
using OrdersAPI.DTOs;
using OrdersAPI.Models;
using OrdersAPI.Repositories;

namespace OrdersAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private static readonly List<double> _creationTimes = new();

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                TotalAmount = dto.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(order);

            stopwatch.Stop();
            _creationTimes.Add(stopwatch.Elapsed.TotalMilliseconds);

            return new OrderResponseDto
            {
                Id = created.Id,
                CustomerName = created.CustomerName,
                TotalAmount = created.TotalAmount,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return null;

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync(int page, int pageSize, string? customerName)
        {
            var orders = await _repository.GetAllAsync(page, pageSize, customerName);

            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            }).ToList();
        }

        public async Task<OrderResponseDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return null;

            order.TotalAmount = dto.TotalAmount;
            await _repository.UpdateAsync(order);

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<MetricsDto> GetMetricsAsync()
        {
            var total = await _repository.CountAsync();
            var average = total > 0 ? await _repository.AverageAmountAsync() : 0;
            var avgTime = _creationTimes.Count > 0 ? _creationTimes.Average() : 0;

            return new MetricsDto
            {
                TotalOrders = total,
                AverageOrderAmount = average,
                AverageCreationTimeMs = Math.Round(avgTime, 2)
            };
        }
    }
}