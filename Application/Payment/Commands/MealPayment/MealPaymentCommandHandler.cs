using Application.ClientAccounts.Commands.CreditAccount;
using Application.Common.Requests;
using EntityFramework.Commands;
using EntityFramework.Entities;
using MediatR;

namespace Application.Payment.Commands.MealPayment;

public class MealPaymentCommandHandler : IRequestHandler<MealPaymentCommand, RequestResult<MealPaymentResult>>
{

    private readonly ClientAccountCommands _clientAccountCommands;
    private readonly ClientAccountTransactionHistoryCommands _clientAccountTransactionHistoryCommands;
    private readonly ClientAccountQueries _clientAccountQueries;

    public MealPaymentCommandHandler(ClientAccountCommands clientAccountCommands, ClientAccountTransactionHistoryCommands clientAccountTransactionHistoryCommands, ClientAccountQueries clientAccountQueries)
    {
        _clientAccountCommands = clientAccountCommands;
        _clientAccountTransactionHistoryCommands = clientAccountTransactionHistoryCommands;
        _clientAccountQueries = clientAccountQueries;
    }


    public async Task<RequestResult<MealPaymentResult>> Handle(MealPaymentCommand request, CancellationToken cancellationToken)
    {

        return new RequestResult<MealPaymentResult>
        {
            Result = new MealPaymentResult
            {
            },

            StatusCodes = RequestStatusCodes.Status200OK
        };
    }
}
