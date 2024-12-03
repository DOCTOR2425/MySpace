namespace InstrumentStore.Domain.DataBase.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FIO { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;

        //public Customer(string fio, string telephone, string eMail)
        //{
        //    FIO = fio;
        //    Telephone = telephone;
        //    EMail = eMail;
        //}
    }
}
