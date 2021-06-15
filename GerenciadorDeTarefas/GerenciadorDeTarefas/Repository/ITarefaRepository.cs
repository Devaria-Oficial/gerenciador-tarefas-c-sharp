using GerenciadorDeTarefas.Enums;
using GerenciadorDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Repository
{
    public interface ITarefaRepository
    {
        void AdicionarTarefa(Tarefa tarefa);
        Tarefa GetById(int idTarefa);
        void RemoverTarefa(Tarefa tarefa);
        void AtualizarTarefa(Tarefa tarefa);
        List<Tarefa> BuscarTarefas(int idUsuario, DateTime? periodoDe, DateTime? periodoAte, StatusTarefaEnum status);
    }
}
