namespace InstrumentStore.Domain.Models
{
	public class InstrumentType
	{
		public int InstrumentTypeID { get; set; }
		public string Name { get; set; }

		public InstrumentType(int instrumentTypeID, string name)
		{
			InstrumentTypeID = instrumentTypeID;
			Name = name;
		}
	}
}
