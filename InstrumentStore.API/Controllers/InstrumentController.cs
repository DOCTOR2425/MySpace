using InstrumentStore.Application.Services;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class InstrumentController : ControllerBase
	{
		//private readonly InstrumentServices _instrumentServices;

		//public InstrumentController()
		//{
		//	_instrumentServices = new InstrumentServices();
		//}

		//[HttpGet("{id:int}")]
		//public ActionResult<List<Product>> GetSimilarInstrument(int id)
		//{
		//	Product target = _instrumentServices.Get(id);

		//	IEnumerable<Product> list = _instrumentServices.GetAll()
		//		.Where(x => x.Type == target.Type);

		//	return Ok(list);
		//}
		
	}
}
