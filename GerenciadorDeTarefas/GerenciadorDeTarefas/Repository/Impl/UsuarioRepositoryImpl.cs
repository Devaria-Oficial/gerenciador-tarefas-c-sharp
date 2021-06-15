using GerenciadorDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Repository.Impl
{
    public class UsuarioRepositoryImpl : IUsuarioRepository
    {
        private readonly GerenciadorDeTarefasContext _contexto;
        public UsuarioRepositoryImpl(GerenciadorDeTarefasContext contexto)
        {
            _contexto = contexto;
        }

        public bool ExisteUsuarioPeloEmail(string email)
        {
            return _contexto.Usuario.Any(usuario => usuario.Email.ToLower() == email.ToLower());
        }

        public Usuario GetById(int idUsuario)
        {
            return _contexto.Usuario.FirstOrDefault(usuario => usuario.Id == idUsuario);
        }

        public Usuario GetUsuarioByLoginSenha(string login, string senha)
        {
            return _contexto.Usuario.FirstOrDefault(usuario => usuario.Email == login.ToLower() && usuario.Senha == senha);
        }

        public void Salvar(Usuario usuario)
        {
            _contexto.Usuario.Add(usuario);
            _contexto.SaveChanges();
        }
    }
}
