using Application.Common.Requests;
using Application.Payment.Commands.MealPayment;
using Cantine.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cantine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("PayMeal", Name = "PayMeal")]
        [ProducesResponseType(typeof(RequestResult), RequestStatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RequestResult), RequestStatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> PayMeal(MealPaymentCommandDto mealPaymentCommandDto, CancellationToken cancellationToken)
        {
            var creditAccountResult = await _mediator.Send(new MealPaymentCommand { ClientId = Request.UserId(), MealPaymentCommandDto = mealPaymentCommandDto }, cancellationToken);

            return new ObjectResult(creditAccountResult)
            {
                StatusCode = creditAccountResult.StatusCodes
            };
        }
    }
}