using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IOrderProcessor
    {
        void ProcessOrder(ShoppingCart cart, ShippingDetails shippingDetails);
    }
}
