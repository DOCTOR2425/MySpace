namespace InstrumentStore.Domain.Contracts.User
{
    public class UserProfileResponse
    {
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string Entrance { get; set; } = string.Empty;
        public string Flat { get; set; } = string.Empty;

        public int CommentNumber { get; set; } = 0;
        public int OrderNumber { get; set; } = 0;
        public int PendingReviewNumber { get; set; } = 0;
    }
}
