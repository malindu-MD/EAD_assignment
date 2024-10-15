/***************************************************************************
 * File: VendorController.cs
 * Description: Controller responsible for managing vendor-related operations 
 *              such as creating vendors, updating vendor details, adding 
 *              comments and ratings, and retrieving vendor information.
 ***************************************************************************/

using System.Security.Claims;
using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        // Constructor to inject the vendor service
        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // POST: Create a new vendor (Admin-only)
        [HttpPost("Create")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vendor = await _vendorService.CreateVendorAsync(vendorDto);
            return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
        }

        // GET: Retrieve a vendor by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound($"Vendor with ID {id} not found.");

            return Ok(vendor);
        }

        // PUT: Update vendor details (Admin-only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _vendorService.UpdateVendorAsync(id, vendorDto);
            if (!success) return NotFound($"Vendor with ID {id} not found.");

            return Ok("Vendor updated successfully.");
        }

        // POST: Add a comment and rating for a vendor (Customer-only)
        [HttpPost("{vendorId}/AddComment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCommentAndRating(string vendorId, [FromBody] AddVendorCommentDto commentDto)
        {
            var success = await _vendorService.AddCommentAndRatingAsync(vendorId, commentDto);
            if (!success) return NotFound($"Vendor with ID {vendorId} not found.");

            return Ok("Comment and rating added successfully.");
        }

        // PUT: Edit a customer's comment and rating for a vendor (Customer-only)
        [HttpPut("{vendorId}/EditComment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> EditCommentAndRating(string vendorId, [FromBody] AddVendorCommentDto updatedCommentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("User ID not found.");

            var success = await _vendorService.EditCommentAndRatingAsync(vendorId, userId, updatedCommentDto);
            if (!success) return NotFound($"Vendor or comment not found for Vendor ID: {vendorId}");

            return Ok("Comment and rating updated successfully.");
        }

        // GET: Retrieve all comments for a vendor
        [HttpGet("{vendorId}/Comments")]
        public async Task<IActionResult> GetVendorComments(string vendorId)
        {
            var comments = await _vendorService.GetVendorCommentsAsync(vendorId);
            if (comments == null) return NotFound($"Vendor with ID {vendorId} not found.");

            return Ok(comments);
        }

        // GET: Retrieve the average rating for a vendor
        [HttpGet("{vendorId}/Rating")]
        public async Task<IActionResult> GetVendorRating(string vendorId)
        {
            var rating = await _vendorService.GetVendorRatingAsync(vendorId);
            if (rating == null) return NotFound($"Vendor with ID {vendorId} not found.");

            return Ok(new { AverageRating = rating });
        }

        // GET: Retrieve all vendors
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }


        [HttpGet("{vendorId}/comment/{userId}")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> GetCommentByVendorAndUser(string vendorId, string userId)
        {
            var comment = await _vendorService.GetCommentByVendorAndUserIdAsync(vendorId, userId);
            if (comment == null)
            {
                return NotFound(); // Return 404 if no comment is found
            }
            return Ok(comment); // Return the found comment
        }
        [HttpGet("comments/user/{userId}")]
        public async Task<IActionResult> GetCommentsByUserId(string userId)
        {
            var comments = await _vendorService.GetAllCommentsByUserIdAsync(userId);
            if (comments == null || !comments.Any())
            {
                return NotFound(); // Return 404 if no comments found
            }
            return Ok(comments); // Return the found comments along with vendor details
        }



    }
}
