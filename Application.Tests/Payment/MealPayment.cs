using Application.Common.Requests;
using Application.Payment.Commands.MealPayment;
using Domain.Client;
using Domain.Product;
using EntityFramework.Commands;
using EntityFramework.Entities;
using EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.ClientAccounts
{
    public class MealPaymentTests : UnitTestBase
    {
        private readonly MealPaymentCommandHandler _mealPaymentCommandHandler;

        public MealPaymentTests()
        {
            Mock<ClientAccountCommands> clientAccountCommands = new(DbContextMock);
            Mock<ClientAccountTransactionHistoryCommands> clientAccountTransactionHistoryCommands = new(DbContextMock);
            Mock<ClientAccountQueries> clientAccountQueries = new(DbContextMock);
            Mock<ClientQueries> clientQueries = new(DbContextMock);
            Mock<ProductQueries> productQueries = new(DbContextMock);
            Mock<OrderCommands> orderCommands = new(DbContextMock);
            Mock<OrderContentCommands> orderContentCommands = new(DbContextMock);
            Init();
            _mealPaymentCommandHandler = new MealPaymentCommandHandler(clientAccountCommands.Object, clientAccountTransactionHistoryCommands.Object, clientAccountQueries.Object, clientQueries.Object, productQueries.Object, orderCommands.Object, orderContentCommands.Object);
        }

        internal void Init()
        {
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Plateau", ProductPrice = 10, ProductType = ProductType.Other, ProductCode = "MealTray" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Boisson", ProductPrice = 1, ProductType = ProductType.Beverage, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Fromage", ProductPrice = 1, ProductType = ProductType.Other, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Pain", ProductPrice = 0.4M, ProductType = ProductType.Bread, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Petit Salade Bar", ProductPrice = 4, ProductType = ProductType.Other, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Grand Salade Bar", ProductPrice = 6, ProductType = ProductType.Other, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Portion de fruit", ProductPrice = 1, ProductType = ProductType.Other, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Entrée supplementaire", ProductPrice = 3, ProductType = ProductType.Starter, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Plat supplementaire", ProductPrice = 6, ProductType = ProductType.MainDish, ProductCode = "" });
            DbContextMock.Product.Add(new Product { Id = Guid.NewGuid(), ProductName = "Dessert supplementaire", ProductPrice = 3, ProductType = ProductType.Dessert, ProductCode = "" });

            var intern = DbContextMock.Role.Add(new Role { Id = Guid.NewGuid(), ClientRole = ClientRole.Intern, CanOverDraft = true, MealCareAmount = 7.5M });
            var provider = DbContextMock.Role.Add(new Role { Id = Guid.NewGuid(), ClientRole = ClientRole.Provider, CanOverDraft = false, MealCareAmount = 6M });
            var vip = DbContextMock.Role.Add(new Role { Id = Guid.NewGuid(), ClientRole = ClientRole.Vip, CanOverDraft = true, MealCarePercent = 100 });
            var trainee = DbContextMock.Role.Add(new Role { Id = Guid.NewGuid(), ClientRole = ClientRole.Trainee, CanOverDraft = false, MealCareAmount = 10M });
            var visitor = DbContextMock.Role.Add(new Role { Id = Guid.NewGuid(), ClientRole = ClientRole.Visitor, CanOverDraft = false });

            var internId = Guid.NewGuid();
            DbContextMock.Client.Add(new Client { Id = internId, Email = "", ClientRole = ClientRole.Intern, Role = intern.Entity });
            DbContextMock.ClientAccount.Add(new ClientAccount { Amount = 5, ClientId = internId, });

            var providerId = Guid.NewGuid();
            DbContextMock.Client.Add(new Client { Id = providerId, Email = "", ClientRole = ClientRole.Provider, Role = provider.Entity });
            DbContextMock.ClientAccount.Add(new ClientAccount { Amount = 5, ClientId = providerId, });

            var vipId = Guid.NewGuid();
            DbContextMock.Client.Add(new Client { Id = vipId, Email = "", ClientRole = ClientRole.Vip, Role = vip.Entity });
            DbContextMock.ClientAccount.Add(new ClientAccount { Amount = 5, ClientId = vipId, });

            var traineeId = Guid.NewGuid();
            DbContextMock.Client.Add(new Client { Id = traineeId, Email = "", ClientRole = ClientRole.Trainee, Role = trainee.Entity });
            DbContextMock.ClientAccount.Add(new ClientAccount { Amount = 5, ClientId = traineeId, });

            var visitorId = Guid.NewGuid();
            DbContextMock.Client.Add(new Client { Id = visitorId, Email = "", ClientRole = ClientRole.Visitor, Role = visitor.Entity });
            DbContextMock.ClientAccount.Add(new ClientAccount { Amount = 5, ClientId = visitorId, });
            DbContextMock.SaveChanges();
        }

        [Fact]
        public async Task Unknown_client_Should_Throw_BadRequestException()
        {
            var response = await _mealPaymentCommandHandler.Handle(
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

            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Intern);

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [Guid.NewGuid()]
                    }
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Should_Throw_BadRequestException()
        {
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Intern);

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = DbContextMock.Client.FirstOrDefault().Id,
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
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Intern);
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
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
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Intern);
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var grandSaladeBar = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Grand Salade Bar");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
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
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Provider);
            await DbContextMock.SaveChangesAsync();
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var pain = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Pain");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
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
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Vip);
            var plateau = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Plateau");
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");
            var grandSaladeBar = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Grand Salade Bar");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
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
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Visitor);
            var boisson = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductName == "Boisson");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [boisson.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 1 && response.Result.ClientAmount == 1 && response.Result.CareAmount == 0);
        }

        [Fact]
        public async Task Care_Amount_Should_Not_Exceed_TotalAmout_ok()
        {
            var client = DbContextMock.Client.FirstOrDefault(c => c.ClientRole == ClientRole.Provider);
            var boisson = DbContextMock.Product.FirstOrDefault(p => p.ProductName == "Boisson");

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [boisson.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 1 && response.Result.ClientAmount == 0 && response.Result.CareAmount == 1);
        }

        [Fact]
        public async Task Meal_Tray_Should_Cost_10_ok()
        {
            var client = await DbContextMock.Client.FirstOrDefaultAsync(c => c.ClientRole == ClientRole.Intern);
            var dessert = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductType == ProductType.Dessert);
            var mainDish = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductType == ProductType.MainDish);
            var bread = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductType == ProductType.Bread);
            var starter = await DbContextMock.Product.FirstOrDefaultAsync(p => p.ProductType == ProductType.Starter);

            var response = await _mealPaymentCommandHandler.Handle(
                new MealPaymentCommand()
                {
                    ClientId = client.Id,
                    MealPaymentCommandDto = new MealPaymentCommandDto
                    {
                        ProductIds = [dessert.Id, mainDish.Id, bread.Id, starter.Id]
                    }
                }, new CancellationToken());
            Assert.True(response.Result.TotalAmount == 10 && response.Result.ClientAmount == 2.5M && response.Result.CareAmount == 7.5M);
        }
    }
}