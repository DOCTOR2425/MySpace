using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Contracts.Cart
{
	public record OrderOptionsResponse(
		List<DeliveryMethod> DeliveryMethods,
		List<string> PaymentMethods
	);
}
