using Gradutionproject.Context;
using Gradutionproject.Dtos;
using Gradutionproject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gradutionproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly graduationDbContext _context;

        public AdminController(graduationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminReadDto>>> GetAdmins()
        {

            var admin = await _context.Admins.ToListAsync();
            var adminDto = admin.Select(a => new AdminReadDto
            {
                AdminId = a.AdminId,
                Username = a.Username,
                Email = a.Email,
                Password = a.AdminPassword
            });
            return Ok(adminDto);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminReadDto>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();
            var dto = new AdminReadDto
            {
                AdminId = admin.AdminId,
                Username = admin.Username,
                Email = admin.Email,
                Password = admin.AdminPassword

            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<Admin>> CreateAdmin([FromForm] AdminCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email Required");
            var emailExists = await _context.Admins
              .AnyAsync(a => a.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                return Conflict(new { message = "Email already in use." });
            }
            var existingAdmin = await _context.Admins
               .FirstOrDefaultAsync(a => a.Username.ToLower() == dto.Username.ToLower());
            if (existingAdmin != null)
            {
                return Conflict(new { message = "Username already exists." });
            }
            var hasher = new PasswordHasher<object>();

            var admin = new Admin
            {
                Username = dto.Username,
                AdminPassword = hasher.HashPassword(null, dto.Password),
                Email = dto.Email

            };
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            var response = new AdminReadDto
            {
                AdminId = admin.AdminId,
                Username = admin.Username,
                Email = admin.Email,
                Password = admin.AdminPassword
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found." });
            }

            try
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
                return Content("The admin was deleted.");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("DELETE statement conflicted") == true)
                {
                    return BadRequest(new
                    {
                        message = "Cannot delete this admin because it is associated with other data such as courses."
                    });
                }

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while trying to delete the admin.",
                    error = ex.Message
                });
            }
        }
    }
}
