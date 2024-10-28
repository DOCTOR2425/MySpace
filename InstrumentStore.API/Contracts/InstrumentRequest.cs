namespace InstrumentStore.API.Contracts
{
	public record InstrumentRequest(
		string Name,
		string Description,
		decimal Price,
		int Quantity,
		Byte[] Image,
		int InstrumentType,
		int Country,
		int Supplier);
}
