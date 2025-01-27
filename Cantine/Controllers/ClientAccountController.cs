using Application.ClientAccounts.Commands.CreditAccount;
using Application.Common.Requests;
using Cantine.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cantine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientAccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Credit", Name = "CreditAccount")]
        [ProducesResponseType(typeof(RequestResult<CreditAccountResult>), RequestStatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RequestResult<CreditAccountResult>), RequestStatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreditAccount(CreditClientCommandDto creditAccountClientCommand, CancellationToken cancellationToken)
        {
            var creditAccountResult = await _mediator.Send(new CreditAccountCommand { UserId = Request.UserId(), CreditAccountClientCommand = creditAccountClientCommand }, cancellationToken);

            return new ObjectResult(creditAccountResult)
            {
                StatusCode = creditAccountResult.StatusCodes
            };
        }
    }
}