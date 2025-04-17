using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface ICommentService
    {
        Task<Guid> CreateCommentToProduct(string commentText, Guid productId, Guid userId);
        Task<List<Comment>> GetAllCommentsByProduct(Guid productId);
        Task<List<Comment>> GetCommentsByUser(Guid userId);
    }
}