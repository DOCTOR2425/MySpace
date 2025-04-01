using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaidOrderController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IMapper _mapper;
        private readonly InstrumentStoreDBContext _dbContext;

        public PaidOrderController(
            ICartService cartService,
            IJwtProvider jwtProvider,
            IMapper mapper,
            IPaidOrderService paidOrderService,
            InstrumentStoreDBContext dBContext)
        {
            _cartService = cartService;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _paidOrderService = paidOrderService;
            _dbContext = dBContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<PaidOrderItem>>> GetUserCart()
        {
            return Ok(await _cartService.GetAllOrders(
                await _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName])));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-order-by-id/{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            PaidOrder order = await _paidOrderService.GetById(id);

            return Ok(_mapper.Map<AdminPaidOrderResponse>(order,
                    opt => opt.Items["DbContext"] = _dbContext));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page)
        {
            List<PaidOrder> orders = await _paidOrderService.GetAll(page);
            List<AdminPaidOrderResponse> response = new List<AdminPaidOrderResponse>();

            foreach (PaidOrder order in orders)
            {
                response.Add(_mapper.Map<AdminPaidOrderResponse>(order,
                    opt => opt.Items["DbContext"] = _dbContext));
            }

            return Ok(response);
        }
    }
}
