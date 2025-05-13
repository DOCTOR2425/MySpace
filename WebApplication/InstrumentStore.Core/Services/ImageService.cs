using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class ImageService : IImageService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public ImageService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Image>> GetByProductId(Guid productId)
		{
			return await _dbContext.Image
				.Where(i => i.Product.ProductId == productId)
				.OrderBy(i => i.Index)
				.ToListAsync();
		}

		public async Task<Guid> Create(Image image)
		{
			await _dbContext.Image.AddAsync(image);
			await _dbContext.SaveChangesAsync();

			return image.ImageId;
		}

		public async Task<bool> IsImage(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return false;

			var allowedMimeTypes = new[] { "image/jpg", "image/jpeg", "image/png" };
			if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
				return false;

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
			var fileExtension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(fileExtension))
				return false;

			return true;
		}

		public async Task DeleteImagesByProductId(Guid productId)
		{
			await _dbContext.Image
				.Where(i => i.Product.ProductId == productId)
				.ExecuteDeleteAsync();
		}

		public async Task<Image> GetMainProductImage(Guid productId)
		{
			return await _dbContext.Image
				.FirstOrDefaultAsync(i => i.Product.ProductId == productId &&
					i.Index == 0);
		}
	}
}
