using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IAdminService
	{
		Task SendAdminMailAboutOrder(Guid paidOrderId);
		Task<bool> IsAdminEmail(string email);
		Task<bool> IsAdminId(Guid Id);
	}
}