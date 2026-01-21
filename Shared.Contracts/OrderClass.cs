using System.Net.Sockets;

namespace Shared.Contracts
{
    public class Class1
    {

    }

    public record OrderCreated(
        Guid OrderId,
        string CustomerId,
        decimal TotalAmount,
        List<OrderItem> Items,
        DateTime CreatedAt,
        Address ShippingAddress);

    public record OrderItem(string Sku, int Quantity, decimal UnitPrice);
    public record Address(string Street, string City, string State, string ZipCode, string Country);
}
