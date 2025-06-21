using Gradutionproject.Context;
using Gradutionproject.Models;
using Gradutionproject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gradutionproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly graduationDbContext _context;
        private readonly EmailService _emailService;


        public UserDataController(UserManager<ApplicationUser> userManager, graduationDbContext context, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }
       // [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.EmailParent,
                user.PhoneParent
            });
        }
       // [Authorize]
        [HttpPost("user/{id}")]
        public async Task<IActionResult> UpdateUserById(string id, [FromForm] UpdateUserRequest model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid password. Please enter correct password." });

            if (!string.IsNullOrWhiteSpace(model.UserName) && model.UserName != user.UserName)
            {
                user.UserName = model.UserName;
                user.NormalizedUserName = model.UserName.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(model.EmailParent) && model.EmailParent != user.EmailParent)
            {
                user.EmailParent = model.EmailParent;
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneParent) && model.PhoneParent != user.PhoneParent)
            {
                user.PhoneParent = model.PhoneParent;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            await _emailService.SendProfileUpdatedEmailAsync(user.Email, "Your Profile Has Been Updated");
            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUserById(string id, [FromBody] PasswordConfirmationRequest model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid password. Please enter correct password." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

             await _emailService.SendAccountDeletedEmailAsync(user.Email, "Your AI Tutor Account Has Been Deleted");
            return Ok(new { message = "User deleted successfully" });
        }

    }
}
