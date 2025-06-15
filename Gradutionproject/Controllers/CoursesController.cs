using Gradutionproject.Context;
using Gradutionproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Gradutionproject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CoursesController : ControllerBase
	{
		private readonly graduationDbContext _context;
        private readonly ILogger<CoursesController> _logger;

       
        public CoursesController(graduationDbContext context, ILogger<CoursesController> logger)
		{
			_context = context;
            _logger = logger;
		}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCourses()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/Images/";

            var courses = await _context.Courses
                .Select(course => new {
                    course.Id,
                    course.Title,
                    ImageUrl = baseUrl + course.ImageName
                })
                .ToListAsync();

            return Ok(courses);
        }

        // عرض المحاضرات حسب الكورس المختار

        [HttpGet("courses/{courseId}/lectures")]
        public async Task<ActionResult<IEnumerable<Lecture>>> GetLecturesByCourse(int courseId)
        {
            var lectures = await _context.Lectures
                                         .Where(l => l.CourseId == courseId)
                                         .ToListAsync();

            if (lectures == null || lectures.Count == 0)
                return NotFound("No lectures found for this course.");

            return Ok(lectures);
        }
        //// عرض السكاشن حسب المحاضرة المختارة
        [HttpGet("lectures/{lectureId}/sections")]
        public async Task<ActionResult<IEnumerable<Section>>> GetSectionsByLecture(int lectureId)
        {
            var sections = await _context.Sections
                                         .Where(s => s.LectureId == lectureId)
                                         .ToListAsync();

            if (sections == null || sections.Count == 0)
                return NotFound("No sections found for this lecture.");

            return Ok(sections);
        }
        // لعرض ملف المحاضره داخل التطبيق:
        [HttpGet("courses/{courseId}/lectures/{lectureId}/file")]
        public async Task<IActionResult> ViewLectureFile(int courseId, int lectureId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound("Course not found.");

            var lecture = await _context.Lectures
                .Where(l => l.Id == lectureId && l.CourseId == courseId)
                .FirstOrDefaultAsync();

            if (lecture == null)
                return NotFound("Lecture not found or does not belong to the specified course.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title, "Lectures", lecture.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var mimeType = "application/pdf";

            // ✅ لا ترجع اسم الملف عشان ما يتحمّل تلقائي
            return File(fileBytes, mimeType);
        }
        // لعرض ملف السكشن داخل التطبيق

        [HttpGet("lectures/{lectureId}/sections/{sectionId}/file")]
        public async Task<IActionResult> ViewSectionFile(int lectureId, int sectionId)
        {
            // نجيب المحاضرة ونشمل الكورس عشان نعرف مكان الملف
            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lectureId);

            if (lecture == null)
                return NotFound("Lecture not found.");

            // نتاكد ان السكشن فعلاً ينتمي للمحاضرة
            var section = await _context.Sections
                .Where(s => s.Id == sectionId && s.LectureId == lectureId)
                .FirstOrDefaultAsync();

            if (section == null)
                return NotFound("Section not found or does not belong to the lecture.");

            // بناء مسار الملف
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", lecture.Course.Title, "Sections", section.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var mimeType = "application/pdf";
            Response.Headers.Add("Content-Disposition", "inline; filename=" + section.FileName);

            return File(fileBytes, mimeType); // ✅ بدون اسم الملف عشان يتم العرض مش التحميل
        }


        //لتحميل الملفات بدف
        //[HttpGet("courses/{courseId}/lectures/{lectureId}/file")]
        //public async Task<IActionResult> DownloadLectureFile(int courseId, int lectureId)
        //{
        //    var course = await _context.Courses.FindAsync(courseId);
        //    if (course == null)
        //        return NotFound("Course not found.");

        //    var lecture = await _context.Lectures
        //        .Where(l => l.Id == lectureId && l.CourseId == courseId)
        //        .FirstOrDefaultAsync();

        //    if (lecture == null)
        //        return NotFound("Lecture not found or does not belong to the specified course.");

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title, "Lectures", lecture.FileName);

        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound("File not found.");

        //    var mimeType = "application/pdf";
        //    return PhysicalFile(filePath, mimeType, lecture.FileName);
        //}

        //تحميل الملف بدف
        //[HttpGet("lectures/{lectureId}/sections/{sectionId}/file")]
        //public async Task<IActionResult> DownloadSectionFile(int lectureId, int sectionId)
        //{
        //    // نجيب المحاضرة عشان نعرف اسم الكورس
        //    var lecture = await _context.Lectures
        //        .Include(l => l.Course)
        //        .FirstOrDefaultAsync(l => l.Id == lectureId);

        //    if (lecture == null)
        //        return NotFound("Lecture not found.");

        //    // نتاكد ان السكشن تبع نفس الكورس
        //    var section = await _context.Sections
        //        .Where(s => s.Id == sectionId && s.Id == lecture.CourseId)
        //        .FirstOrDefaultAsync();

        //    if (section == null)
        //        return NotFound("Section not found or does not belong to the lecture's course.");

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", lecture.Course.Title, "Sections", section.FileName);

        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound("File not found.");

        //    var mimeType = "application/pdf";
        //    return PhysicalFile(filePath, mimeType, section.FileName);
        //}
        //1. Endpoint لرفع ملف محاضره وتخزينه في مكانه
        [HttpPost("courses/{courseId}/lectures/upload")]
        public async Task<IActionResult> UploadLectureFile(int courseId, IFormFile file)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound("Course not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var courseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", course.Title, "Lectures");

            // تأكد أن الفولدر موجود
            if (!Directory.Exists(courseFolder))
                Directory.CreateDirectory(courseFolder);

            var filePath = Path.Combine(courseFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // نحفظ اسم الملف بس في الداتا بيز كمثال
            var lecture = new Lecture
            {
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                FileName = file.FileName,
                CourseId = courseId
            };

            _context.Lectures.Add(lecture);
            await _context.SaveChangesAsync();

            return Ok(lecture);
        }
        //1. Endpoint لرفع ملف سكشن وتخزينه في مكانه
        [HttpPost("lectures/{lectureId}/sections/upload")]
        public async Task<IActionResult> UploadSectionFile(int lectureId, IFormFile file)
        {
            // نجيب المحاضرة ونجيب معاها الكورس المرتبط بيها
            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lectureId);

            if (lecture == null)
                return NotFound("Lecture not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // نستخدم اسم الكورس لتحديد مسار الملف
            var courseName = lecture.Course.Title;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", courseName, "Sections");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // نحفظ بيانات السكشن في الداتا بيز وربطه بالمحاضرة
            var section = new Section
            {
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                FileName = file.FileName,
                LectureId = lectureId
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return Ok(section);
        }


    }
}

