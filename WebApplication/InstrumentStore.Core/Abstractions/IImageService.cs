using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IImageService
	{
		Task<Guid> Create(Image image);
		Task<City> GetById(Guid cityId);
		Task<List<Image>> GetByProductId(Guid productId);
		Task<bool> IsImage(IFormFile file);
	}
}