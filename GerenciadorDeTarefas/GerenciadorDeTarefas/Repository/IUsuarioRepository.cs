using GerenciadorDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Repository
{
    public interface IUsuarioRepository
    {
        public void Salvar(Usuario usario);
    }
}
