using Application.Common.Requests;
using EntityFramework.Commands;
using EntityFramework.Entities;
using MediatR;

namespace Application.ClientAccounts.Commands.CreditAccount;

public class CreditAccountCommandHandler : IRequestHandler<CreditAccountCommand, RequestResult<CreditAccountResult>>
{

    private readonly ClientAccountCommands _clientAccountCommands;
    private readonly ClientAccountTransactionHistoryCommands _clientAccountTransactionHistoryCommands;
    private readonly ClientAccountQueries _clientAccountQueries;

    public CreditAccountCommandHandler(ClientAccountCommands clientAccountCommands, ClientAccountTransactionHistoryCommands clientAccountTransactionHistoryCommands, ClientAccountQueries clientAccountQueries)
    {
        _clientAccountCommands = clientAccountCommands;
        _clientAccountTransactionHistoryCommands = clientAccountTransactionHistoryCommands;
        _clientAccountQueries = clientAccountQueries;
    }


    public async Task<RequestResult<CreditAccountResult>> Handle(CreditAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.CreditAccountClientCommand.Amount <= 0) return new RequestResult<CreditAccountResult> { Message = "The credited amount must be greater than zero", StatusCodes = RequestStatusCodes.Status400BadRequest };

        var clientAccount = await _clientAccountQueries.GetByClientId(request.CreditAccountClientCommand.ClientId);

        if (clientAccount == null) return new RequestResult<CreditAccountResult> { Message = "The client account doesn't exist", StatusCodes = RequestStatusCodes.Status400BadRequest };

        clientAccount.Amount += request.CreditAccountClientCommand.Amount;
        clientAccount.ModifiedOn = DateTime.UtcNow;

        await _clientAccountCommands.UpdateEntityAsync(clientAccount, cancellationToken);
        await _clientAccountTransactionHistoryCommands.AddAsync(new ClientAccountTransactionHistory
            {
                TransactionAmount = request.CreditAccountClientCommand.Amount,
                ClientNewAmount = clientAccount.Amount,
                ClientId = request.CreditAccountClientCommand.ClientId,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                ProviderId = request.ProviderId,
            }, cancellationToken);

        return new RequestResult<CreditAccountResult> {
            Result = new CreditAccountResult
            { 
                Amount = clientAccount.Amount,
                ClientId = request.CreditAccountClientCommand.ClientId,
            },

            StatusCodes = RequestStatusCodes.Status200OK  
        };
    }
}
