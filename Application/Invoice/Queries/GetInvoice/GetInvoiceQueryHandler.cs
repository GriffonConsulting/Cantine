using Application.Common.Requests;
using Application.Payment.Commands.MealPayment;
using EntityFramework.Entities;
using EntityFramework.Queries;
using MediatR;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Text;

namespace Application.Invoice.Queries.GetInvoice;

public class GetInvoiceQueryHandler : IRequestHandler<GetInvoiceQuery, RequestResult<GetInvoiceQueryResult>>
{
    private readonly OrderQueries _orderQueries;

    public GetInvoiceQueryHandler(OrderQueries orderQueries)
    {
        _orderQueries = orderQueries;
    }


    public async Task<RequestResult<GetInvoiceQueryResult>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderQueries.GetOrderByIdWithOrderContentAsync(request.OrderId, request.ClientId, cancellationToken);
        if (order == null) return new RequestResult<GetInvoiceQueryResult> { Message = "The order doesn't exist", StatusCodes = RequestStatusCodes.Status400BadRequest };
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20);

        var y = 0;
        foreach (var orderContent in order.OrderContents)
        {
            gfx.DrawString($"{orderContent.ProductName} {orderContent.Amount} €", font, XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopLeft);
            y += 20;
        }
        gfx.DrawString($"Payé par le client : {order.ClientAmount} €", font, XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopLeft);
        y += 20;
        gfx.DrawString($"Prise en charge : {order.CareAmount} €", font, XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopLeft);
        y += 20;
        gfx.DrawString($"Montant total : {order.TotalAmount} €", font, XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopLeft);

        byte[]? fileContents = null;
        using (MemoryStream stream = new MemoryStream())
        {
            document.Save(stream, true);
            fileContents = stream.ToArray();
        }

        return new RequestResult<GetInvoiceQueryResult>
        {
            Result = new GetInvoiceQueryResult
            {
                FileContent = fileContents,
                Filename = order.Id.ToString()
            },

            StatusCodes = RequestStatusCodes.Status200OK
        };
    }
}
