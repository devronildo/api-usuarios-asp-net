using ApiUsuario.Dto.Login;
using ApiUsuario.Dto.Usuario;
using ApiUsuario.Models;

namespace ApiUsuario.Services.Usuario
{
    public interface IUsuarioInterface
    {
        Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto);
        Task<ResponseModel<List<UsuarioModel>>> ListarUsuarios();

        Task<ResponseModel<UsuarioModel>> BuscarUsuarioPorId(int Id);

        Task<ResponseModel<UsuarioModel>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto); 

        Task<ResponseModel<UsuarioModel>> RemoverUsuario(int id);

        Task<ResponseModel<UsuarioModel>> Login(UsuarioLoginDto usuarioLoginDto);


    }
}
