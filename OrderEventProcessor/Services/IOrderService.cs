public interface IOrderService
{
    Task ProcessOrderEvent(OrderEvent orderEvent);
}
