/*
    File: LoginDTO.cs
    Author: [Your Name]
    Description: This file defines the LoginDTO (Data Transfer Object) class, which is used to transfer 
    user login data (email and password) from the client to the server for authentication purposes.
*/

namespace ecommerceWebServicess.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
