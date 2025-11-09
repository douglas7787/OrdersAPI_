using Microsoft.Extensions.Caching.Memory;
using OrdersAPI.DTOs;

namespace OrdersAPI.Services
{
    public class CachedOrderService : IOrderService
    {
        private readonly OrderService _innerService;
        private readonly IMemoryCache _cache;
        private const string ORDERS_CACHE_KEY = "orders_list";
        private const string ORDER_CACHE_KEY_PREFIX = "order_";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public CachedOrderService(OrderService innerService, IMemoryCache cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        /// <summary>
        /// Lista pedidos com cache
        /// </summary>
        public async Task<List<OrderResponseDto>> GetOrdersAsync(int page, int pageSize, string? customerName)
        {
            // Criar chave única para cada combinação de parâmetros
            var cacheKey = $"{ORDERS_CACHE_KEY}_{page}_{pageSize}_{customerName ?? "all"}";

            // Tentar pegar do cache
            if (_cache.TryGetValue(cacheKey, out List<OrderResponseDto>? cachedOrders))
            {
                return cachedOrders!;
            }

            // Se Não está no cache pega do banco
            var orders = await _innerService.GetOrdersAsync(page, pageSize, customerName);

            // Salvar no cache por 5 minutos
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _cache.Set(cacheKey, orders, cacheOptions);

            return orders;
        }

        /// <summary>
        /// Busca pedido por ID com cache
        /// </summary>
        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var cacheKey = $"{ORDER_CACHE_KEY_PREFIX}{id}";

            // Tentar pegar do cache
            if (_cache.TryGetValue(cacheKey, out OrderResponseDto? cachedOrder))
            {
                return cachedOrder;
            }

            // Buscar do banco
            var order = await _innerService.GetOrderByIdAsync(id);

            if (order != null)
            {
                // Salvar no cache
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, order, cacheOptions);
            }

            return order;
        }

        /// <summary>
        /// Cria pedido e INVALIDA o cache
        /// </summary>
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var result = await _innerService.CreateOrderAsync(dto);

            InvalidateListCache();

            return result;
        }

        /// <summary>
        /// Atualiza pedido e INVALIDA o cache
        /// </summary>
        public async Task<OrderResponseDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var result = await _innerService.UpdateOrderAsync(id, dto);

            if (result != null)
            {
                // Remover este pedido específico do cache
                var cacheKey = $"{ORDER_CACHE_KEY_PREFIX}{id}";
                _cache.Remove(cacheKey);

                // Limpar cache de listas
                InvalidateListCache();
            }

            return result;
        }

        /// <summary>
        /// Obtém métricas (sem cache - sempre atualizado)
        /// </summary>
        public async Task<MetricsDto> GetMetricsAsync()
        {
            // Métricas devem ser sempre atualizadas
            return await _innerService.GetMetricsAsync();
        }

        /// <summary>
        /// Invalida todo o cache de listas
        /// </summary>
        private void InvalidateListCache()
        {

            // Para simplificar, vamos remover as chaves mais comuns
            for (int page = 1; page <= 10; page++)
            {
                for (int pageSize = 10; pageSize <= 100; pageSize += 10)
                {
                    _cache.Remove($"{ORDERS_CACHE_KEY}_{page}_{pageSize}_all");
                }
            }
        }
    }
}