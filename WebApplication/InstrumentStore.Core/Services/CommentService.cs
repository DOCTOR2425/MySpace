using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace InstrumentStore.Domain.Services
{
	public class CommentService : ICommentService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public CommentService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Guid> CreateCommentToProduct(string commentText, Guid productId, Guid userId)
		{
			Product? product = await _dbContext.Product
				.FindAsync(productId);
			User? user = await _dbContext.User
				.FindAsync(userId);

			if (product == null || user == null)
				throw new ArgumentNullException($"Нет таких продукта или юзера:\n{productId}\n{userId}");

			if (user.BlockDate != null)
				throw new AuthenticationException("Запрещено оставлять комментарии по причине бана");

			if (commentText.Any() == false || commentText.Length > 1000)
				new ArgumentNullException("неверный текст");

			Comment comment = new Comment()
			{
				CommentId = Guid.NewGuid(),
				Text = commentText.Trim(),
				CreationDate = DateTime.Now,
				Product = product,
				User = user
			};

			await _dbContext.Comment.AddAsync(comment);
			await _dbContext.SaveChangesAsync();

			return comment.CommentId;
		}

		public async Task<List<Comment>> GetAllCommentsByProduct(Guid productId)
		{
			return await _dbContext.Comment
				.Include(c => c.Product)
				.Include(c => c.User)
				.Where(c => c.Product.ProductId == productId)
				.OrderByDescending(c => c.CreationDate)
				.ToListAsync();
		}

		public async Task<List<Comment>> GetCommentsByUser(Guid userId)
		{
			return await _dbContext.Comment
				.Include(c => c.User)
				.Include(c => c.Product)
				.Include(c => c.Product.Brand)
				.Include(c => c.Product.Country)
				.Include(c => c.Product.ProductCategory)
				.Where(c => c.User.UserId == userId)
				.OrderByDescending(c => c.CreationDate)
				.ToListAsync();
		}
	}
}
