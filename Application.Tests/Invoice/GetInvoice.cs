using Application.ClientAccounts.Commands.CreditAccount;
using Application.Common.Requests;
using Application.Invoice.Queries.GetInvoice;
using EntityFramework.Commands;
using EntityFramework.Entities;
using EntityFramework.Queries;
using Moq;

namespace Application.Tests.ClientAccounts
{
    public class GetInvoiceTests : UnitTestBase
    {
        private readonly GetInvoiceQueryHandler getInvoiceQueryHandler;


        public GetInvoiceTests()
        {
            Mock<OrderQueries> orderQueries = new(DbContextMock);
            getInvoiceQueryHandler = new GetInvoiceQueryHandler(orderQueries.Object);
        }

        [Fact]
        public async Task Zero_amount_Should_Throw_BadRequestException()
        {
            var response = await getInvoiceQueryHandler.Handle(
                new GetInvoiceQuery()
                {
                    OrderId = new Guid(),
                    ClientId = new Guid(),
                }, new CancellationToken());
            Assert.True(response.StatusCodes == RequestStatusCodes.Status400BadRequest);
        }



        [Fact]
        public async Task Should_Be_Ok()
        {
            var clientId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            DbContextMock.Order.Add(
                new Order
                {
                    ClientId = clientId,
                    Id = orderId,
                    CareAmount = 10,
                    TotalAmount = 5,
                    ClientAmount = 5,
                    OrderContents = [new OrderContent { Amount = 10, ProductName = "Plateau", ProductType = Domain.Product.ProductType.Other } ]
                });
            DbContextMock.SaveChanges();

            var response = await getInvoiceQueryHandler.Handle(
                new GetInvoiceQuery()
                {
                    OrderId = orderId,
                    ClientId = clientId,
                }, new CancellationToken());
            Assert.True(response.Result.FileContent.Length != 0);
        }

    }
}
