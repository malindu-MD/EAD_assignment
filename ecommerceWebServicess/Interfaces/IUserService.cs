using ecommerceWebServicess.DTOs;

namespace ecommerceWebServicess.Interfaces
{
    public interface IUserService
    {

        Task<UserDTO> GetUserByIdAsync(string id);

        Task<IEnumerable<UserDTO>> GetAllUsersAsync();

        Task<UserDTO> RegisterUserAsync(RegisterDTO registerDto);

        Task<UserDTO> UpdateUserAsync(string id, UserDTO userDto);

        Task DeactivateUserAsync(string id);

        Task ReactivateUserAsync(string id);

        Task DeleteUserAsync(string id);


    }
}
