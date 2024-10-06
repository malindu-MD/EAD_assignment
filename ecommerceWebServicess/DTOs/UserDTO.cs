/*
    File: UserDTO.cs
    Author: [Your Name]
    Description: This file defines the UserDTO (Data Transfer Object) class. 
    The UserDTO class is used to transfer user data between the client and server in a simplified and secure manner, 
    excluding sensitive information like passwords.
*/


namespace ecommerceWebServicess.DTOs
{
    public class UserDTO
    {

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }  
        public string Role { get; set; }

    }
}
