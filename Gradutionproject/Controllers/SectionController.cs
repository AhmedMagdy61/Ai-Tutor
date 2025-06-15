using Gradutionproject.AuthServices;
using Gradutionproject.Context;
using Gradutionproject.Dtos;
using Gradutionproject.Models;
using Gradutionproject.UdateModelsDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gradutionproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly graduationDbContext _context;
        private readonly FileUploadService _fileUploadService;


        public SectionController(graduationDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections()
        {
            return await _context.Sections.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SectionReadDto>> GetSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
                return NotFound("Section Not Exist");
            var dto = new SectionReadDto
            {
                SectionId = section.Id,
                Title = section.Title,
                LectureId = section.LectureId,
                AdminId = section.AdminId,
                SectionPDF = section.FileName

            };
            return Ok(dto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSection([FromForm] SectionCreateDto dto)
        {
            var sectionExists = await _context.Sections
            .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower());

            if (sectionExists)
            {
                return BadRequest("A Section with the same name already exists.");
            }
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
            if (!adminExists)
            {
                return BadRequest("Admin not found.");
            }

            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == dto.LectureId);

            if (lecture == null)
            {
                return BadRequest("Lecture not found.");
            }
            if (dto == null)
            {
                return BadRequest("The input data is null.");
            }
            var invalidChars = Path.GetInvalidFileNameChars();
            if (dto.Title.Any(c => invalidChars.Contains(c)))
            {
                return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
            }
            var courseTitle = lecture.Course?.Title ?? "UnknownCourse";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", courseTitle, "Sections");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.SectionPDF.FileName);
            var newPath = Path.Combine(uploadPath, uniqueFileName);

            try
            {
                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.SectionPDF.CopyToAsync(stream);
                }
                var section = new Section
                {
                    Title = dto.Title,
                    FileName = uniqueFileName,
                    LectureId = dto.LectureId,
                    AdminId = dto.AdminId
                };
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();

                return Ok(section);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("update/{SectionID}")]
        public async Task<IActionResult> UpdateSection(int SectionID, [FromForm] SectionUpdateDto dto)
        {
            var section = await _context.Sections
                    .Include(s => s.Lecture)
                    .ThenInclude(l => l.Course)
                    .FirstOrDefaultAsync(s => s.Id == SectionID);
            if (section == null)
            {
                return NotFound("Section not found.");
            }
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                var sectionExists = await _context.Sections
                    .AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower() && c.Id != SectionID);

                if (sectionExists)
                {
                    return BadRequest("A Section with the same name already exists.");
                }
                var invalidChars = Path.GetInvalidFileNameChars();
                if (dto.Title.Any(c => invalidChars.Contains(c)))
                {
                    return BadRequest("Course title contains invalid characters (\\ / : * ? \" < > |).");
                }
                section.Title = dto.Title;
            }

            if (dto.AdminId.HasValue)
            {
                var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == dto.AdminId);
                if (!adminExists)
                {
                    return BadRequest("Admin not found.");
                }
                section.AdminId = dto.AdminId.Value;
            }

            if (dto.LectureId.HasValue)
            {
                var lectureExists = await _context.Lectures
                           .Include(l => l.Course)
                           .FirstOrDefaultAsync(c => c.Id == dto.LectureId);
                if (lectureExists == null)
                {
                    return BadRequest("Lecture not found.");
                }

                section.LectureId = dto.LectureId.Value;
                section.Lecture = lectureExists;
            }

            // تحديث الملف لو تم إرساله
            if (dto.SectionPDF != null)
            {
                // اسم الملف القديم
                var oldFileName = section.FileName;

                // ✅ تأكد إن الملف القديم مش مستخدم في Section تانية
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    var isFileUsedElsewhere = await _context.Sections
                        .AnyAsync(s => s.Id != section.Id && s.FileName == oldFileName);

                    if (!isFileUsedElsewhere)
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", section.Lecture?.Course?.Title ?? "UnknownCourse", "Sections", oldFileName);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                }

                // ✅ إنشاء مسار التخزين
                var courseTitle = section.Lecture?.Course?.Title ?? "UnknownCourse";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", section.Lecture?.Course?.Title ?? "UnknownCourse", "Sections");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // ✅ اسم فريد للملف الجديد
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.SectionPDF.FileName);
                var newPath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.SectionPDF.CopyToAsync(stream);
                }

                section.FileName = uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return Ok(section);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections
                .Include(s => s.Lecture)
                .ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(s => s.Id == id); if (section == null)
                return NotFound("Section not found.");


            if (!string.IsNullOrEmpty(section.FileName))
            {
                var courseTitle = section.Lecture?.Course?.Title ?? "UnknownCourse";
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", courseTitle, "Sections", section.FileName);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            try
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
                return Content("The section was deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
