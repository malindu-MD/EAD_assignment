/***************************************************************************
 * File: IProductService.cs
 * Description: Interface for product services. Defines methods for product
 *              management including creation, updating, deletion, and stock.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product for a vendor.
        /// </summary>
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string vendorId);

        /// <summary>
        /// Updates a product for a vendor.
        /// </summary>
        Task<ProductDto> UpdateProductAsync(string productId, UpdateProductDto updateProductDto, string vendorId);

        /// <summary>
        /// Deletes a product by ID for a vendor.
        /// </summary>
        Task<bool> DeleteProductAsync(string productId, string vendorId);

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        Task<ProductDto> GetProductByIdAsync(string productId);

        /// <summary>
        /// Retrieves a list of products based on query parameters.
        /// </summary>
        Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);

        /// <summary>
        /// Activates a product by its ID.
        /// </summary>
        Task<bool> ActivateProductAsync(string productId);

        /// <summary>
        /// Deactivates a product by its ID.
        /// </summary>
        Task<bool> DeactivateProductAsync(string productId);

        /// <summary>
        /// Updates the inventory of a product for a vendor.
        /// </summary>
        Task<bool> UpdateInventoryAsync(string productId, InventoryUpdateDto inventoryUpdateDto, string vendorId);

        /// <summary>
        /// Checks a product for low stock.
        /// </summary>
        Task CheckLowStockAsync(string productId);

        /// <summary>
        /// Removes product stock by product ID for a vendor.
        /// </summary>
        Task<bool> RemoveProductStockAsync(string productId, string vendorId);

        /// <summary>
        /// Retrieves a list of all products.
        /// </summary>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Searches products by a keyword.
        /// </summary>
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword);

        /// <summary>
        /// Retrieves products for a specific vendor.
        /// </summary>
        Task<IEnumerable<ProductDto>> GetProductsByVendorIdAsync(string vendorId);

        /// <summary>
        /// Reduces stock of a product by a specified quantity.
        /// </summary>
        Task<bool> ReduceStockAsync(string productId, int quantity);

        /// <summary>
        /// Increases stock of a product by a specified quantity.
        /// </summary>
        Task<bool> IncreaseStockAsync(string productId, int quantity);

        /// <summary>
        /// Checks all products for low stock and sends notifications.
        /// </summary>
        Task CheckAllProductsForLowStockAsync();
    }
}
