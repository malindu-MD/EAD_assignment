using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly INotificationService _notificationService;


        public OrderService(IMongoClient mongoClient, INotificationService notificationService)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _orderCollection = database.GetCollection<Order>("Orders");
            _notificationService = notificationService;
        }

        // Cancel an order
        public async Task<bool> CancelOrderAsync(string orderId, string cancellationNote)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, OrderStatus.Cancelled);
            var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);
            if (result.ModifiedCount > 0)
            {
                var order = await GetOrderByIdAsync(orderId);
                await _notificationService.SendNotificationAsync(order.UserId, $"Your order has been cancelled. Note: {cancellationNote}", orderId);
                return true;
            }
            return false;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                UserId = createOrderDto.UserId,
                ShippingAddress = new Address
                {
                    Street = createOrderDto.ShippingAddress.Street,
                    City = createOrderDto.ShippingAddress.City,
                    Zip = createOrderDto.ShippingAddress.Zip
                },
                OrderItems = createOrderDto.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = FulfillmentStatusEnum.Pending
                }).ToList(),
                TotalAmount = createOrderDto.OrderItems.Sum(item => item.ProductPrice * item.Quantity)
            };

            await _orderCollection.InsertOneAsync(order);

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    Zip = order.ShippingAddress.Zip
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString()
                }).ToList()
            };
        }

        // Get all orders
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderCollection.Find(Builders<Order>.Filter.Empty).ToListAsync();
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    Zip = order.ShippingAddress.Zip
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString()
                }).ToList()
            });
        }

        // Get an order by ID
        public async Task<OrderDto> GetOrderByIdAsync(string orderId)
        {
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    Zip = order.ShippingAddress.Zip
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString()
                }).ToList()
            };
        }

        // Get all orders by user ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _orderCollection.Find(o => o.UserId == userId).ToListAsync();
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    Zip = order.ShippingAddress.Zip
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString()
                }).ToList()
            });
        }

        // Get vendor orders
        public async Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(string vendorId)
        {
            var orders = await _orderCollection.Find(o => o.OrderItems.Any(i => i.VendorId == vendorId)).ToListAsync();
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    Zip = order.ShippingAddress.Zip
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString()
                }).ToList()
            });
        }

        public async Task<bool> MarkOrderAsDeliveredAsync(string orderId)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, OrderStatus.Fulfilled);
            var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);
            if (result.ModifiedCount > 0)
            {
                var order = await GetOrderByIdAsync(orderId);
                await _notificationService.SendNotificationAsync(order.UserId, "Your order has been delivered.", orderId);
                return true;
            }
            return false;
        }

        // Vendor marks their product as ready
        public async Task<bool> MarkProductAsReadyByVendorAsync(string orderId, string productId, string vendorId)
        {
            // Fetch the order by its ID
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return false; // If the order doesn't exist, return false
            }

            // Find the specific product from the order items based on productId and vendorId
            var product = order.OrderItems.FirstOrDefault(p => p.ProductId == productId && p.VendorId == vendorId);
            if (product != null)
            {
                // Mark the product's fulfillment status as "Fulfilled"
                product.FulfillmentStatus = FulfillmentStatusEnum.Fulfilled;

                // Check if all order items are fulfilled
                var allItemsFulfilled = order.OrderItems.All(p => p.FulfillmentStatus == FulfillmentStatusEnum.Fulfilled);

                // Determine the overall order status
                var statusUpdate = allItemsFulfilled ? OrderStatus.Fulfilled : OrderStatus.PartiallyFulfilled;

                // Create update definition for MongoDB
                var update = Builders<Order>.Update
                    .Set(o => o.OrderItems, order.OrderItems)  // Update the order items in the order
                    .Set(o => o.Status, statusUpdate);         // Update the overall order status

                // Apply the update to MongoDB
                var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);

                if (result.ModifiedCount > 0)
                {
                    // If the entire order is fulfilled, notify the customer
                    if (statusUpdate == OrderStatus.Fulfilled)
                    {
                        await _notificationService.SendNotificationAsync(order.UserId, $"Your order {orderId} has been fully delivered.", orderId);
                    }

                    // Return true if the update was successful
                    return true;
                }
            }

            // Return false if the product wasn't found or the update failed
            return false;
        }



        public async Task<bool> UpdateOrderAsync(UpdateOrderDto updateOrderDto)
        {
            var update = Builders<Order>.Update
                .Set(o => o.ShippingAddress, new Address
                {
                    Street = updateOrderDto.ShippingAddress.Street,
                    City = updateOrderDto.ShippingAddress.City,
                    Zip = updateOrderDto.ShippingAddress.Zip
                })
                .Set(o => o.Status, Enum.Parse<OrderStatus>(updateOrderDto.Status))
                .Set(o => o.OrderItems, updateOrderDto.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = Enum.Parse<FulfillmentStatusEnum>(item.FulfillmentStatus)
                }).ToList());

            var result = await _orderCollection.UpdateOneAsync(o => o.Id == updateOrderDto.Id, update);
            return result.ModifiedCount > 0;
        }

    }
}
