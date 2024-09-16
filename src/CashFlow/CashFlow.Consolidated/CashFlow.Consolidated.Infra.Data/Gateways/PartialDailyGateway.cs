using CashFlow.Consolidated.Domain.SharedKernel.Outputs;
using CashFlow.Consolidated.Infra.Data.Contracts;
namespace CashFlow.Consolidated.Infra.Data.Gateways
{
    public class PartialDailyGateway(IEntriesApi _entriesApi): IPartialDailyGateway
    {
        public async Task<ICollection<PartialDailyOutput>> GetPartialsDaily(long timestamp) 
            => await _entriesApi.GetPartialDaily(timestamp);

    }
}
