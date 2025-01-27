using Application.ClientAccounts.Commands.CreditAccount;
using Application.Common.Requests;
using Application.Payment.Commands.MealPayment;
using EntityFramework.Commands;
using EntityFramework.Entities;
using EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.ClientAccounts
{
    public class MealPaymentTests : UnitTestBase
    {
        private readonly MealPaymentCommandHandler _creditAccountCommandHandler;
        private readonly Guid _clientId;

        public MealPaymentTests()
        {
            Mock<ClientAccountCommands> clientAccountCommands = new(DbContextMock);
            Mock<ClientAccountTransactionHistoryCommands> clientAccountTransactionHistoryCommands = new(DbContextMock);
            Mock<ClientAccountQueries> clientAccountQueries = new(DbContextMock);
            Mock<ClientQueries> clientQueries = new(DbContextMock);
            Mock<ProductQueries> productQueries = new(DbContextMock);
            Mock<OrderCommands> orderCommands = new(DbContextMock);
            Mock<OrderContentCommands> orderContentCommands = new(DbContextMock);
            _clientId = Guid.NewGuid();
            Init();
            _creditAccountCommandHandler = new MealPaymentCommandHandler(clientAccountCommands.Object, clientAccountTransactionHistoryCommands.Object, clientAccountQueries.Object, clientQueries.Object, productQueries.Object, orderCommands.Object, orderContentCommands.Object);
        }

        public async void Init()
        {
            await DbContextMock.ClientAccount.AddAsync(new ClientAccount { Amount = 5, ClientId = _clientId });

            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Plateau", ProductPrice = 10 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Boisson", ProductPrice = 1 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Fromage", ProductPrice = 1 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Pain", ProductPrice = 0.4M });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Petit Salade Bar", ProductPrice = 4 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Grand Salade Bar", ProductPrice = 6 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Portion de fruit", ProductPrice = 1 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Entrée supplementaire", ProductPrice = 3 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Plat supplementaire", ProductPrice = 6 });
            await DbContextMock.Product.AddAsync(new Product { Id = Guid.NewGuid(), ProductName = "Dessert supplementaire", ProductPrice = 3 });

            await DbContextMock.Role.AddAsync(new Role { Id = Guid.NewGuid(), ClientRole = Domain.Client.ClientRole.Intern, CanOverDraft = true, MealCareAmount = 7.5M  });
            await DbContextMock.Role.AddAsync(new Role { Id = Guid.NewGuid(), ClientRole = Domain.Client.ClientRole.Provider, CanOverDraft = false, MealCareAmount =6M });
            await DbContextMock.Role.AddAsync(new Role { Id = Guid.NewGuid(), ClientRole = Domain.Client.ClientRole.Vip, CanOverDraft = true, MealCarePercent = 100 });
            await DbContextMock.Role.AddAsync(new Role { Id = Guid.NewGuid(), ClientRole = Domain.Client.ClientRole.Trainee, CanOverDraft = false, MealCareAmount = 10M });
            await DbContextMock.Role.AddAsync(new Role { Id = Guid.NewGuid(), ClientRole = Domain.Client.ClientRole.Visitor, CanOverDraft = false });
            await DbContextMock.SaveChangesAsync();

        }

        [Fact]
        public async Task Unknown_client_Should_Throw_BadRequestException()
        {
            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = Guid.NewGuid(),
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = []
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }


        [Fact]
        public async Task Unknown_product_Should_Throw_BadRequestException()
        {
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "" });
            await DbContextMock.SaveChangesAsync();

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [ Guid.NewGuid() ]
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Should_Throw_BadRequestException()
        {
            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [Guid.NewGuid()]
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }
        

        [Fact]
        public async Task Should_Be_Ok()
        {
            var role = await DbContextMock.Role.FirstOrDefaultAsync(p => p.ClientRole == Domain.Client.ClientRole.Intern);
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "", Role = role });
            await DbContextMock.SaveChangesAsync();
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [plateau.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 10 && response.Result.ClientAmount == 2.5M && response.Result.CareAmount == 7.5M);
        }

        [Fact]
        public async Task Intern_With_Overdraft_Should_Be_Ok()
        {
            var role = await DbContextMock.Role.FirstOrDefaultAsync(p => p.ClientRole == Domain.Client.ClientRole.Intern);
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "", Role = role });
            await DbContextMock.SaveChangesAsync();
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var grandSaladeBar = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Grand Salade Bar");

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [plateau.Id, boisson.Id, grandSaladeBar.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 17 && response.Result.ClientAmount == 9.5M && response.Result.CareAmount == 7.5M);
        }

        [Fact]
        public async Task Provider_With_Overdraft_Should_Throw_BusinessException()
        {
            var role = await DbContextMock.Role.FirstOrDefaultAsync(p => p.ClientRole == Domain.Client.ClientRole.Provider);
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "", Role = role });
            await DbContextMock.SaveChangesAsync();
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var pain = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Pain");

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [plateau.Id, boisson.Id, pain.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Vip_Should_Be_Ok()
        {
            var role = await DbContextMock.Role.FirstOrDefaultAsync(p => p.ClientRole == Domain.Client.ClientRole.Vip);
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "", Role = role });
            await DbContextMock.SaveChangesAsync();
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var grandSaladeBar = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Grand Salade Bar");

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [plateau.Id, boisson.Id, grandSaladeBar.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 17 && response.Result.ClientAmount == 0 && response.Result.CareAmount == 17);
        }

        [Fact]
        public async Task Visitor_Should_Be_Ok()
        {
            var role = await DbContextMock.Role.FirstOrDefaultAsync(p => p.ClientRole == Domain.Client.ClientRole.Visitor);
            await DbContextMock.Client.AddAsync(new Client { Id = _clientId, Email = "", Role = role });
            await DbContextMock.SaveChangesAsync();
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");

            var response = await _creditAccountCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = _clientId,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [boisson.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 1 && response.Result.ClientAmount == 1 && response.Result.CareAmount == 0);
        }
    }
}