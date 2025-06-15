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

        public UserDataController(UserManager<ApplicationUser> userManager, graduationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
        public async Task<IActionResult> UpdateUserById(string id, [FromBody] UpdateUserRequest model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.NormalizedUserName = model.UserName?.ToUpper();
            user.NormalizedEmail = model.Email?.ToUpper();
            user.EmailParent = model.EmailParent;
            user.PhoneParent = model.PhoneParent;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "User updated successfully" });
        }
    }
}
