public interface IPaymentService
{
    Task ProcessPaymentEvent(PaymentEvent paymentEvent);
}
