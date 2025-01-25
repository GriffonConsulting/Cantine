using Application.Common.Requests;
using MediatR;

namespace Application.ClientAccounts.Commands.CreditAccount;

public class CreditAccountCommand : IRequest<RequestResult<CreditAccountResult>>
{
    public required Guid ProviderId { get; set; }
    public required CreditClientCommandDto CreditAccountClientCommand { get; set; }
}

public class CreditClientCommandDto
{
    public required Guid ClientId { get; set; }
    public decimal Amount { get; set; }

}