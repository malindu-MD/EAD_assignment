/*
    File: RegisterDTO.cs
    Author: [Your Name]
    Description: This file defines the RegisterDTO (Data Transfer Object) class, which is used to transfer user registration 
    data from the client to the server during the user registration process. It contains basic user details like username, 
    email, phone number, and password.
*/


namespace ecommerceWebServicess.DTOs
{
    public class RegisterDTO
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

    }
}
