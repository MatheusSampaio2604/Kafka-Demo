using Shared.Contracts;

namespace Consumer.Worker.Services
{
    public interface IOrderProcessingService
    {
        Task<bool> ProcessAsync(OrderCreated order, string currentTopic);
    }
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(ILogger<OrderProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ProcessAsync(OrderCreated order, string currentTopic)
        {
            if (currentTopic.Contains("retry") && Random.Shared.NextDouble() < 0.4)
            {
                if (_logger.IsEnabled(LogLevel.Warning))                
                    _logger.LogWarning("Simulated failure for order {OrderId} on retry topic.", order.OrderId);
                
                return false;
            }

            await Task.Delay(2000); // Simulate processing time
            
            if (_logger.IsEnabled(LogLevel.Information))            
                _logger.LogInformation("Processed order {OrderId} for customer {CustomerId} with total amount {TotalAmount}.", order.OrderId, order.CustomerId, order.TotalAmount);
            
            return true;
        }
    }
}
