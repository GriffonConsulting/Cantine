namespace Application.Payment.Commands.MealPayment;

public class MealPaymentResult
{
    public Guid OrderId { get; set; }
    public decimal ClientAmount { get; set; }
    public decimal CareAmount { get; set; }
    public decimal TotalAmount { get; set; }
}