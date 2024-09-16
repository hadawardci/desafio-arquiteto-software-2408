using CashFlow.Entries.Api.Authorization;
using CashFlow.Entries.Application.PartialDaily;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Entries.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartialDailyController : ControllerBase
    {
        [HttpGet("{Timestamp:long}")]
        [Authorize(Policy = Policies.PartialDaily)]
        public async Task<IReadOnlyCollection<PartialDailyOutput>> GetDailyPartialsAsync(
            [FromServices] IMediator mediator,
            [FromRoute] PartialDailyInput input,
            CancellationToken cancellationToken) => await mediator.Send(input, cancellationToken);

    }
}
