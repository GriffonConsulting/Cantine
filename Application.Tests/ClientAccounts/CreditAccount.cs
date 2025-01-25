using Application.ClientAccounts.Commands.CreditAccount;
using Application.Common.Requests;
using EntityFramework.Commands;
using EntityFramework.Entities;
using Moq;

namespace Application.Tests.ClientAccounts
{
    public class GetClientTests : UnitTestBase
    {
        private readonly CreditAccountCommandHandler _creditAccountCommandHandler;


        public GetClientTests()
        {
            Mock<ClientAccountCommands> clientAccountCommands = new(DbContextMock);
            Mock<ClientAccountTransactionHistoryCommands> clientAccountTransactionHistoryCommands = new(DbContextMock);
            Mock<ClientAccountQueries> clientAccountQueries = new(DbContextMock);
            _creditAccountCommandHandler = new CreditAccountCommandHandler(clientAccountCommands.Object, clientAccountTransactionHistoryCommands.Object, clientAccountQueries.Object);
        }

        [Fact]
        public async Task zero_amount_Should_Throw_BadRequestException()
        {
            var response =  await _creditAccountCommandHandler.Handle(
                new CreditAccountCommand() 
                { 
                    ProviderId = new Guid(), 
                    CreditAccountClientCommand = new CreditClientCommandDto
                    {
                        ClientId = new Guid(),
                        Amount = 0
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task negative_amount_Should_Throw_BadRequestException()
        {
            var response = await _creditAccountCommandHandler.Handle(
                new CreditAccountCommand()
                {
                    ProviderId = new Guid(),
                    CreditAccountClientCommand = new CreditClientCommandDto
                    {
                        ClientId = new Guid(),
                        Amount = -50
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Unknown_client_Should_Throw_BadRequestException()
        {
            var response = await _creditAccountCommandHandler.Handle(
                new CreditAccountCommand()
                {
                    ProviderId = new Guid(),
                    CreditAccountClientCommand = new CreditClientCommandDto
                    {
                        ClientId = new Guid(),
                        Amount = 10
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Should_Be_Ok()
        {
            var clientId = Guid.NewGuid();
            await DbContextMock.ClientAccount.AddAsync(new ClientAccount { Amount = 50, ClientId = clientId });
            await DbContextMock.SaveChangesAsync();


            var response = await _creditAccountCommandHandler.Handle(
                new CreditAccountCommand()
                {
                    ProviderId = new Guid(),
                    CreditAccountClientCommand = new CreditClientCommandDto
                    {
                        ClientId = clientId,
                        Amount = 10
                    }
                }, new CancellationToken());
            Assert.True(response.Result.Amount == 60);
        }
    }
}
