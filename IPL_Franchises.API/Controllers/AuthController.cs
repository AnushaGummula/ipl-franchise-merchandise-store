using System.Security.Claims;
using IPL_Franchises.API.Services;
using IPL_Franchises.Application.DTOs.Auth;
using IPL_Franchises.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IPL_Franchises.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly JwtTokenService
        _jwtTokenService;


    public AuthController(
        UserManager<ApplicationUser> userManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var email =
            request.Email.Trim();


        var existingUser =
            await _userManager
                .FindByEmailAsync(email);


        if (existingUser != null)
        {
            return Conflict(new
            {
                message =
                    "An account with this email already exists."
            });
        }


        var user =
            new ApplicationUser
            {
                FullName =
                    request.FullName.Trim(),

                UserName = email,

                Email = email,

                CreatedAt =
                    DateTime.UtcNow
            };


        var result =
            await _userManager
                .CreateAsync(
                    user,
                    request.Password);


        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message =
                    "Unable to create account.",

                errors =
                    result.Errors.Select(
                        error =>
                            error.Description)
            });
        }


        var response =
            _jwtTokenService
                .CreateToken(user);


        return Ok(response);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var email =
            request.Email.Trim();


        var user =
            await _userManager
                .FindByEmailAsync(email);


        if (user == null)
        {
            return Unauthorized(new
            {
                message =
                    "Invalid email or password."
            });
        }


        var passwordValid =
            await _userManager
                .CheckPasswordAsync(
                    user,
                    request.Password);


        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message =
                    "Invalid email or password."
            });
        }


        var response =
            _jwtTokenService
                .CreateToken(user);


        return Ok(response);
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier),

            fullName =
                User.FindFirstValue(
                    ClaimTypes.Name),

            email =
                User.FindFirstValue(
                    ClaimTypes.Email)
        });
    }
}
