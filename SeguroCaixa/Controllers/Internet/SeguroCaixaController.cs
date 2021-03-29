using BackOfficeSeguroCaixa.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Request;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class SeguroCaixaController : BaseController
    {
        private readonly SeguroCaixaService _seguroCaixaService;

        private readonly ILogger<SeguroCaixaController> _logger;


        public SeguroCaixaController(ILogger<SeguroCaixaController> logger, SeguroCaixaService seguroCaixa, IMemoryCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _logger = logger;
            _seguroCaixaService = seguroCaixa;
        }

        [HttpGet]
        [Route("v1/tipoBeneficiario/{IdTipoIndenizacao}")]
        public async Task<ActionResult<List<TipoBeneficiarioResponse>>> TiposBeneficiario(short? IdTipoIndenizacao)
        {
            try
            {
                _logger.LogInformation($"Início - TiposBeneficiario: {IdTipoIndenizacao}");

                string cacheKey = string.Format(CacheTypes.Beneficiario, IdTipoIndenizacao);

                List<TipoBeneficiarioResponse> tiposBeneficiario = GetCache<List<TipoBeneficiarioResponse>>(cacheKey);
                if (tiposBeneficiario == null)
                {
                    tiposBeneficiario = await _seguroCaixaService.TiposBeneficiario(IdTipoIndenizacao);
                    SetCache(cacheKey, tiposBeneficiario);
                }

                _logger.LogInformation($"Fim - TiposBeneficiario: {tiposBeneficiario}");

                return StatusCode(200, tiposBeneficiario);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AppSeguroCaixa - tipoBeneficiario: {0}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/tipoVeiculoAcidente")]
        public async Task<ActionResult<List<TipoVeiculoAcidenteResponse>>> TipoVeiculoAcidente()
        {
            try
            {
                _logger.LogInformation($"Início - TipoVeiculoAcidente");

                List<TipoVeiculoAcidenteResponse> tipoVeiculoAcidenteResponse = GetCache<List<TipoVeiculoAcidenteResponse>>(CacheTypes.Veiculo);

                if (tipoVeiculoAcidenteResponse == null)
                {
                    tipoVeiculoAcidenteResponse = await _seguroCaixaService.TipoVeiculoAcidente();
                    SetCache(CacheTypes.Veiculo, tipoVeiculoAcidenteResponse);
                }

                _logger.LogInformation($"Fim - TipoVeiculoAcidente: {tipoVeiculoAcidenteResponse}");

                return StatusCode(200, tipoVeiculoAcidenteResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AppSeguroCaixa - tipoVeiculoAcidente: {0}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/tipoPedidoIndenizacao")]
        public async Task<ActionResult<List<TipoPedidoResponse>>> TiposPedidoIndenizacao()
        {
            try
            {
                _logger.LogInformation($"Início - TiposPedidoIndenizacao");

                List<TipoPedidoResponse> listaTipoPedido = GetCache<List<TipoPedidoResponse>>(CacheTypes.TipoIndenizacao);

                if (listaTipoPedido == null)
                {
                    listaTipoPedido = await _seguroCaixaService.TipoPedidoIndenizacao();
                    SetCache(CacheTypes.TipoIndenizacao, listaTipoPedido);
                }

                _logger.LogInformation($"Fim - TiposPedidoIndenizacao: {listaTipoPedido}");

                return StatusCode(200, listaTipoPedido);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - TiposPedidoIndenizacao {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPut]
        [Route("v1/envioprocesso")]
        public async Task<ActionResult<MensagemProcessoResponse>> ProcessaSolicitacaoPedido([FromBody] DadosRequisicaoSeguroRequest dadosRequisicaoSeguro)
        {
            try
            {
                _logger.LogInformation($"Início - ProcessaSolicitacaoPedido: {dadosRequisicaoSeguro}");
                dadosRequisicaoSeguro.Canal = 1;

                MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();

                if (dadosRequisicaoSeguro.DadosPessoa.CpfVitima == CPF && dadosRequisicaoSeguro.IdTipoPedido == 4)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, "A pessoa solicitante não pode ser a vítima caso o tipo da solicitação seja de indenização por morte");
                }

                if (!Regex.IsMatch(Convert.ToString(dadosRequisicaoSeguro.DadosPessoa.Nif == null ? 0 : dadosRequisicaoSeguro.DadosPessoa.Nif), ("^[0-9]+$")))
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, "Nif deve conter apenas números.");
                }

                if (dadosRequisicaoSeguro.DadosAcidente.DataHoraAcidente > DateTime.Now)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, "Hora do acidente não pode ser maior que a hora atual.");
                }

                if (dadosRequisicaoSeguro.NuPedido != null)
                {
                    mensagemProcessoResponse = await _seguroCaixaService.AtualizaProcessaRequisicaoSeguro(dadosRequisicaoSeguro, CPF);
                }
                else
                {
                    mensagemProcessoResponse = await _seguroCaixaService.ProcessaRequisicaoSeguro(dadosRequisicaoSeguro, CPF);
                }

                _logger.LogInformation($"Fim - ProcessaSolicitacaoPedido: {mensagemProcessoResponse}");

                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - ProcessaSolicitacaoPedido {dadosRequisicaoSeguro} - {e.Message}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/tipoveiculopatrimonio")]
        public ActionResult<List<TipoVeiculoAcidenteResponse>> TipoVeiculoPatrimonio()
        {
            try
            {
                _logger.LogInformation($"Início - TipoVeiculoPatrimonio");

                List<TipoVeiculoAcidenteResponse> tipoVeiculoAcidenteResponse = new List<TipoVeiculoAcidenteResponse>()
                {
                     new TipoVeiculoAcidenteResponse()
                     {
                         Codigo = 1, Descricao = "Carro"
                     },
                     new TipoVeiculoAcidenteResponse()
                     {
                         Codigo = 3, Descricao = "Moto"
                     },
                     new TipoVeiculoAcidenteResponse()
                     {
                         Codigo = 6, Descricao = "Ônibus"
                     },
                     new TipoVeiculoAcidenteResponse()
                     {
                         Codigo = 8, Descricao = "Caminhão"
                     }
                };

                _logger.LogInformation($"Fim - TipoVeiculoPatrimonio: {tipoVeiculoAcidenteResponse}");

                return StatusCode(200, tipoVeiculoAcidenteResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - TipoVeiculoPatrimonio: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPost]
        [Route("v1/confirmarProcesso")]
        public async Task<ActionResult<MensagemProcessoResponse>> ConfirmarProcesso([FromBody] ConfirmarProcessoRequest confirmarProcesso)
        {
            try
            {
                _logger.LogInformation($"Início - ConfirmarProcesso: {confirmarProcesso}");

                MensagemProcessoResponse mensagemProcessoResponse = await _seguroCaixaService.AtualizaProcesso(confirmarProcesso, CPF);

                _logger.LogInformation($"Fim - ConfirmarProcesso: {mensagemProcessoResponse}");

                

                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  AtualizarProcesso: {confirmarProcesso}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/solicitacaoIncompleta")]
        public async Task<ActionResult<MensagemProcessoResponse>> VerificarSolicitacaoIncompleta()
        {
            try
            {
                _logger.LogInformation($"Início - VerificarSolicitacaoIncompleta: {CPF}");

                MensagemProcessoResponse mensagemProcessoResponse = await _seguroCaixaService.VerificaSolicitacaoIncompleta(CPF);

                _logger.LogInformation($"Fim - VerificarSolicitacaoIncompleta: {mensagemProcessoResponse}");

                if (mensagemProcessoResponse.CodigoRetorno == 204)
                {
                    return StatusCode(204);
                }

                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - VerificarSolicitacaoIncompleta: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/pendencias")]
        public async Task<ActionResult<MensagemProcessoResponse>> VerificarPendenciaDocumentacao()
        {
            try
            {
                _logger.LogInformation($"Início - VerificarPendenciaDocumentacao: {CPF}");

                MensagemProcessoResponse mensagemProcessoResponse = await _seguroCaixaService.VerificaPendenciaDocumentacao(CPF);

                _logger.LogInformation($"Fim - VerificarPendenciaDocumentacao: {mensagemProcessoResponse}");

                if (mensagemProcessoResponse.CodigoRetorno == 204)
                {
                    return StatusCode(204);
                }

                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  VerificaPendenciaDocumentacao: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/solicitacoes")]
        public async Task<ActionResult<MensagemProcessoResponse>> VerificarSolicitacoesEmAndamento()
        {
            try
            {
                _logger.LogInformation($"Início - VerificarSolicitacoesEmAndamento: {CPF}");

                MensagemProcessoResponse mensagemProcessoResponse = await _seguroCaixaService.VerificaSolicitacoesEmAndamento(CPF);

                _logger.LogInformation($"Fim - VerificarSolicitacoesEmAndamento: {mensagemProcessoResponse}");

                if (mensagemProcessoResponse.CodigoRetorno == 204)
                {
                    return StatusCode(204);
                }

                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  VerificarSolicitacoes: {CPF}");
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
