﻿/**************************************************************************
 * File: OrderService.cs
 * Description: Service for managing orders, including creating, updating, 
 *              canceling, and retrieving orders.
 **************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;
using System;
using System.Linq;


namespace ecommerceWebServicess.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly INotificationService _notificationService;
        private readonly IVendorService _vendorService; // Inject VendorService
        private readonly IProductService _productService;

        public OrderService(IMongoClient mongoClient, INotificationService notificationService,IVendorService vendorService,IProductService productService)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _orderCollection = database.GetCollection<Order>("Orders");
            _notificationService = notificationService;
            _vendorService = vendorService;
            _productService = productService;   

        }

        // Cancel an order and send a notification
        public async Task<bool> CancelOrderAsync(string orderId, string cancellationNote)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, OrderStatus.Cancelled);
            var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);

            if (result.ModifiedCount > 0)
            {
                var order = await GetOrderByIdAsync(orderId);

                if (order == null) return false; // Ensure the order exists

                // Update the fulfillment status of all order items to Delivered
                foreach (var item in order.OrderItems)
                {
                    if (item.FulfillmentStatus != FulfillmentStatusEnum.Cancelled.ToString()) // Check if the item is not already Cancelled
                    {
                        item.FulfillmentStatus = FulfillmentStatusEnum.Cancelled.ToString(); // Change status to Cancelled

                        await _productService.IncreaseStockAsync(item.ProductId, item.Quantity);


                        await _notificationService.SendNotificationAsync(item.VendorId, $" Order has been cancelled OrderID-{order.OrderId}");

                    }
                }

                // Create the update definition for the order items

                // Execute the update operation on the database

                var updateItems = Builders<Order>.Update.Set(o => o.OrderItems, order.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = Enum.Parse<FulfillmentStatusEnum>(item.FulfillmentStatus),
                    ImageUrl = item.ImageUrl
                }).ToList());

                await _orderCollection.UpdateOneAsync(o => o.Id == orderId, updateItems);

                // Send notification to the user
                await _notificationService.SendNotificationAsync(order.UserId, $"Your order {order.OrderId} has been cancelled. Note: {cancellationNote}");



                return true;
            }
            return false;
        }

        // Create a new order
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var orderItems = new List<OrderItem>();

            // Loop through each item in the createOrderDto to get the vendor name and reduce stock
            foreach (var item in createOrderDto.OrderItems)
            {
                // Get the vendor name using the VendorService
                var vendor = await _vendorService.GetVendorByIdAsync(item.VendorId);

                // Create the order item with the vendor name
                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = vendor?.BusinessName ?? "Unknown", // Set vendor name, default to "Unknown" if not found
                    FulfillmentStatus = FulfillmentStatusEnum.Pending,
                    ImageUrl = item.ImageUrl
                });

                // Reduce stock for the ordered product
                bool stockReduced = await _productService.ReduceStockAsync(item.ProductId, item.Quantity);
                if (!stockReduced)
                {
                    // Handle stock reduction failure (e.g., log the error, throw an exception, etc.)
                    throw new Exception($"Failed to reduce stock for product {item.ProductId}");
                }

                await _notificationService.SendNotificationAsync(item.VendorId, $"New Order Recived For {item.ProductName} ({item.ProductCode})");

                
            }

            var order = new Order
            {
                OrderId = GenerateShortOrderId(),
                UserId = createOrderDto.UserId,
                ShippingAddress = new Address
                {
                    Street = createOrderDto.ShippingAddress.Street,
                    City = createOrderDto.ShippingAddress.City,
                    Zip = createOrderDto.ShippingAddress.Zip
                },
                OrderItems = orderItems, // Use the populated order items list
                TotalAmount = orderItems.Sum(item => item.ProductPrice * item.Quantity)
            };

            await _orderCollection.InsertOneAsync(order);

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderId = order.OrderId,
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
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString(),
                    ImageUrl = item.ImageUrl
                }).ToList()
            };
        }



        // Get all orders
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderCollection.Find(Builders<Order>.Filter.Empty).ToListAsync();
            return orders.Select(order => MapToOrderDto(order));
        }

        // Get an order by ID
        public async Task<OrderDto> GetOrderByIdAsync(string orderId)
        {
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            return order == null ? null : MapToOrderDto(order);
        }

        // Get all orders by user ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _orderCollection.Find(o => o.UserId == userId).ToListAsync();
            return orders.Select(order => MapToOrderDto(order));
        }

        // Get vendor orders
        public async Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(string vendorId)
        {
            var orders = await _orderCollection.Find(o => o.OrderItems.Any(i => i.VendorId == vendorId)).ToListAsync();
            return orders.Select(order => MapToOrderDto(order));
        }

        // Mark an order as delivered
        public async Task<bool> MarkOrderAsDeliveredAsync(string orderId)
        {
            // Update the main order status to Fulfilled
            var updateOrder = Builders<Order>.Update.Set(o => o.Status, OrderStatus.Fulfilled);
            var orderUpdateResult = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, updateOrder);

            if (orderUpdateResult.ModifiedCount > 0)
            {
                // Fetch the order to get the order items
                var order = await GetOrderByIdAsync(orderId);
                if (order == null) return false; // Ensure the order exists

                // Update the fulfillment status of all order items to Delivered
                foreach (var item in order.OrderItems)
                {
                    if (item.FulfillmentStatus != FulfillmentStatusEnum.Delivered.ToString()) // Check if the item is not already delivered
                    {
                        item.FulfillmentStatus = FulfillmentStatusEnum.Delivered.ToString(); // Change status to Delivered
                    }
                }

                // Create the update definition for the order items

                // Execute the update operation on the database

                var updateItems = Builders<Order>.Update.Set(o => o.OrderItems, order.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = Enum.Parse<FulfillmentStatusEnum>(item.FulfillmentStatus),
                    ImageUrl = item.ImageUrl
                }).ToList());

                await _orderCollection.UpdateOneAsync(o => o.Id == orderId, updateItems);

                // Send notification to the user
                await _notificationService.SendNotificationAsync(order.UserId, $"Your order {order.OrderId} has been fully delivered.");
                return true;
            }
            return false; // Indicate failure if the order status update failed
        }






        // Mark product as ready by vendor
        public async Task<bool> MarkProductAsReadyByVendorAsync(string orderId, string productId, string vendorId)
        {
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return false;

            var product = order.OrderItems.FirstOrDefault(p => p.ProductId == productId && p.VendorId == vendorId);
            if (product != null)
            {
                product.FulfillmentStatus = FulfillmentStatusEnum.Delivered;
                var allItemsFulfilled = order.OrderItems.All(p => p.FulfillmentStatus == FulfillmentStatusEnum.Delivered);
                var statusUpdate = allItemsFulfilled ? OrderStatus.Fulfilled : OrderStatus.PartiallyFulfilled;

                var update = Builders<Order>.Update.Set(o => o.OrderItems, order.OrderItems).Set(o => o.Status, statusUpdate);
                var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);

                if (result.ModifiedCount > 0) 
                {
                    if (statusUpdate == OrderStatus.Fulfilled)
                    {
                        await _notificationService.SendNotificationAsync(order.UserId, $"Your order {order.OrderId} has been fully delivered.");
                    }
                    else
                    {
                        await _notificationService.SendNotificationAsync(order.UserId, $"Your {product.ProductName} product in  order {order.OrderId} has been delivered.");

                    }
                    return true;
                }
            }
            return false;
        }

        // Update an order
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
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = Enum.Parse<FulfillmentStatusEnum>(item.FulfillmentStatus),
                    ImageUrl = item.ImageUrl
                }).ToList());

            var result = await _orderCollection.UpdateOneAsync(o => o.Id == updateOrderDto.Id, update);
            return result.ModifiedCount > 0;
        }

        // Get order items by order ID and vendor ID
        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAndVendorIdAsync(string orderId, string vendorId)
        {
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return Enumerable.Empty<OrderItemDto>();

            var vendorOrderItems = order.OrderItems.Where(item => item.VendorId == vendorId).ToList();
            if (!vendorOrderItems.Any()) return Enumerable.Empty<OrderItemDto>();

            return vendorOrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                ProductPrice = item.ProductPrice,
                Quantity = item.Quantity,
                VendorId = item.VendorId,
                VendorName = item.VendorName,
                FulfillmentStatus = item.FulfillmentStatus.ToString(),
                ImageUrl = item.ImageUrl
            }).ToList();
        }



        public async Task<bool> MarkProductAsShippedByVendorAsync(string orderId, string productId, string vendorId)
        {
            // Retrieve the order by ID
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return false;

            // Find the specific product in the order using productId and vendorId
            var product = order.OrderItems.FirstOrDefault(p => p.ProductId == productId && p.VendorId == vendorId);
            if (product != null)
            {
                // Change the fulfillment status of the specified product
                product.FulfillmentStatus = FulfillmentStatusEnum.Shipped; // Set the desired status here

                // Create the update definition to update only the specific order item
                var update = Builders<Order>.Update
                    .Set(o => o.OrderItems, order.OrderItems); // Update order items only

                // Execute the update operation on the database
                var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, update);

                if (result.ModifiedCount > 0)
                {
                    // Optional: Send a notification if needed
                    await _notificationService.SendNotificationAsync(order.UserId, $"The {product.ProductName} product in your order {order.OrderId} has been marked as shipped.");
                    return true; // Indicate that the operation was successful
                }
            }
            return false; // Indicate that the product was not found or the operation failed
        }



        // Get order items by order ID
        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(string orderId)
        {
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return Enumerable.Empty<OrderItemDto>();

            return order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                ProductPrice = item.ProductPrice,
                Quantity = item.Quantity,
                VendorId = item.VendorId,
                VendorName = item.VendorName,
                FulfillmentStatus = item.FulfillmentStatus.ToString(),
                ImageUrl = item.ImageUrl
            }).ToList();
        }

        // Helper method to map Order to OrderDto
        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderId = order.OrderId,
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
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    VendorId = item.VendorId,
                    VendorName = item.VendorName,
                    FulfillmentStatus = item.FulfillmentStatus.ToString(),
                    ImageUrl = item.ImageUrl
                }).ToList(),
                Messages=order.Messages
                
            };
        }

        public static string GenerateShortOrderId()
        {
            // Generate a 6-character random string
            var randomString = GenerateRandomString(6);
            // Get the current timestamp
            var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
            // Combine the random string with the timestamp
            return $"{randomString}-{timestamp}";
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }





        public async Task<bool> CancelOrderAsyn(OrderCancelRequestDto orderCancelRequest)
        {
            // Fetch the order first
            var order = await GetOrderByIdAsyn(orderCancelRequest.OrderId);

            if (order == null) return false; // Ensure the order exists

            // Add the cancellation note to the order's cancellation requests
            if (order.Messages == null)
            {
                order.Messages = new List<string>(); // Initialize if null
            }
            order.Messages.Add(orderCancelRequest.Note); // Add the cancellation note

            // Update the fulfillment status of all order items to Cancelled

            // Create the update definition for the order
            var update = Builders<Order>.Update
                .Set(o => o.Messages, order.Messages);// Update cancellation requests
              

            // Execute the update operation on the database
            var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderCancelRequest.OrderId, update);

            if (result.ModifiedCount > 0)

            {
                // Send notification to the user about the cancellation
                return true; // Indicate success
            }

            return false; // Indicate failure if the order status update failed
        }


        // Method to get an order by ID
        public async Task<Order> GetOrderByIdAsyn(string orderId)
        {
            // Fetch the order from the database using the provided order ID
            var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();

            // Return the order, or null if not found
            return order;
        }






    }
}
