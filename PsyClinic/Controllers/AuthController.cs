using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PsyClinic.Api.Services;
using PsyClinic.Api.ViewModels.Auth;
using PsyClinic.Infrasctructure.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public const string AccessTokenCookieName = "access_token";

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RequestUserViewModel request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.Phone,
            FederalRegistration = request.FederalRegistration
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var translatedErrors = result.Errors
                .Select(error => IdentityErrorTranslator.Translate(error.Code))
                .ToList();

            return BadRequest(CreateErrorResponse(400, translatedErrors));
        }

        return Ok(new ResponseUserViewModel
        {
            Message = "Usuário criado com sucesso!",
            Status = true,
            Code = 200
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(CreateErrorResponse(401, ["Usuário ou senha inválidos."]));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(CreateErrorResponse(401, ["Usuário ou senha inválidos."]));
        }

        var token = _jwtTokenService.GenerateToken(user);

        Response.Cookies.Append(AccessTokenCookieName, token, BuildAuthCookieOptions());

        return Ok(new ResponseUserViewModel
        {
            Message = "Login realizado com sucesso!",
            Status = true,
            Code = 200,
            Email = user.Email,
            UserName = user.UserName
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AccessTokenCookieName);

        return Ok(new ResponseUserViewModel
        {
            Message = "Você saiu do sistema!",
            Status = true,
            Code = 200
        });
    }

    private static ResponseUserViewModel CreateErrorResponse(int code, List<string> errors) => new()
    {
        Status = false,
        Code = code,
        Errors = errors,
        Message = code == 401 ? "Usuário ou senha inválidos." : "Falha ao processar a solicitação."
    };

    private static CookieOptions BuildAuthCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddMinutes(60),
        Path = "/"
    };
}
