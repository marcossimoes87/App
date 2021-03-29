using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO;
using SeguroCaixa.DTO.Request;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipanteController : BaseController
    {
        private readonly ILogger<ParticipanteController> _logger;
        ParticipanteService _participanteService;

        public ParticipanteController(ParticipanteService participanteService, ILogger<ParticipanteController> logger, IMemoryCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _participanteService = participanteService;
            _logger = logger;
        }

        [HttpGet]
        [Route("v1/estadocivil")]
        public async Task<ActionResult<List<EstadoCivilResponse>>> GetEstadoCivil()
        {
            try
            {
                _logger.LogInformation($"Início - GetEstadoCivil: {GetCache<List<EstadoCivilResponse>>(CacheTypes.EstadoCivil)}");

                List<EstadoCivilResponse> listaEstadoCivil = GetCache<List<EstadoCivilResponse>>(CacheTypes.EstadoCivil);

                if (listaEstadoCivil == null)
                {
                    listaEstadoCivil = await _participanteService.GetListaEstadoCivil();
                    SetCache(CacheTypes.EstadoCivil, listaEstadoCivil);
                }

                _logger.LogInformation($"Fim - GetEstadoCivil: {listaEstadoCivil}");

                return StatusCode(200, listaEstadoCivil);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  GetEstadoCivil: {GetCache<List<EstadoCivilResponse>>(CacheTypes.EstadoCivil)}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPost]
        [Route("v1/validapessoa")]
        public async Task<ActionResult<DadosPessoaisResponse>> ValidaDadosPessoais([FromBody] DadosPessoaisRequest dadosPessoaisRequest)
        {
            try
            {
                _logger.LogInformation($"Início - ValidaDadosPessoais: {dadosPessoaisRequest}");



                if (dadosPessoaisRequest.IcVitima.HasValue && dadosPessoaisRequest.IcVitima.Value)
                {
                    string mensagem = "";
                    if (dadosPessoaisRequest.Cpf == CPF)
                        mensagem = "A pessoa solicitante não pode ser a vítima caso o tipo da solicitação seja de indenização por morte";

                    //buscar pedido de indenização através do CPF da vitima
                    // não permitir pedido de indenização se o for por caso de morte  e o status da solicitação estiver diferente de Solicitação indeferida
                    if (mensagem != "")
                    {
                        var pedido = await _participanteService.RetornaPedidoNaoIndeferidoFluxoMortePorCpfVitima(dadosPessoaisRequest.Cpf);
                        if (pedido.NuPedidoIndenizacao > 0)
                            mensagem = "Não é possível mais de uma solicitação por vitima em processo no caso de indenização por morte.";
                    }

                    if (mensagem != "")
                        return StatusCode((int)HttpStatusCode.PreconditionFailed, mensagem);
                }


                if (dadosPessoaisRequest.Cpf != CPF && dadosPessoaisRequest.IcVitima == null 
                    && dadosPessoaisRequest.IcProcurador != true && dadosPessoaisRequest.IcRepresentanteLegal != true)
                {
                    return StatusCode(401, "Não autorizado. O usuário solicitante deve ser o mesmo usuário que acessou o aplicativo.");
                }



                DadosPessoaisResponse dadosPessoaisResponse = await _participanteService.ValidaDadosPessoais(dadosPessoaisRequest);

                _logger.LogInformation($"Fim - ValidaDadosPessoais: {dadosPessoaisResponse}");

                return StatusCode(dadosPessoaisResponse.status, dadosPessoaisResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ValidaDadosPessoais: {dadosPessoaisRequest}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/parentesco")]
        public async Task<ActionResult<List<ParentescoResponse>>> GetParentesco()
        {
            try
            {
                _logger.LogInformation($"Início - GetParentesco: {GetCache<List<ParentescoResponse>>(CacheTypes.Parentesco)}");

                List<ParentescoResponse> listaParentesco = GetCache<List<ParentescoResponse>>(CacheTypes.Parentesco);

                if (listaParentesco == null)
                {
                    listaParentesco = await _participanteService.GetParentesco();
                    SetCache(CacheTypes.Parentesco, listaParentesco);
                }

                _logger.LogInformation($"Fim - GetParentesco: {listaParentesco}");

                return StatusCode(200, listaParentesco);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  GetParentesco: {GetCache<List<ParentescoResponse>>(CacheTypes.Parentesco)}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/dadospessoais")]
        public async Task<ActionResult<List<DadosPessoaisResponse>>> GetDadosPessoais()
        {
            try
            {
                _logger.LogInformation($"Início - GetDadosPessoais: {CPF}");

                DadosPessoaisResponse dadosPessoais = await _participanteService.RetornaDadosPessoais(CPF);

                _logger.LogInformation($"Fim - GetDadosPessoais: {dadosPessoais}");

                return StatusCode(200, dadosPessoais);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  GetDadosPessoais: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }


        [HttpGet]
        [Route("v1/ocupacao/{tipoOcupacao}")]
        public async Task<ActionResult<List<OcupacaoResponse>>> GetOcupacao(string tipoOcupacao)
        {
            try
            {
                _logger.LogInformation($"Início - GetOcupacao: {tipoOcupacao}");

                string cacheKey = string.Format(CacheTypes.Ocupacao, tipoOcupacao);

                List<OcupacaoResponse> ocupacaoResponses = GetCache<List<OcupacaoResponse>>(cacheKey);
                if (ocupacaoResponses == null)
                {
                    ocupacaoResponses = await _participanteService.GetOcupacoes(tipoOcupacao);
                    SetCache(cacheKey, ocupacaoResponses);
                }

                _logger.LogInformation($"Fim - GetOcupacao: {ocupacaoResponses}");

                return StatusCode(200, ocupacaoResponses);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  GetOcupacao: {tipoOcupacao}");
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
