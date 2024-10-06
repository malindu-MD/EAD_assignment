using System.Security.Claims;
using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
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


        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }



        // POST: api/Vendor/Create
        [HttpPost("Create")]
        [Authorize(Roles = "Administrator")]  // Only administrators can create vendors
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vendor = await _vendorService.CreateVendorAsync(vendorDto);
            return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
        }

        // GET: api/Vendor/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound($"Vendor with ID {id} not found.");

            return Ok(vendor);
        }


        // PUT: api/Vendor/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]  // Only administrators can update vendor details
        public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _vendorService.UpdateVendorAsync(id, vendorDto);
            if (!success) return NotFound($"Vendor with ID {id} not found.");

            return Ok("Vendor updated successfully.");
        }



        // POST: api/Vendor/{vendorId}/AddComment
        [HttpPost("{vendorId}/AddComment")]
        [Authorize(Roles = "Customer")]  // Only customers can leave comments and ratings
        public async Task<IActionResult> AddCommentAndRating(string vendorId, [FromBody] AddVendorCommentDto commentDto)
        {
            var success = await _vendorService.AddCommentAndRatingAsync(vendorId, commentDto);
            if (!success) return NotFound($"Vendor with ID {vendorId} not found.");

            return Ok("Comment and rating added successfully.");
        }


        // PUT: api/Vendor/{vendorId}/EditComment
        [HttpPut("{vendorId}/EditComment")]
        [Authorize(Roles = "Customer")]  // Only customers can edit their comments and ratings
        public async Task<IActionResult> EditCommentAndRating(string vendorId, [FromBody] AddVendorCommentDto updatedCommentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get the current user's ID

            if (userId == null) return Unauthorized("User ID not found.");

            var success = await _vendorService.EditCommentAndRatingAsync(vendorId, userId, updatedCommentDto);
            if (!success) return NotFound($"Vendor or comment not found for Vendor ID: {vendorId}");

            return Ok("Comment and rating updated successfully.");
        }



        [HttpGet("{vendorId}/Comments")]
        public async Task<IActionResult> GetVendorComments(string vendorId)
        {
            var comments = await _vendorService.GetVendorCommentsAsync(vendorId);
            if (comments == null)
            {
                return NotFound($"Vendor with ID {vendorId} not found.");
            }

            return Ok(comments);
        }


        // GET: api/Vendor/{vendorId}/Rating
        [HttpGet("{vendorId}/Rating")]
        public async Task<IActionResult> GetVendorRating(string vendorId)
        {
            var rating = await _vendorService.GetVendorRatingAsync(vendorId);
            if (rating == null)
            {
                return NotFound($"Vendor with ID {vendorId} not found.");
            }

            return Ok(new { AverageRating = rating });
        }


        // GET: api/Vendor
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }





    }
}
