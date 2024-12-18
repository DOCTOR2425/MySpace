using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IUsersService _usersService;
        private IProductService _productService;
        private IPaidOrderService _paidOrderService;
        private IDeliveryMethodService _deliveryMethodService;
        private IPaymentMethodService _paymentMethodService;
        private IMapper _mapper;

        public AdminController(IUsersService usersService, 
            IProductService productService, 
            IPaidOrderService paidOrderService, 
            IDeliveryMethodService deliveryMethodService, 
            IPaymentMethodService paymentMethodService, 
            IMapper mapper)
        {
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _mapper = mapper;
        }

        [HttpPost("create-delivery-method")]
        public async Task<ActionResult<Guid>> CreateDeliveryMethod(
            [FromBody] CreateDeliveryMethodRequest request)
        {
            DeliveryMethod deliveryMethod = new DeliveryMethod()
            {
                DeliveryMethodId = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price
            };

            return Ok(await _deliveryMethodService.Create(deliveryMethod));
        }

        [HttpPost("create-payment-method")]
        public async Task<ActionResult<Guid>> CreatePaymentMethod([FromBody] string paymentMethodName)
        {
            PaymentMethod paymentMethod = new PaymentMethod()
            {
                PaymentMethodId = Guid.NewGuid(),
                Name = paymentMethodName
            };

            return Ok(await _paymentMethodService.Create(paymentMethod));
        }


    }
}
