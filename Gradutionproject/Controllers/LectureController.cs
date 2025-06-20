using Gradutionproject.AuthServices;
using Gradutionproject.Context;
using Gradutionproject.Dtos;
using Gradutionproject.Models;
using Gradutionproject.UdateModelsDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gradutionproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly graduationDbContext _context;
        private readonly FileUploadService _fileUploadService;


        public LectureController(graduationDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lecture>>> GetLectures()
        {
            //.Include(c => c.Course)
            var lectures = await _context.Lectures.Include(l => l.Sections)
                .Include(l => l.Course)
                .Select(l => new LectureReadDto
                {
                    LectureId = l.Id,
                    Title = l.Title,
                    LecturePDF = l.FileName,
                    CourseId = l.CourseId,
                    TitleCourse = l.Course.Title,
                    AdminId = l.AdminId


                })
        .ToListAsync();

            return Ok(lectures);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<LectureReadDto>> GetLecture(int id)
        {
            var lecture = await _context.Lectures
                            .Include(l => l.Course)
                            .FirstOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }
            var dto = new LectureReadDto
            {
                LectureId = lecture.Id,
                Title = lecture.Title,
                LecturePDF = lecture.FileName,
                CourseId = lecture.CourseId,
                AdminId = lecture.AdminId,
                TitleCourse = lecture.Course.Title

            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLecture([FromForm] LectureCreateDto dto)
        {
            var lectureExists = await _context.Lectures
            .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower());

            if (lectureExists)
            {
                return BadRequest("A Lecture with the same name already exists.");
            }

            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
            if (!adminExists)
            {
                return BadRequest("Admin not found.");
            }
            if (dto.LecturePDF == null)
            {
                return BadRequest("No file uploaded.");
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }
            var invalidChars = Path.GetInvalidFileNameChars();
            if (dto.Title.Any(c => invalidChars.Contains(c)))
            {
                return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
            }
            var CourseTitle = course.Title;

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", CourseTitle, "Lectures");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.LecturePDF.FileName);
            var newPath = Path.Combine(uploadPath, uniqueFileName);
            try
            {

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.LecturePDF.CopyToAsync(stream);
                }
                var lecture = new Lecture
                {
                    Title = dto.Title,
                    FileName = uniqueFileName,
                    CourseId = dto.CourseId,
                    AdminId = dto.AdminId
                };
                _context.Lectures.Add(lecture);
                await _context.SaveChangesAsync();
                var result = new LectureReadDto
                {
                    AdminId = lecture.AdminId,
                    CourseId = course.Id,
                    TitleCourse = course.Title,
                    LectureId = lecture.Id,
                    Title = lecture.Title,
                    LecturePDF = lecture.FileName

                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("update/{LectureId}")]
        public async Task<IActionResult> UpdateLecture(int LectureId, [FromForm] LectureUpdateDto dto)
        {
            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == LectureId);
            if (lecture == null)
            {
                return NotFound("Lecture not found.");
            }
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                var lectureExists = await _context.Lectures
                    .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower() && c.Id != LectureId);
                if (lectureExists)
                {
                    return BadRequest("A Lecture with the same name already exists.");
                }
                var invalidChars = Path.GetInvalidFileNameChars();
                if (dto.Title.Any(c => invalidChars.Contains(c)))
                {
                    return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
                }
                lecture.Title = dto.Title;
            }

            if (dto.AdminId.HasValue)
            {
                var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
                if (!adminExists)
                {
                    return BadRequest("Admin not found.");
                }
                lecture.AdminId = dto.AdminId.Value;
            }
            int courseIdToUse;
            if (dto.CourseId.HasValue)
            {
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId.Value);
                if (!courseExists)
                {
                    return BadRequest("Course not found.");
                }
                lecture.CourseId = dto.CourseId.Value;
                courseIdToUse = dto.CourseId.Value;
            }
            else
            {
                courseIdToUse = lecture.CourseId;
            }

            // Update pdf if send it 
            if (dto.LecturePDF != null)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseIdToUse);
                if (course == null)
                {
                    return BadRequest("Associated course not found.");
                }
                var CourseTitle = course.Title;
                var oldFileName = lecture.FileName;
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    var isFileUsedElsewhere = await _context.Lectures
                    .AnyAsync(s => s.Id != lecture.Id && s.FileName == oldFileName);
                    if (!isFileUsedElsewhere)
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", CourseTitle, "Lectures", oldFileName);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", CourseTitle, "Lectures");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.LecturePDF.FileName);
                var newPath = Path.Combine(uploadPath, uniqueFileName);
                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.LecturePDF.CopyToAsync(stream);
                }

                lecture.FileName = uniqueFileName;
            }

            await _context.SaveChangesAsync();
            var result = new LectureReadDto
            {
                AdminId = lecture.AdminId,
                LectureId = lecture.Id,
                Title = lecture.Title,
                LecturePDF = lecture.FileName,
                CourseId = lecture.CourseId,
                TitleCourse = lecture.Course.Title

            };
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _context.Lectures
                   .Include(s => s.Sections).Include(l => l.Course)
                   .FirstOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
                return NotFound(new { message = "Lecture not found." });

            var CourseTitle = lecture.Course.Title;
            foreach (var section in lecture.Sections)
            {
                Console.WriteLine($"Trying to delete section file: {section.FileName}");

                if (!string.IsNullOrEmpty(section.FileName))
                {
                    var sectionFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", CourseTitle, "Lectures", section.FileName);
                    if (System.IO.File.Exists(sectionFilePath))
                    {
                        System.IO.File.Delete(sectionFilePath);
                        Console.WriteLine("File deleted: " + sectionFilePath);
                    }
                    else
                    {
                        Console.WriteLine("File NOT found: " + sectionFilePath);
                    }
                }
            }
            if (!string.IsNullOrEmpty(lecture.FileName))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", CourseTitle, "Lectures", lecture.FileName);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            try
            {
                _context.Lectures.Remove(lecture);
                await _context.SaveChangesAsync();
                return Content("The lecture was deleted with its section.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
