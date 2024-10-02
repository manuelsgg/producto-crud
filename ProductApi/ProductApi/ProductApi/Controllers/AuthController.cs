using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmpresaContext dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(EmpresaContext _dbContext, IConfiguration configuration)
        {
            dbContext = _dbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            try
            {
                var user = dbContext.Usuarios.SingleOrDefault(u => u.Email == login.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                    return Unauthorized();

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token, Role = user.Role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al autentificar: " + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] Usuario newUser)
        {
            try
            {
                if (!ValidarEmail(newUser.Email))
                {
                    return BadRequest("El formato del email es incorrecto.");
                }

                if (dbContext.Usuarios.Any(u => u.Email == newUser.Email))
                {
                    return BadRequest("El email ya está en uso.");
                }

                newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
                dbContext.Usuarios.Add(newUser);
                dbContext.SaveChanges();
                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear usuario: " + ex.Message);
            }
        }

        [HttpPatch("edit/{id}")]
        public IActionResult EditUser(int id, [FromBody] Usuario updatedUser)
        {
            try
            {
                var user = dbContext.Usuarios.Find(id);
                if (user == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);
                }

                if (!string.IsNullOrEmpty(updatedUser.Role))
                {
                    user.Role = updatedUser.Role;
                }

                dbContext.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al editar usuario: " + ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = dbContext.Usuarios.Find(id);
                if (user == null)
                    return NotFound();

                dbContext.Usuarios.Remove(user);
                dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar usuario: " + ex.Message);
            }
        }

        private bool ValidarEmail(string email)
        {
            try
            {
                var vEmail = new System.Net.Mail.MailAddress(email);
                return vEmail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Role, usuario.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
