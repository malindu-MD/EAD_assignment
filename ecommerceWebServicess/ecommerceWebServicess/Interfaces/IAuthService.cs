/***************************************************************************
 * File: IAuthService.cs
 * Description: Interface for authentication services. Defines methods for 
 *              handling user authentication processes such as login.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using the provided login details.
        /// </summary>
        /// <param name="loginDto">Login details provided by the user.</param>
        /// <returns>LoginResponseDTO containing the user's token and information.</returns>
        Task<LoginResponseDTO> AuthenticateAsync(LoginDTO loginDto);
    }
}
