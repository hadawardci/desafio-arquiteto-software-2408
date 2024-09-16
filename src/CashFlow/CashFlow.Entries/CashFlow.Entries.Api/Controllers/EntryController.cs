using CashFlow.Entries.Api.Authorization;
using CashFlow.Entries.Application.AddEntry;
using CashFlow.Entries.Application.ViewEntries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Entries.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = Policies.Entries)]
        public async Task<IActionResult> AddEntryAsync(
            [FromServices] IMediator mediator,
            [FromBody] AddEntryInput input,
            CancellationToken cancellationToken)
        {
            await mediator.Send(input, cancellationToken);
            return NoContent();
        }

        [HttpGet("{Date}")]
        [Authorize(Policy = Policies.Entries)]
        public async Task<IReadOnlyCollection<ViewEntriesOutput>> ViewEntriesAsync(
            [FromServices] IMediator mediator,
            [FromRoute] ViewEntriesInput input,
            CancellationToken cancellationToken) => await mediator.Send(input, cancellationToken);


    }
}
