using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}