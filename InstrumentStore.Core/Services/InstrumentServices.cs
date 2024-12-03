using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Application.Services
{
    public class InstrumentServices
    {
        private static readonly InstrumentStoreDBContext db;

        static InstrumentServices()
        {
            db = new InstrumentStoreDBContext();
        }

        public List<Instrument> GetAll()
        {
            return db.Instrument.ToList();
        }

        public Instrument Get(int Id)
        {
            Instrument? Instrument = db.Instrument.ToList().FirstOrDefault(x => x.InstrumentId == Id);
            if (Instrument == null)
                throw new ArgumentException("Не существеут инструмента с id " + Id);

            return Instrument;
        }

        public int Create(Instrument Instrument)
        {
            if (Instrument == null || IsFieldsFalId(Instrument) == false)
                throw new ArgumentException("Данные об инструменте не заполненны или заполненны неправильно");

            db.Instrument.Add(Instrument);
            db.SaveChanges();

            return Instrument.InstrumentId;
        }

        public int Delete(int Id)
        {
            Instrument? Instrument = db.Instrument.ToList().FirstOrDefault(x => x.InstrumentId == Id);
            if (Instrument == null)
                throw new ArgumentException("Не существеут инструмента с id " + Id);

            db.Instrument.Remove(Instrument);
            db.SaveChanges();

            return Instrument.InstrumentId;
        }

        public int Update(int Id, string name,
            string description, decimal price, int quantity,
            byte[] image, int type, int country, int supplier)
        {
            Instrument? Instrument = db.Instrument.ToList().FirstOrDefault(x => x.InstrumentId == Id);

            if (IsFieldsFalId(Instrument) == false)
                throw new ArgumentException("Данные для изменения заполненны неверно");

            Instrument.Name = name;
            Instrument.Description = description;
            Instrument.Price = price;
            Instrument.Quantity = quantity;
            Instrument.Image = image;
            Instrument.Type = type;
            Instrument.CountryId = country;
            Instrument.SupplierId = supplier;

            db.SaveChanges();

            return Id;
        }

        private bool IsFieldsFalId(Instrument Instrument)
        {
            if (Instrument == null)
                return false;

            if (Instrument.Name == null ||
                Instrument.Name.Length == 0)
                return false;

            if (Instrument.Description == null ||
                Instrument.Description.Length == 0)
                return false;

            if (Instrument.Image == null ||
                Instrument.Image.Length == 0)
                return false;

            if (Instrument.Price <= 0)
                return false;

            return true;
        }
    }
}
