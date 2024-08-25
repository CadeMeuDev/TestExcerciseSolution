using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IOptions<AuthUserSettings> authUserSettingsOptions)
    : ControllerBase
{
    private readonly AuthUserSettings _authUserSettings = authUserSettingsOptions.Value;

    [HttpGet("user")]
    [ProducesResponseType(200, Type = typeof(string))]
    public IActionResult Signin()
    {
        var issuer = _authUserSettings.Issuer;
        var audience = _authUserSettings.Audience;
        var key = Encoding.ASCII.GetBytes(_authUserSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        return Ok(stringToken);
    }
}