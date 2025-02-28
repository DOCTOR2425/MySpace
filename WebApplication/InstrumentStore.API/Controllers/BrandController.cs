using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BrandController : ControllerBase
	{
		private readonly IBrandService _brandService;

		public BrandController(IBrandService brandService)
		{
			_brandService = brandService;
		}

		[HttpGet]
		public async Task<ActionResult<List<Brand>>> GetAllBrands()
		{
			return await _brandService.GetAll();
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> CreateBrand([FromBody] string brandName)
		{
			Brand brand = new Brand
			{
				BrandId = Guid.NewGuid(),
				Name = brandName
			};

			return Ok(await _brandService.Create(brand));
		}
	}
}
