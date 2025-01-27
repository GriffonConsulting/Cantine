namespace Application.Invoice.Queries.GetInvoice;

public class GetInvoiceQueryResult
{
    public required byte[] FileContent { get; set; }
    public required string Filename { get; set; }
}
