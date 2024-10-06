
using ecommerceWebServicess.Interfaces;

namespace ecommerceWebServicess.Helpers
{
    public class StockCheckService : IHostedService, IDisposable
    {

        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;


        public StockCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run the stock check every 10 minutes (600,000 milliseconds)
            _timer = new Timer(CheckStock, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void CheckStock(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                // Call the method to check all products for low stock
                await productService.CheckAllProductsForLowStockAsync();
            }
        }

    }
}
