using CashFlow.Consolidated.Domain.SharedKernel.Outputs;
using Refit;

namespace CashFlow.Consolidated.Infra.Data.Contracts
{
    public interface IEntriesApi
    {
        [Get("/api/partial-daily/{timeUtc}")]
        [Headers("accept: application/json",
         "Authorization: Bearer")]
        Task<ICollection<PartialDailyOutput>> GetPartialDaily([AliasAs("timeUtc")] long timeUtc);
    }
}
