﻿using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IImageService
	{
		Task<Guid> Create(Image image);
		Task<List<Image>> GetByProductId(Guid productId);
		Task<bool> IsImage(IFormFile file);
		Task DeleteImagesByProductId(Guid productId);
		Task<Image> GetMainProductImage(Guid productId);
	}
}