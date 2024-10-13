/***************************************************************************
 * File: StockCheckService.cs
 * Description: This file implements a background service that periodically 
 *              checks product stock levels and sends notifications for low stock.
 ***************************************************************************/

using ecommerceWebServicess.Interfaces;

namespace ecommerceWebServicess.Helpers
{
    // Background service for checking product stock levels at regular intervals.
    public class StockCheckService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        // Constructor to initialize service provider for dependency injection.
        public StockCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Dispose of the timer when the service is disposed.
        public void Dispose()
        {
            _timer?.Dispose();
        }

        // Start the service and initiate the stock check timer.
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run the stock check every 5 minutes.
            _timer = new Timer(CheckStock, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        // Stop the service and disable the timer.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        // Method that checks for low stock by calling the product service.
        private async void CheckStock(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                // Call the method to check all products for low stock.
                await productService.CheckAllProductsForLowStockAsync();
            }
        }
    }
}
