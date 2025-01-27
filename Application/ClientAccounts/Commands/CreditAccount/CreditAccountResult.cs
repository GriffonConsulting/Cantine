namespace Application.ClientAccounts.Commands.CreditAccount;

public class CreditAccountResult
{
    public required Guid ClientId { get; set; }
    public required decimal Amount { get; set; }
}
