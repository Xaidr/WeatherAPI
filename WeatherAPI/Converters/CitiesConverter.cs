using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Классы необходимые для преобразования из JSON в C#
//Эти классы относятся к преобразованию файла city.list.json и получения коллекции городов 
namespace WeatherAPI.Converters
{
	public class CityCities
	{
		public double id { get; set; }
		public string name { get; set; }
		public string state { get; set; }
		public string country { get; set; }
		public CoordCities coord { get; set; }
	}

	public class CoordCities
	{
		public double lon { get; set; }
		public double lat { get; set; }
	}

	public class RootCities
	{
		public List<CityCities> city { get; set; }
	}

}
