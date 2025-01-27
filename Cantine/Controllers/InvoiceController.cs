using Application.Invoice.Queries.GetInvoice;
using Cantine.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cantine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoice([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var getInvoiceResult = await _mediator.Send(new GetInvoiceQuery { ClientId = Request.UserId(), OrderId = orderId}, cancellationToken);


            return File(getInvoiceResult.Result.FileContent, "application/pdf", getInvoiceResult.Result.Filename);
        }
    }
}