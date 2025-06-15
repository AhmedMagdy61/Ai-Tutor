using Gradutionproject.Context;
using Gradutionproject.Dtos;
using Gradutionproject.Models;
using Gradutionproject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]

[ApiController]
public class AuthController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IConfiguration _configuration;
    private readonly graduationDbContext _context;
    private readonly EmailService _emailService;
    private readonly IMemoryCache _cache;
    public AuthController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration, 
        graduationDbContext context, EmailService emailService,
        IMemoryCache cache)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_configuration = configuration;
        _context = context;
        _emailService = emailService;
        _cache = cache;

    }

    [HttpPost("register")]
	public async Task<IActionResult> Register([FromForm] RegisterModel model)
	{
		var user = new ApplicationUser
		{
			UserName = model.Username,
			Email = model.Email,
			EmailParent = model.EmailParent,
			PhoneParent = model.PhoneParent
		};

		var result = await _userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			return BadRequest(result.Errors);
		}

		return Ok("User registered successfully.");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromForm] LoginModel model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
		{
			return Unauthorized("Invalid email or password.");
		}

		var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
		if (!result.Succeeded)
		{
			return Unauthorized("Invalid email or password.");
		}

		var token = GenerateJwtToken(user);

		return Ok(new
		{
			message = "Login successful",
			token = token,
			userId = user.Id,
			email = user.Email
		});
	}




    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
        new Claim("username", user.UserName ?? "Unknown"),
        new Claim("role", "User")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromForm] ForgotPasswordDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email is required.");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Email not found.");

        var code = new Random().Next(100000, 999999).ToString();
        var cacheKey = $"reset-code-{dto.Email}";

        _cache.Set(cacheKey, code, TimeSpan.FromMinutes(5)); // حفظ الكود لمدة 5 دقائق

        var resetLink = $"https://localhost:7032/reset-password?token={code}&email={dto.Email}";
        await _emailService.SendResetPasswordEmailAsync(user.Email, resetLink, "Reset Your Password", code);
        return Ok("Reset code sent to email.");
    }

    [HttpPost("verify-code")]
    public IActionResult VerifyResetCode([FromForm] VerifyCodeDto dto)
    {
        var cacheKey = $"reset-code-{dto.Email}";
        var verifiedKey = $"verified-reset-{dto.Email}";
        if (!_cache.TryGetValue(cacheKey, out string correctCode))
        {
            return BadRequest("Code expired or not found.");
        }

        if (correctCode != dto.Code)
        {
            return BadRequest("Invalid code.");
        }
        _cache.Set(verifiedKey, true, TimeSpan.FromMinutes(5));
        return Ok("Code verified successfully.");
    }

    [HttpPost("Reset-Password")]
    public async Task<IActionResult> Reset_Password([FromForm] ResetPasswordDto dto)
    {
        var verifiedKey = $"verified-reset-{dto.Email}";

        if (!_cache.TryGetValue(verifiedKey, out bool isVerified) || !isVerified)
        {
            return BadRequest("Code verification required before resetting password.");
        }

        var user = await _userManager.FindByEmailAsync(dto.Email.Trim());
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest("Passwords do not match.");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        _cache.Remove(verifiedKey);
        _cache.Remove($"reset-code-{dto.Email}"); // Remove the code after reset success
        await _emailService.SendPasswordChangedConfirmationEmailAsync(user.Email);
        return Ok("Password reset successful.");
    }

}




