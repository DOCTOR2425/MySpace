using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
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
