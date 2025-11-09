using Moq;
using OrdersAPI.DTOs;
using OrdersAPI.Models;
using OrdersAPI.Repositories;
using OrdersAPI.Services;
using Xunit;

namespace OrdersAPI.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_ValidData_ReturnsOrderResponse()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var service = new OrderService(mockRepo.Object);

            var dto = new CreateOrderDto
            {
                CustomerName = "João Silva",
                TotalAmount = 350.50m
            };

            var order = new Order
            {
                Id = 1,
                CustomerName = dto.CustomerName,
                TotalAmount = dto.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Order>()))
                    .ReturnsAsync(order);

            // Act
            var result = await service.CreateOrderAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CustomerName, result.CustomerName);
            Assert.Equal(dto.TotalAmount, result.TotalAmount);
            mockRepo.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ExistingId_ReturnsOrder()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var service = new OrderService(mockRepo.Object);

            var order = new Order
            {
                Id = 1,
                CustomerName = "Maria Santos",
                TotalAmount = 500m,
                CreatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.GetByIdAsync(1))
                    .ReturnsAsync(order);

            // Act
            var result = await service.GetOrderByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Maria Santos", result.CustomerName);
        }

        [Fact]
        public async Task GetOrderByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var service = new OrderService(mockRepo.Object);

            mockRepo.Setup(r => r.GetByIdAsync(999))
                    .ReturnsAsync((Order?)null);

            // Act
            var result = await service.GetOrderByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}