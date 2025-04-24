namespace InstrumentStore.Domain.Abstractions
{
    public interface IAccountService
    {
        Task<string> LoginFirstStage(string email);
        Task<string> LoginSecondStage(string email, string code, string codeHash);
        Task<string> GenerateCodeHas(string code);
        Task<bool> VerifyCode(string token, string tokenHash);
        Task<string> AdminLoginSecondStage(string email, string code, string codeHash);
    }
}