using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class InstrumentTypeEntity
	{
		[Key]
		public int InstrumentTypeID { get; set; }
		public string Name { get; set; }

		public InstrumentTypeEntity(string name)
		{
			Name = name;
		}

		public InstrumentTypeEntity(int instrumentTypeID, string name)
		{
			InstrumentTypeID = instrumentTypeID;
			Name = name;
		}
	}
}
