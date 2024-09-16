using CashFlow.Consolidated.Domain.SharedKernel.Outputs;

namespace CashFlow.Consolidated.Domain.Gateways
{
    public interface IPartialDailyGateway
    {
        Task<ICollection<PartialDailyOutput>> GetPartialsDaily(long timestamp);
    }
}
