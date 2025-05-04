using ApiUsuario.Dto.Login;
using ApiUsuario.Dto.Usuario;
using ApiUsuario.Services.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioInterface _usuarioInterface;

        public LoginController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = await _usuarioInterface.RegistrarUsuario(usuarioCriacaoDto);

            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = await _usuarioInterface.Login(usuarioLoginDto);

            return Ok(usuario);
         }  
    }
}
