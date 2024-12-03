namespace InstrumentStore.Domain.DataBase.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        //public Supplier(int supplierId, string name, string phone, string email)
        //{
        //    SupplierId = supplierId;
        //    Name = name;
        //    Phone = phone;
        //    Email = email;
        //}
    }
}
