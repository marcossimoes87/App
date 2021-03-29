using BackOfficeSeguroCaixa.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricoController : BaseController
    {
        private readonly HistoricoService _historicoService;

        private readonly ILogger<HistoricoController> _logger;



        public HistoricoController(ILogger<HistoricoController> logger, HistoricoService historicoService, IMemoryCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _logger = logger;
            _historicoService = historicoService;
        }

        /**
         * Retorna ultimo historico da pessoa para preencher o bloco de historico da home logada
         */
        [HttpGet]
        [Route("v1")]
        public async Task<ActionResult<HistoricoResponse>> UltimoHistorico()
        {
            try
            {
                _logger.LogInformation($"Início - UltimoHistorico: {CPF}");

                HistoricoResponse historico = await _historicoService.UltimoHistorico(CPF);

                _logger.LogInformation($"Fim - UltimoHistorico: {historico}");

                return StatusCode(200, historico);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  UltimoHistorico: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        /*
        Retorna a lista contento os sinistros de uma pessoa
     */
        [HttpGet]
        [Route("v1/lista")]
        public async Task<ActionResult<List<HistoricoResponse>>> ListaHistorico()
        {
            try
            {
                _logger.LogInformation($"Início - ListaHistorico: {CPF}");

                List<HistoricoResponse> historicos = await _historicoService.ListaHistoricos(CPF);

                if (historicos == null)
                {
                    return new NoContentResult();
                }

                _logger.LogInformation($"Fim - ListaHistorico: {historicos}");

                return StatusCode(200, historicos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ListaHistorico: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }



        /*
            Consulta da timeline do pedido
            @IMPOPRTANT
         */
        [HttpGet]
        [Route("v1/consultaTimeLineProcesso/{nuPedido}")]
        public async Task<ActionResult<List<HistoricoResponse>>> ListaHistoricosPedido(long nuPedido)
        {
            try
            {
                _logger.LogInformation($"Início - ListaHistoricosPedido: {CPF} - {nuPedido}");

                List<HistoricoResponse> historicos = await _historicoService.ListaHistoricosPedido(CPF, nuPedido);

                if (historicos == null)
                {
                    return new NoContentResult();
                }

                _logger.LogInformation($"Fim - ListaHistoricosPedido: {CPF} - {historicos}");

                return StatusCode(200, historicos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ListaHistoricosPedido: {CPF} - {nuPedido}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        /*
            Consulta pedido com historico
            @IMPOPRTANT
         */
        [HttpGet]
        [Route("v1/consultaProcessoHistorico")]
        public async Task<ActionResult<List<PedidoComHistoricoResponse>>> ListaPedidoComHistorico()
        {
            try
            {
                List<PedidoComHistoricoResponse> historicos = await _historicoService.ListaPedidoComHistorico(CPF);

                if (historicos == null)
                {
                    return new NoContentResult();
                }

                return StatusCode(200, historicos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CPF: {0} - listarPedidoComHistoricosDoUsuario", 0);
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
