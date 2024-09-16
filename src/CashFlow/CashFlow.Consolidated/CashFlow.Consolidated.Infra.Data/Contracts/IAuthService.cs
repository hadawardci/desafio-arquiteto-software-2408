namespace CashFlow.Consolidated.Infra.Data.Contracts
{
    public interface IAuthService
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken);
    }
}
