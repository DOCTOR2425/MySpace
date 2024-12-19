namespace InstrumentStore.Domain.Abstractions
{
    public interface IAdminService
    {
        Task SendAdminMailAboutOrder(Guid paidOrderId);
    }
}