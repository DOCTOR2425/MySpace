using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
	public class CountryEntity
	{
		[Key]
		public int CountryID { get; set; }
		public string Name { get; set; }

		//public CountryEntity(int countryID, string name)
		//{
		//	CountryID = countryID;
		//	Name = name;
		//}
		public CountryEntity(string name)
		{
			Name = name;
		}
	}
}
