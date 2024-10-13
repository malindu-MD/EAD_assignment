/***************************************************************************
 * File: IUserService.cs
 * Description: Interface for user-related services. Defines methods for
 *              managing user accounts, including registration, updating, 
 *              deactivation, and fetching users.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        Task<UserDTO> GetUserByIdAsync(string id);

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();

        /// <summary>
        /// Registers a new user.
        /// </summary>
        Task<UserDTO> RegisterUserAsync(RegisterDTO registerDto);

        /// <summary>
        /// Updates an existing user by their ID.
        /// </summary>
        Task<UserDTO> UpdateUserAsync(string id, UserDTO userDto);

        /// <summary>
        /// Deactivates a user by their ID.
        /// </summary>
        Task DeactivateUserAsync(string id);

        /// <summary>
        /// Reactivates a user by their ID.
        /// </summary>
        Task ReactivateUserAsync(string id);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        Task DeleteUserAsync(string id);

        /// <summary>
        /// Retrieves a list of all customers.
        /// </summary>
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    }
}
