using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacaoController : BaseController
    {
        private readonly NotificacaoService _notificacaoService;       

        private readonly ILogger<NotificacaoController> _logger;

        public NotificacaoController(ILogger<NotificacaoController> logger, NotificacaoService notificacaoService, IMemoryCache cache, IConfiguration configuration) : base (cache, configuration)
        {
            _logger = logger;         
            _notificacaoService = notificacaoService;
            
        }

        [HttpPut]
        [Route("v1")]
        public async Task<ActionResult<NotificacaoResponse>> VisualizaNotificacao()
        {
            try
            {
                _logger.LogInformation($"Início - VisualizaNotificacao: {CPF}");

                ResponseMessage response = await _notificacaoService.VisualizaNotificacoes(CPF);

                _logger.LogInformation($"Fim - VisualizaNotificacao: {response}");

                return StatusCode(200, response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  VisualizaNotificacao: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/notificacoes")]
        public async Task<ActionResult<NotificacaoResponse>> ListaNotificacoes()
        {
            try
            {
                _logger.LogInformation($"Início - ListaNotificacoes: {CPF}");

                List<NotificacaoResponse> notificacaoResponses = await _notificacaoService.ListaNotificacao(CPF);

                _logger.LogInformation($"Fim - ListaNotificacoes: {notificacaoResponses}");

                return StatusCode(200, notificacaoResponses);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ListaNotificacoes: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
