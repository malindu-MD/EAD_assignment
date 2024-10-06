using ecommerceWebServicess.DTOs;

namespace ecommerceWebServicess.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string vendorId);

        Task<ProductDto> UpdateProductAsync(string productId, UpdateProductDto updateProductDto, string vendorId);

        Task<bool> DeleteProductAsync(string productId, string vendorId);

        Task<ProductDto> GetProductByIdAsync(string productId);

        Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);

        Task<bool> ActivateProductAsync(string productId);

        Task<bool> DeactivateProductAsync(string productId);

        // Inventory management methods
        Task<bool> UpdateInventoryAsync(string productId, InventoryUpdateDto inventoryUpdateDto, string vendorId);

        Task CheckLowStockAsync(string productId);

        Task<bool> RemoveProductStockAsync(string productId, string vendorId);

        Task<IEnumerable<ProductDto>> GetAllProductsAsync();


        Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword);



        Task<IEnumerable<ProductDto>> GetProductsByVendorIdAsync(string vendorId);


        Task<bool> ReduceStockAsync(string productId, int quantity);


        Task<bool> IncreaseStockAsync(string productId, int quantity);



        Task CheckAllProductsForLowStockAsync();






    }
}
