namespace InstrumentStore.Domain.Contracts.User
{
	public class UserOrderInfo
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string HouseNumber { get; set; }
		public string Entrance { get; set; }
		public string Flat { get; set; }

		public UserOrderInfo() { }

		public UserOrderInfo(
			Guid userId,
			string firstName,
			string telephone,
			string eMail,
			string city,
			string street,
			string houseNumber,
			string entrance,
			string flat)
		{
			UserId = userId;
			FirstName = firstName;
			Telephone = telephone;
			Email = eMail;
			City = city;
			Street = street;
			HouseNumber = houseNumber;
			Entrance = entrance;
			Flat = flat;
		}
	}
}
