using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InfantApp.WebApi.Host.Controllers;

public class UserModel
{
    public string Username { get; set; }
    public string EmailAddress { get; set; }
}

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : Controller
{
    private IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Sp(IServiceProvider serviceProvider)
    {
        return Ok();
    }
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody]UserModel login)
    {
        IActionResult response = Unauthorized();
        var user = AuthenticateUser(login);

        if (user != null)
        {
            var tokenString = GenerateJsonWebToken(user);
            response = Ok(new { token = tokenString });
        }

        return response;
    }

    [HttpPost]
    public string GenerateJsonWebToken(UserModel userInfo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            [
                new Claim(ClaimTypes.Email, userInfo.EmailAddress),
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Role, "Product.Admin")
            ],
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserModel AuthenticateUser(UserModel login)
    {
        UserModel user = null;

        //Validate the User Credentials
        //Demo Purpose, I have Passed HardCoded User Information
        if (login.Username == "Kasra")
        {
            user = new UserModel { Username = "Kasra Sh", EmailAddress = "testmail@gmail.com" };
        }
        return user;
    }
}