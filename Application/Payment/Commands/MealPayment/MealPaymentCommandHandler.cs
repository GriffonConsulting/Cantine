using Application.Common.Requests;
using Domain.Product;
using EntityFramework.Commands;
using EntityFramework.Entities;
using EntityFramework.Queries;
using MediatR;

namespace Application.Payment.Commands.MealPayment;

public class MealPaymentCommandHandler : IRequestHandler<MealPaymentCommand, RequestResult<MealPaymentResult>>
{

    private readonly ClientAccountCommands _clientAccountCommands;
    private readonly ClientAccountTransactionHistoryCommands _clientAccountTransactionHistoryCommands;
    private readonly ClientAccountQueries _clientAccountQueries;
    private readonly ClientQueries _clientQueries;
    private readonly ProductQueries _productQueries;
    private readonly OrderCommands _orderCommands;
    private readonly OrderContentCommands _orderContentCommands;

    public MealPaymentCommandHandler(ClientAccountCommands clientAccountCommands,
        ClientAccountTransactionHistoryCommands clientAccountTransactionHistoryCommands,
        ClientAccountQueries clientAccountQueries,
        ClientQueries clientQueries,
        ProductQueries productQueries,
        OrderCommands orderCommands,
        OrderContentCommands orderContentCommands)
    {
        _clientAccountCommands = clientAccountCommands;
        _clientAccountTransactionHistoryCommands = clientAccountTransactionHistoryCommands;
        _clientAccountQueries = clientAccountQueries;
        _clientQueries = clientQueries;
        _productQueries = productQueries;
        _orderCommands = orderCommands;
        _orderContentCommands = orderContentCommands;
    }


    public async Task<RequestResult<MealPaymentResult>> Handle(MealPaymentCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientQueries.GetById(request.ClientId, cancellationToken);
        if (client == null) return new RequestResult<MealPaymentResult> { Message = "The client doesn't exist", StatusCodes = RequestStatusCodes.Status400BadRequest };

        var clientAccount = await _clientAccountQueries.GetByClientIdAsync(request.ClientId, cancellationToken);
        if (clientAccount == null) return new RequestResult<MealPaymentResult> { Message = "The client account doesn't exist", StatusCodes = RequestStatusCodes.Status400BadRequest };

        var products = await _productQueries.GetByProductsIdsAsync(request.MealPaymentCommandDto.ProductIds, cancellationToken);
        var orderContents = new List<OrderContent>();
        decimal orderTotalAmount = 0;

        foreach (var productId in request.MealPaymentCommandDto.ProductIds)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return new RequestResult<MealPaymentResult> { Message = "Product not found", StatusCodes = RequestStatusCodes.Status400BadRequest };

            orderContents.Add(new OrderContent { Amount = product.ProductPrice, ProductId = productId, ProductType = product.ProductType, ProductName = product.ProductName });
        }

        await checkMealTrayAsync(products, orderContents, cancellationToken);
        orderTotalAmount = orderContents.Sum(oc => oc.Amount);

        decimal careAmount = CalcCareAmount(client, orderTotalAmount);

        clientAccount.Amount -= orderTotalAmount - careAmount;
        clientAccount.ModifiedOn = DateTime.UtcNow;

        if (clientAccount.Amount < 0 && !client.Role.CanOverDraft) return new RequestResult<MealPaymentResult> { Message = "OverDraft", StatusCodes = RequestStatusCodes.Status400BadRequest };


        await _clientAccountCommands.UpdateEntityAsync(clientAccount, cancellationToken);
        await _clientAccountTransactionHistoryCommands.AddAsync(new ClientAccountTransactionHistory
        {
            TransactionAmount = -orderTotalAmount,
            ClientNewAmount = clientAccount.Amount,
            ClientId = request.ClientId,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            UserId = request.ClientId,
        }, cancellationToken);

        var orderId = await _orderCommands.AddAsync(new Order
        {
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            CareAmount = careAmount,
            ClientId = request.ClientId,
            ClientAmount = orderTotalAmount - careAmount,
            TotalAmount = orderTotalAmount
        }, cancellationToken);

        foreach (var orderContent in orderContents)
        {
            await _orderContentCommands.AddAsync(new OrderContent
            {
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                OrderId = orderId,
                Amount = orderContent.Amount,
                ProductId = orderContent.ProductId,
                ProductName = orderContent.ProductName
            }, cancellationToken);
        }

        return new RequestResult<MealPaymentResult>
        {
            Result = new MealPaymentResult
            {
                OrderId = orderId,
                CareAmount = careAmount,
                ClientAmount = orderTotalAmount - careAmount,
                TotalAmount = orderTotalAmount
            },

            StatusCodes = RequestStatusCodes.Status200OK
        };
    }

    private async Task checkMealTrayAsync(Product[] products, List<OrderContent> orderContents, CancellationToken cancellationToken)
    {
        if (orderContents.Any(oc => oc.ProductType == ProductType.MainDish)
            && orderContents.Any(oc => oc.ProductType == ProductType.Dessert)
            && orderContents.Any(oc => oc.ProductType == ProductType.Bread)
            && orderContents.Any(oc => oc.ProductType == ProductType.Starter))
        {
            orderContents.FirstOrDefault(oc => oc.ProductType == ProductType.MainDish).Amount = 0;
            orderContents.FirstOrDefault(oc => oc.ProductType == ProductType.Dessert).Amount = 0;
            orderContents.FirstOrDefault(oc => oc.ProductType == ProductType.Bread).Amount = 0;
            orderContents.FirstOrDefault(oc => oc.ProductType == ProductType.Starter).Amount = 0;

            var mealTray = await _productQueries.GetByProductCode("MealTray", cancellationToken);
            if (mealTray == null) throw new Exception("product MealTray not defined");
            orderContents.Add(new OrderContent { Amount = mealTray.ProductPrice, ProductId = mealTray.Id, ProductType = mealTray.ProductType, ProductName = mealTray.ProductName });
        }
    }


    private static decimal CalcCareAmount(Client client, decimal orderTotalAmount)
    {
        decimal careAmount = 0;
        if (client.Role?.MealCareAmount != null)
        {
            if (client.Role?.MealCareAmount > orderTotalAmount)
                careAmount = orderTotalAmount;
            else
                careAmount = (decimal)(client.Role?.MealCareAmount);
        }
        else if (client.Role?.MealCarePercent != null)
        {
            careAmount = (decimal)(orderTotalAmount * client.Role?.MealCarePercent / 100);
        }

        return careAmount;
    }
}
