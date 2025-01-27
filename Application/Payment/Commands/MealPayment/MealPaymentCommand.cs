using Application.Common.Requests;
using MediatR;

namespace Application.Payment.Commands.MealPayment;

public class MealPaymentCommand : IRequest<RequestResult<MealPaymentResult>>
{
    public required Guid ClientId { get; set; }
    public required MealPaymentCommandDto MealPaymentCommandDto { get; set; }
}

public class MealPaymentCommandDto
{
    public required Guid[] ProductIds { get; set; }
}