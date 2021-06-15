using GerenciadorDeTarefas.Enums;
using GerenciadorDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Repository.Impl
{
    public class TarefaRespositoryImpl : ITarefaRepository
    {
        private readonly GerenciadorDeTarefasContext _contexto;

        public TarefaRespositoryImpl(GerenciadorDeTarefasContext contexto)
        {
            _contexto = contexto;
        }

        public void AdicionarTarefa(Tarefa tarefa)
        {
            _contexto.Tarefa.Add(tarefa);
            _contexto.SaveChanges();
        }

        public void AtualizarTarefa(Tarefa tarefa)
        {
            _contexto.Entry(tarefa).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _contexto.SaveChanges();
            _contexto.Entry(tarefa).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        public List<Tarefa> BuscarTarefas(int idUsuario, DateTime? periodoDe, DateTime? periodoAte, StatusTarefaEnum status)
        {
            return _contexto.Tarefa.Where(tarefa => tarefa.IdUsuario == idUsuario
                             && (periodoDe == null || periodoDe == DateTime.MinValue || tarefa.DataPrevistaConclusao >= ((DateTime)periodoDe).Date)
                             && (periodoAte == null || periodoAte == DateTime.MinValue || tarefa.DataPrevistaConclusao <= ((DateTime)periodoAte).Date)
                             && (status == StatusTarefaEnum.Todos || (status == StatusTarefaEnum.Ativos && tarefa.DataConclusao == null)
                                        || (status == StatusTarefaEnum.Concluidos && tarefa.DataConclusao != null)))
                        .ToList();
        }

        public Tarefa GetById(int idTarefa)
        {
            return _contexto.Tarefa.FirstOrDefault(tarefa => tarefa.Id == idTarefa);
        }

        public void RemoverTarefa(Tarefa tarefa)
        {
            _contexto.Tarefa.Remove(tarefa);
            _contexto.SaveChanges();
        }
    }
}
