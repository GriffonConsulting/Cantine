using Application.Common.Requests;
using MediatR;

namespace Application.Invoice.Queries.GetInvoice;

public class GetInvoiceQuery : IRequest<RequestResult<GetInvoiceQueryResult>>
{
    public required Guid ClientId { get; set; }
    public required Guid OrderId { get; set; }
}
