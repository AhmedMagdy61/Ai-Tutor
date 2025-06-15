using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gradutionproject.Context;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gradutionproject.Dtos;
using Gradutionproject.Models;

namespace Gradutionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly graduationDbContext _context;

        public QuizController(graduationDbContext context)
        {
            _context = context;
        }

        // ✅ 1. عرض كل المستخدمين ودرجاتهم
        [HttpGet("all-with-quizzes")]
        public async Task<ActionResult<IEnumerable<UserWithQuizzesDTO>>> GetAllUsersWithQuizzes()
        {
            var users = await _context.Users
                .Include(u => u.Quizzes)
                    .ThenInclude(q => q.Course)
                .Select(u => new UserWithQuizzesDTO
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Quizzes = u.Quizzes.Select(q => new QuizDto
                    {
                        QuizId = q.Id,
                        Score = q.Score,
                        QuizDate = q.QuizDate,
                        CourseName = q.Course.Title
                    }).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ 2. عرض مستخدم معيّن بالدرجات
        [HttpGet("{userId}/quizzes")]
        public async Task<ActionResult<UserWithQuizzesDTO>> GetUserWithQuizzes(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Quizzes)
                    .ThenInclude(q => q.Course)
                .Where(u => u.Id == userId)
                .Select(u => new UserWithQuizzesDTO
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Quizzes = u.Quizzes.Select(q => new QuizDto
                    {
                        QuizId = q.Id,
                        Score = q.Score,
                        QuizDate = q.QuizDate,
                        CourseName = q.Course.Title
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddQuiz([FromBody] CreateQuizDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return NotFound("Course not found.");

            var quiz = new Quiz
            {
                Score = dto.Score,
                UserId = dto.UserId,
                UserName = user.UserName,
                CourseId = dto.CourseId,
                QuizDate = DateTime.Now
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quiz saved successfully.",
                quizId = quiz.Id
            });
        }

    }
}

