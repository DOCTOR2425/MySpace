using InstrumentStore.API.Contracts;
using InstrumentStore.Application.Services;
using InstrumentStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class MainCintroller : ControllerBase
	{
		private readonly InstrumentServices _instrumentServices;

		public MainCintroller()
		{
			_instrumentServices = new InstrumentServices();
		}

		[HttpGet]
		public async Task<ActionResult<List<InstrumentResponse>>> GetInstruments()
		{
			List<Instrument> instruments = await _instrumentServices.GetAllInstruments();

			IEnumerable<InstrumentResponse> response = instruments.Select(x => new InstrumentResponse(
				x.InstrumentID,
				x.Name,
				x.Description,
				x.Price,
				x.Quantity,
				x.Image,
				x.Type,
				x.Country,
				x.Supplier));

			return Ok(response);
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<InstrumentResponse>> GetInstrument(int id)
		{
			Instrument instrument = await _instrumentServices.GetInstrument(id);

			InstrumentResponse response = new InstrumentResponse(
				instrument.InstrumentID,
				instrument.Name,
				instrument.Description,
				instrument.Price,
				instrument.Quantity,
				instrument.Image,
				instrument.Type,
				instrument.Country,
				instrument.Supplier);

			return Ok(response);
		}

		[HttpPost]
		public async Task<ActionResult<int>> CreateInstrument([FromBody] InstrumentRequest instrument)
		{
			Instrument inst = new Instrument(
				0,
				instrument.Name,
				instrument.Description,
				instrument.Price,
				instrument.Quantity,
				instrument.Image,
				instrument.InstrumentType,
				instrument.Country,
				instrument.Supplier);

			return Ok(await _instrumentServices.CreateInstrument(inst));
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult<int>> UpdateInstrument(int id, [FromBody] InstrumentRequest instrument)
		{
			int instrumentId = await _instrumentServices.UpdateInstrument(
				id,
				instrument.Name,
				instrument.Description,
				instrument.Price,
				instrument.Quantity,
				instrument.Image,
				instrument.InstrumentType,
				instrument.Country,
				instrument.Supplier);

			return Ok(instrumentId);
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult<int>> DeleteInstrument(int id)
		{
			return Ok(await _instrumentServices.DeleteInstrument(id));
		}
	}
}
