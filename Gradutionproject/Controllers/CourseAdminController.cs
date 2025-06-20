using Gradutionproject.Context;
using Gradutionproject.Dtos;
using Gradutionproject.Models;
using Gradutionproject.UdateModelsDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Gradutionproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseAdminController : ControllerBase
    {
        private readonly graduationDbContext _context;

        public CourseAdminController(graduationDbContext context)
        {
            _context = context;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.Include(c => c.Lectures).ThenInclude(l => l.Sections).ToListAsync();
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = _context.Courses
                        .Include(c => c.Lectures)
                        .ThenInclude(l => l.Sections)
                        .FirstOrDefault(c => c.Id == id);

            if (course == null)
                return NotFound("Not Found");

            return course;
        }

        // POST: api/Course
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse([FromForm] CourseDto dto)
        {
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
            if (!adminExists)
            {
                return BadRequest("Admin not found.");
            }
            var courseExists = await _context.Courses
              .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower());

            if (courseExists)
            {
                return BadRequest("A course with the same name already exists.");
            }

            if (dto.Photo == null)
            {
                return BadRequest("No file uploaded.");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(dto.Photo.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            if (dto.Title.Any(c => invalidChars.Contains(c)))
            {
                return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
            }
            var courseFolderName = dto.Title.Trim();

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Photo.FileName);
            var newPath = Path.Combine(uploadPath, uniqueFileName);
            try
            {

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.Photo.CopyToAsync(stream);
                }

                var course = new Course
                {
                    Title = dto.Title,
                    AdminId = dto.AdminId,
                    ImageName = uniqueFileName,
                    Lectures = new List<Lecture>()
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                var courseWithLectures = await _context.Courses
                        .Include(c => c.Lectures)
                        .FirstOrDefaultAsync(c => c.Id == course.Id);


                return Ok(courseWithLectures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("update/{CourseID}")]
        public async Task<IActionResult> UpdateSection(int CourseID, [FromForm] CourseUpdateDto dto)
        {
            var course = await _context.Courses.FindAsync(CourseID);

            if (course == null)
            {
                return NotFound("Course not found.");
            }
            if (dto.AdminId.HasValue)
            {
                var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
                if (!adminExists)
                {
                    return BadRequest("Admin not found.");
                }
                course.AdminId = dto.AdminId.Value;
            }
            var oldTitle = course.Title;
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                var courseExists = await _context.Courses
                    .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower() && c.Id != CourseID);

                if (courseExists)
                {
                    return BadRequest("A course with the same name already exists.");
                }
                var invalidChars = Path.GetInvalidFileNameChars();
                if (dto.Title.Any(c => invalidChars.Contains(c)))
                {
                    return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
                }
                if (!oldTitle.Equals(dto.Title, StringComparison.OrdinalIgnoreCase))
                {
                    var oldFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", oldTitle.Trim());
                    var newFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", dto.Title.Trim());

                    if (Directory.Exists(oldFolder))
                    {
                        Directory.Move(oldFolder, newFolder); // rename
                    }
                }
                course.Title = dto.Title;
            }

            if (dto.Photo != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(dto.Photo.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                }
                var oldFileName = course.ImageName;
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    var isFileUsedElsewhere = await _context.Courses
                                      .AnyAsync(s => s.Id != course.Id && s.ImageName == oldFileName);
                    if (!isFileUsedElsewhere)
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title, oldFileName);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                }
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Photo.FileName);
                var newPath = Path.Combine(uploadPath, uniqueFileName);
                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.Photo.CopyToAsync(stream);
                }

                course.ImageName = uniqueFileName;
            }

            await _context.SaveChangesAsync();
            var courseWithLectures = await _context.Courses
                  .Include(c => c.Lectures).ThenInclude(s => s.Sections)
                  .FirstOrDefaultAsync(c => c.Id == CourseID);
            return Ok(courseWithLectures);
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, recursive: true); // recursive = true لمسح كل اللي جواه
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return Content("The Course was deleted with its Lectures and Sections.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
