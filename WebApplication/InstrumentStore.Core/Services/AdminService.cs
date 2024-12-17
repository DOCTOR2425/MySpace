using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;

namespace InstrumentStore.Domain.Services
{
    public class AdminService
    {
        private InstrumentStoreDBContext _dbContext;
        private IUsersService _usersService;
        private IProductService _productService;
        private IPaidOrderService _paidOrderService;
        private IDeliveryMethodService _eliveryMethodService;
        private IPaymentMethodService _paymentMethodService;
        private IMapper _mapper;

        public AdminService(InstrumentStoreDBContext dbContext, 
            IUsersService usersService, 
            IProductService productService, 
            IPaidOrderService paidOrderService, 
            IDeliveryMethodService eliveryMethodService, 
            IPaymentMethodService paymentMethodService, 
            IMapper mapper)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _eliveryMethodService = eliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _mapper = mapper;
        }

        
    }
}
