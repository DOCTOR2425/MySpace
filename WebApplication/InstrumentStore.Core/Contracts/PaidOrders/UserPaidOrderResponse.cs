namespace InstrumentStore.Domain.Contracts.PaidOrders
{
    public class UserPaidOrderResponse
    {
        public Guid PaidOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReceiptDate { get; set; }
        public List<PaidOrderItemResponse> PaidOrderItems { get; set; }
    }
}
