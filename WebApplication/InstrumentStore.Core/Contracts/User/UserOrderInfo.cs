namespace InstrumentStore.Domain.Contracts.User
{
    public class UserOrderInfo
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserDeliveryAddress? UserDeliveryAddress { get; set; }

        public UserOrderInfo()
        {

        }

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

            UserDeliveryAddress = new UserDeliveryAddress();
            UserDeliveryAddress.City = city;
            UserDeliveryAddress.Street = street;
            UserDeliveryAddress.HouseNumber = houseNumber;
            UserDeliveryAddress.Entrance = entrance;
            UserDeliveryAddress.Flat = flat;
        }
    }
}
