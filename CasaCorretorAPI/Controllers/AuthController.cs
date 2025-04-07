using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Controller responsável pela autenticação de usuários e geração de tokens JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Construtor que recebe a instância da configuração da aplicação (IConfiguration).
    /// </summary>
    /// <param name="config">Instância da configuração injetada via DI.</param>
    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Endpoint de login. Valida as credenciais e retorna um token JWT em caso de sucesso.
    /// </summary>
    /// <param name="login">Objeto contendo o nome de usuário e senha.</param>
    /// <returns>Retorna um token JWT se autenticado com sucesso, ou 401 se falhar.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        // Dados para testar a autenticação
        if (login.Username == "admin" && login.Password == "123")
        {
            var token = GenerateToken(login.Username);
            return Ok(new { token });
        }

        return Unauthorized();
    }

    /// <summary>
    /// Gera um token JWT com base no nome de usuário fornecido.
    /// </summary>
    /// <param name="username">Nome do usuário autenticado.</param>
    /// <returns>Uma string contendo o token JWT gerado.</returns>
    private string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único do token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Modelo de dados usado para login, contendo nome de usuário e senha.
/// </summary>
/// <param name="Username">Nome do usuário.</param>
/// <param name="Password">Senha do usuário.</param>
public record LoginModel(string Username, string Password);
