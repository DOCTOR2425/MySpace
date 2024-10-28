using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class CountryEntity
	{
		[Key]
		public int CountryID { get; set; }
		public string Name { get; set; }

		public CountryEntity(string name)
		{
			Name = name;
		}

		public CountryEntity(int countryID, string name)
		{
			CountryID = countryID;
			Name = name;
		}
	}
}
