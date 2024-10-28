using InstrumentStore.Application.Services;
using InstrumentStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class InstrumentController : ControllerBase// TODO 
	{
		private readonly InstrumentServices _instrumentServices;

		public InstrumentController()
		{
			_instrumentServices = new InstrumentServices();
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<List<Instrument>>> GetSimilarInstrument(int id)
		{
			Instrument target = await _instrumentServices.GetInstrument(id);

			IEnumerable<Instrument> list = (await _instrumentServices.GetAllInstruments())
				.Where(x => x.Type == target.Type);

			return Ok(list);
		}
		
	}
}
