using GerenciadorDeTarefas.Dtos;
using GerenciadorDeTarefas.Enums;
using GerenciadorDeTarefas.Models;
using GerenciadorDeTarefas.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefaController : BaseController
    {
        private readonly ILogger<TarefaController> _logger;
        private readonly ITarefaRepository _tarefaRepository;

        public TarefaController(ILogger<TarefaController> logger, 
                IUsuarioRepository usuarioRepository,
                ITarefaRepository tarefaRepository) : base(usuarioRepository)
        {
            _logger = logger;
            _tarefaRepository = tarefaRepository;
        }

        [HttpPost]
        public IActionResult AdicionarTarefa([FromBody] Tarefa tarefa)
        {
            try
            {
                var usuario = ReadToken();
                var erros = new List<string>();
                if (tarefa == null || usuario == null)
                {
                    erros.Add("Favor informar a tarefa ou usuário");
                }
                else
                {
                    if(string.IsNullOrEmpty(tarefa.Nome) || string.IsNullOrWhiteSpace(tarefa.Nome)
                            || tarefa.Nome.Count() < 4)
                    {
                        erros.Add("Favor informar um nome válido");
                    }

                    if(tarefa.DataPrevistaConclusao == DateTime.MinValue || tarefa.DataPrevistaConclusao.Date < DateTime.Now.Date)
                    {
                        erros.Add("Data de previsão não pode ser menor que hoje");
                    }
                }

                if(erros.Count > 0)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erros = erros
                    });
                }

                tarefa.IdUsuario = usuario.Id;
                tarefa.DataConclusao = null;
                _tarefaRepository.AdicionarTarefa(tarefa);

                return Ok(new { msg = "Tarefa criada com sucesso" });
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu erro ao adicionar tarefa", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErroRespotaDto()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Ocorreu erro ao adicionar tarefa, tente novamente!"
                });
            }
        }

        [HttpDelete("{idTarefa}")]
        public IActionResult DeletarTarefa(int idTarefa)
        {
            try 
            {
                var usuario = ReadToken();
                if(usuario == null || idTarefa <= 0)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erro = "usuário ou tarefa inválidos"
                    });
                }

                var tarefa = _tarefaRepository.GetById(idTarefa);

                if(tarefa == null || tarefa.IdUsuario != usuario.Id)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erro = "tarefa não encontrada"
                    });
                }

                _tarefaRepository.RemoverTarefa(tarefa);
                return Ok(new {msg = "Tarefa deletada com sucesso"});
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu erro ao deletar tarefa", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErroRespotaDto()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Ocorreu erro ao deletar tarefa, tente novamente!"
                });
            }
        }

        [HttpPut("{idTarefa}")]
        public IActionResult AtualizarTarefa([FromBody] Tarefa model, int idTarefa)
        {
            try 
            {
                var usuario = ReadToken();
                if (usuario == null || idTarefa <= 0)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erro = "usuário ou tarefa inválidos"
                    });
                }

                var tarefa = _tarefaRepository.GetById(idTarefa);

                if (tarefa == null || tarefa.IdUsuario != usuario.Id)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erro = "tarefa não encontrada"
                    });
                }

                var erros = new List<string>();
                if (model == null)
                {
                    erros.Add("Favor informar a tarefa ou usuário");
                }
                else if (!string.IsNullOrEmpty(model.Nome) && !string.IsNullOrWhiteSpace(model.Nome) && model.Nome.Count() < 4)
                {
                    erros.Add("Favor informar um nome válido");
                }
                
                if (erros.Count > 0)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erros = erros
                    });
                }

                if(!string.IsNullOrEmpty(model.Nome) && !string.IsNullOrWhiteSpace(model.Nome))
                {
                    tarefa.Nome = model.Nome;
                }

                if(model.DataPrevistaConclusao != DateTime.MinValue)
                {
                    tarefa.DataPrevistaConclusao = model.DataPrevistaConclusao;
                }

                if(model.DataConclusao != null && model.DataConclusao != DateTime.MinValue)
                {
                    tarefa.DataConclusao = model.DataConclusao;
                }

                _tarefaRepository.AtualizarTarefa(tarefa);
                return Ok(new { msg = "Tarefa atualizada com sucesso" });
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu erro ao atualizar tarefa", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErroRespotaDto()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Ocorreu erro ao atualizar tarefa, tente novamente!"
                });
            }
        }

        [HttpGet]
        public IActionResult ListarTarefasUsuario(DateTime? periodoDe, DateTime? periodoAte, StatusTarefaEnum status)
        {
            try 
            {
                var usuario = ReadToken();
                if(usuario == null)
                {
                    return BadRequest(new ErroRespotaDto
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erro = "Usuário não informado"
                    });
                }

                var resultado = _tarefaRepository.BuscarTarefas(usuario.Id, periodoDe, periodoAte, status);
                return Ok(resultado);
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu erro ao listar tarefas", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErroRespotaDto()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Ocorreu erro ao listar tarefas, tente novamente!"
                });
            }
        }
    }
}
