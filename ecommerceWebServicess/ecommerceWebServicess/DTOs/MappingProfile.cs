using AutoMapper;
using ecommerceWebServicess.Models;

namespace ecommerceWebServicess.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define mapping between CreateCategoryDto and Category
            CreateMap<CreateCategoryDto, Category>();

            // Define mapping between UpdateCategoryDto and Category
            CreateMap<UpdateCategoryDto, Category>();

            // You can also define reverse mappings if needed
            CreateMap<Category, CategoryDto>();

            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            // Define mapping between InventoryUpdateDto and Product

            CreateMap<InventoryUpdateDto, Product>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.StockThreshold, opt => opt.MapFrom(src => src.StockThreshold));

            // Define mapping between CreateProductDto and Product
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();


          
        }


    }
}
