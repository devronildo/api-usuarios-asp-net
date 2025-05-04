using ApiUsuario.Data;
using ApiUsuario.Dto.Login;
using ApiUsuario.Dto.Usuario;
using ApiUsuario.Models;
using ApiUsuario.Services.Senha;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuario.Services.Usuario
{
    public class UsuarioService : IUsuarioInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;

        public UsuarioService(AppDbContext context, ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }

        public async Task<ResponseModel<UsuarioModel>> BuscarUsuarioPorId(int Id)
        {
           ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario =  await _context.Usuarios.FindAsync(Id);

                if (usuario == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }
                
                response.Dados = usuario;
              
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }


        }

        public async Task<ResponseModel<UsuarioModel>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
               var usuarioBanco = await _context.Usuarios.FindAsync(usuarioEdicaoDto.Id);

                if(usuarioBanco == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }


                usuarioBanco.Usuario = usuarioEdicaoDto.Usuario;
                usuarioBanco.Nome = usuarioEdicaoDto.Nome;
                usuarioBanco.Sobrenome = usuarioEdicaoDto.Sobrenome;
                usuarioBanco.Email = usuarioEdicaoDto.Email;
                usuarioBanco.DataAlteracao = DateTime.Now;
                

                _context.Update(usuarioBanco);
                await _context.SaveChangesAsync();

                response.Dados = usuarioBanco;
                response.Mensagem = "Usuário Editado com sucesso";
                response.Status = true;
                return response;

            }
            catch (Exception ex) {
                 response.Mensagem = ex.Message;
                 response.Status = false;
                 return response;
            }
        }

        public async Task<ResponseModel<List<UsuarioModel>>> ListarUsuarios()
        {
            ResponseModel<List<UsuarioModel>> response = new ResponseModel<List<UsuarioModel>>();

            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                response.Dados = usuarios;
        
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> Login(UsuarioLoginDto usuarioLoginDto)
        {
           ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(ub => ub.Email == usuarioLoginDto.Email);

                if (usuario == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    response.Status = false;
                    return response;
                }
                if (!_senhaInterface.VerificaSenhaHash(usuarioLoginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    response.Mensagem = "Credenciais inválidas";
                    response.Status = false;
                    return response;
                }
                var token = _senhaInterface.CriarToken(usuario);

                usuario.Token = token;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                
                response.Dados = usuario;
                response.Mensagem = "Usuário logado com sucesso!";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message; 
                response.Status = false;
                return response;

            }

        }

        public async Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();
            try
            {
                if (!VerificaSeExisteEmailUsuarioRepetidos(usuarioCriacaoDto))
                {
                    response.Mensagem = "Email/Usuario já cadastrado!";
                    response.Status = false;
                    return response;
                }

                _senhaInterface.CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                UsuarioModel usuario = new UsuarioModel()
                {
                      Usuario = usuarioCriacaoDto.Usuario,
                      Email = usuarioCriacaoDto.Email,
                      Nome = usuarioCriacaoDto.Nome,
                      Sobrenome = usuarioCriacaoDto.Sobrenome,
                      SenhaHash = senhaHash,
                      SenhaSalt = senhaSalt
                };
                
                _context.Add(usuario);
               await _context.SaveChangesAsync();

               response.Mensagem = "Usuário cadastrado com sucesso!";
               response.Dados = usuario;
               return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> RemoverUsuario(int id)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuarioBanco = await _context.Usuarios.FindAsync(id);

                if (usuarioBanco == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }
                response.Mensagem = "Usuário removido com sucesso!";
                response.Status = true;
                response.Dados = usuarioBanco;

                _context.Usuarios.Remove(usuarioBanco);
                await _context.SaveChangesAsync();
               
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        private bool VerificaSeExisteEmailUsuarioRepetidos(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioCriacaoDto.Email || 
                                                          u.Usuario == usuarioCriacaoDto.Usuario);

            if (usuario != null)
            {
                return false;
            }
            return true;    

        }
    }
}
