namespace InstrumentStore.Domain.Models
{
	public class Country
	{
		public int CountryID { get; set; }
		public string Name { get; set; }

		public Country(int countryID, string name)
		{
			CountryID = countryID;
			Name = name;
		}
	}
}
