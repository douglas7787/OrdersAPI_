using Microsoft.AspNetCore.Mvc;
using OrdersAPI.DTOs;
using OrdersAPI.Services;
using FluentValidation;

namespace OrdersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IValidator<CreateOrderDto> _createValidator;
        private readonly IValidator<UpdateOrderDto> _updateValidator;

        public OrdersController(
            IOrderService service,
            IValidator<CreateOrderDto> createValidator,
            IValidator<UpdateOrderDto> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        /// <param name="dto">Dados do pedido</param>
        /// <returns>Pedido criado</returns>
        /// <response code="201">Pedido criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            // VALIDAR ANTES DE CRIAR
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

            var order = await _service.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        /// <summary>
        /// Lista pedidos com paginação
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? customerName = null)
        {
            // Validar paginação
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var orders = await _service.GetOrdersAsync(page, pageSize, customerName);
            return Ok(orders);
        }

        /// <summary>
        /// Busca um pedido por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            var order = await _service.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Pedido não encontrado" });

            return Ok(order);
        }

        /// <summary>
        /// Atualiza o valor total de um pedido
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido" });

            // VALIDAR ANTES DE ATUALIZAR
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

            var order = await _service.UpdateOrderAsync(id, dto);
            if (order == null)
                return NotFound(new { message = "Pedido não encontrado" });

            return Ok(order);
        }
    }
}