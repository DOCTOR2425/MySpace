using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.Extensions.Configuration;

namespace InstrumentStore.Domain.Services
{
    public class AdminService : IAdminService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IProductService _productService;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AdminService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService,
            IDeliveryMethodService eliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IMapper mapper,
            IEmailService emailService,
            IConfiguration config)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _deliveryMethodService = eliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _mapper = mapper;
            _emailService = emailService;
            _config = config;
        }

        public async Task SendAdminMailAboutOrder(Guid paidOrderId)
        {
            PaidOrder paidOrder = await _paidOrderService.GetById(paidOrderId);
            List<PaidOrderItem> paidOrderItems = await _paidOrderService.GetAllItemsByOrder(paidOrderId);

            string mailText = string.Empty;

            mailText += $"Заказ от {paidOrder.PaymentDate}\n";
            mailText += $"Клиент - {paidOrder.User.Surname} {paidOrder.User.FirstName} {paidOrder.User.Patronymic}\n";
            mailText += $"Телефон клиента - {paidOrder.User.Telephone}\n";
            mailText += $"Способ оплаты - {paidOrder.PaymentMethod.Name}\n";
            mailText += $"Способ доставки - {paidOrder.DeliveryMethod.Name} (стоимость - {paidOrder.DeliveryMethod.Price})\n";

            if (await _deliveryMethodService.IsHomeDelivery(paidOrder.DeliveryMethod.DeliveryMethodId))
                mailText += $"Адрес клиента - {paidOrder.User.UserAdress.ToString()}\n";

            mailText += "\n\n   ---- Заказанные товары ----\n";
            decimal summaryPrice = 0;

            foreach (var item in paidOrderItems)
            {
                summaryPrice += item.Product.Price * item.Quantity;

                mailText += $"{item.Product.Name} x {item.Quantity}  -  " +
                    $"{item.Product.Price * item.Quantity}р.\n";
            }

            mailText += $"\nОбщая сумма заказанных товаров: {summaryPrice}.р";
            mailText += $"\nОбщая сумма с доставкой: {summaryPrice + paidOrder.DeliveryMethod.Price}.р";

            _emailService.SendMail(_config.GetValue<string>("MySpaceMail"), mailText, "Новый заказ");
        }
    }
}
