using CashFlow.Consolidated.Api.Authorization;
using CashFlow.Consolidated.Application.ViewConsolidatedDaily;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Consolidated.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsolidatedDailyController : ControllerBase
    {
        [HttpGet()]
        [Authorize(Policy = Policies.ConsolidatedDaily)]
        public async Task<IReadOnlyCollection<ViewConsolidatedDailyOutput>> ViewEntriesAsync(
            [FromServices] IMediator mediator,
            [FromQuery] ViewConsolidatedDailyInput input,
            CancellationToken cancellationToken) => await mediator.Send(input, cancellationToken);
    }
}
