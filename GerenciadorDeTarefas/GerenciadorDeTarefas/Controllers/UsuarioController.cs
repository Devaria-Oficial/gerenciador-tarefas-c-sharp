﻿using GerenciadorDeTarefas.Dtos;
using GerenciadorDeTarefas.Models;
using GerenciadorDeTarefas.Repository;
using GerenciadorDeTarefas.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SalvarUsuario([FromBody]Usuario usuario)
        {
            try
            {
                var erros = new List<string>();
                if(string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrWhiteSpace(usuario.Nome)
                    || usuario.Nome.Length < 2)
                {
                    erros.Add("Nome inválido");
                }

                if (string.IsNullOrEmpty(usuario.Senha) || string.IsNullOrWhiteSpace(usuario.Senha)
                    || usuario.Senha.Length < 4 && Regex.IsMatch(usuario.Senha, "[a-zA-Z0-9]+", RegexOptions.IgnoreCase))
                {
                    erros.Add("Senha inválida");
                }

                Regex regex = new Regex(@"^([\w\.\-\+\d]+)@([\w\-]+)((\.(\w){2,4})+)$");
                if(string.IsNullOrEmpty(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Email)
                    || !regex.Match(usuario.Email).Success)
                {
                    erros.Add("Email inválido");
                }

                if (_usuarioRepository.ExisteUsuarioPeloEmail(usuario.Email))
                {
                    erros.Add("Já existe uma conta com o email informado");
                }

                if(erros.Count > 0)
                {
                    return BadRequest(new ErroRespotaDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Erros = erros
                    });
                }

                usuario.Email = usuario.Email.ToLower();
                usuario.Senha = MD5Utils.GerarHashMD5(usuario.Senha);
                _usuarioRepository.Salvar(usuario);

                return Ok(new{ msg = "Usuário Criado com sucesso" });
            }
            catch(Exception e)
            {
                _logger.LogError("Ocorreu erro ao salvar usuário", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErroRespotaDto()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Ocorreu erro ao salvar usuário, tente novamente!"
                });
            }
        }
    }
}
