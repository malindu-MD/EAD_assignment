using ecommerceWebServicess.DTOs;

namespace ecommerceWebServicess.Interfaces
{
    public interface IAuthService
    {

        Task<LoginResponseDTO> AuthenticateAsync(LoginDTO loginDto);
    }
}
