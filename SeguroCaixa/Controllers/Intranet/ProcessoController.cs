using BackOfficeSeguroCaixa.Filtros;
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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Client_id_intranet")]
    [Authorize(Policy = "requer_role_usuario")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessoController : BaseController
    {
        private readonly ProcessoService _processoService;

        private readonly ILogger<ProcessoController> _logger;

        private readonly DocumentoService _documentoService;

        public ProcessoController(ILogger<ProcessoController> logger,
            ProcessoService processo, IMemoryCache cache,
            IConfiguration configuration,
            DocumentoService documentoService) : base(cache, configuration)
        {
            _logger = logger;
            _processoService = processo;
            _documentoService = documentoService;
        }

        [HttpGet]
        [Route("v1/ListarProcessos")]
        public async Task<ActionResult<ProcessoPaginado>> Get(
        [FromQuery] ProcessoFiltro filtro = null,
        [FromQuery] ProcessoPaginacao paginacao = null)
        {
            try
            {
                _logger.LogInformation("AppBackOfficeSeguroCaixa - Processo [Get All] - iniciado");

                var lista = await _processoService.ListarProcessos(filtro, paginacao, MatriculaFuncCaixa);
                 if (lista.Resultado.Count == 0)
                     return NotFound("Não foram encontrados registros.");

                _logger.LogInformation("AppBackOfficeSeguroCaixa - Processo [Get All] - Finalizado", lista.ToJson());

                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - Processo [Get All]: {0}", e.ToJson());

                return StatusCode(500, $"Erro de serviço");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessoResponse>> Get(long id)
        {
            try
            {
                _logger.LogInformation("AppBackOfficeSeguroCaixa - Processo [Get:id] - iniciado");

                var processo = await _processoService.RetornaProcessoPorId(id);
                if (processo == null)
                    return NotFound("Registro não encontrado.");
               
                _logger.LogInformation("AppBackOfficeSeguroCaixa - Processo [Get:id] - finalizado: ", processo.ToJson());
                
                return Ok(processo);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - Processo [Get:id]: {0}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPut]
        [Route("v1/iniciaAtendimentoProcesso")]
        public async Task<ActionResult<MensagemProcessoResponse>> IniciaAtendimentoProcesso(short NuProcesso, decimal nuCpf, string CoMatriculaCaixa)
        {
            try
            {
                _logger.LogInformation( $"AppBackOfficeSeguroCaixa - IniciaAtendimentoProcesso {NuProcesso} - Iniciado");
               
                if (!CoMatriculaCaixa.Trim().ToLower().Equals(MatriculaFuncCaixa.Trim().ToLower()))
                {
                    _logger.LogInformation($"AppBackOfficeSeguroCaixa - IniciaAtendimentoProcesso {NuProcesso} - Usuário {CoMatriculaCaixa} não autorizado para visualizar o processo.");
                    return BadRequest(new ArgumentException("Usuário não autorizado para visualizar o processo."));
                }
                    

                MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
                mensagemProcessoResponse = await _processoService.AtualizaStatusProcesso(NuProcesso, nuCpf, CoMatriculaCaixa);
                
                _logger.LogInformation($"AppBackOfficeSeguroCaixa - IniciaAtendimentoProcesso {NuProcesso} - Finalizado com Sucesso.");
                return StatusCode(mensagemProcessoResponse.CodigoRetorno, mensagemProcessoResponse.MensagemRetorno);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AppBackOfficeSeguroCaixa - IniciaAtendimentoProcesso {NuProcesso} - {e.Message}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/carregaInformacaoProcesso")]
        public async Task<ActionResult<DadosProcessoResponse>> CarregaInformacaoProcesso(short NuProcesso, decimal nuCpf, string CoMatriculaCaixa)
        {
            try
            {
                _logger.LogInformation($"AppBackOfficeSeguroCaixa - CarregaInformacaoProcesso {NuProcesso} - Iniciado");
               
                if (!CoMatriculaCaixa.Trim().ToLower().Equals(MatriculaFuncCaixa.Trim().ToLower()))
                {
                    _logger.LogInformation($"AppBackOfficeSeguroCaixa - CarregaInformacaoProcesso {NuProcesso} - Usuário {CoMatriculaCaixa} não autorizado para visualizar o processo.");
                    return BadRequest(new ArgumentException("Usuário não autorizado para visualizar o processo."));
                }
                
                CoMatriculaCaixa = MatriculaFuncCaixa;//resgatando matricula do Token (HttpContext)
                DadosProcessoResponse dadosProcessoResponse = new DadosProcessoResponse();
                dadosProcessoResponse = await _processoService.CarregaInformacaoProcesso(NuProcesso, nuCpf, CoMatriculaCaixa);

                _logger.LogInformation($"AppBackOfficeSeguroCaixa - CarregaInformacaoProcesso: {NuProcesso} finalizado com sucesso.");
                return Ok(dadosProcessoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AppBackOfficeSeguroCaixa - CarregaInformacaoProcesso {NuProcesso} - {e.Message}");
                return StatusCode(500, e.Message);
            }

        }

        /*
            Retorna dados base historica líder.
        */
        [HttpGet]
        [Route("v1/consultaBaseHistorica")]
        public async Task<ActionResult> ListaHistoricoPorCpf(long? NuCpf, [FromQuery] ProcessoPaginacaoHistorico paginacao)
        {
            try
            {
                _logger.LogInformation($"Inicio - ListaHistorico: {NuCpf}");
                ProcessoPaginadoHistorico historicos = await _processoService.ListaHistoricoPorCpf(NuCpf, paginacao);

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

        [HttpPost]
        [Route("v1/salvaranalise")]
        public async Task<ActionResult<DadosProcessoResponse>> SalvarAnalise([FromBody] SituacaoMomentoPedidoRequest situacaoMomentoPedido)
        {

            try
            {
                _logger.LogInformation("AppBackOfficeSeguroCaixa - SalvarAnalise - iniciado");
                if (ModelState.IsValid)
                {
                    DadosProcessoResponse dadosProcessoResponse = new DadosProcessoResponse();
                    dadosProcessoResponse = await _processoService.SalvarAnalise(situacaoMomentoPedido);
                    _logger.LogInformation("AppBackOfficeSeguroCaixa - SalvarAnalise - finalizado");
                    return StatusCode(200, dadosProcessoResponse);
                }
                return BadRequest(ResponseError.FromModelStateError(ModelState));

            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - ConcluirAnalise: {0}");
                return StatusCode(500, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - SalvarAnalise: {0}");
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost]
        [Route("v1/concluiranalise")]
        public async Task<ActionResult<ProcessoPaginadoHistorico>> ConcluirAnalise([FromBody] SituacaoPedidoRequest situacaoPedidoRequest)
        {
            try
            {
                _logger.LogInformation("AppBackOfficeSeguroCaixa - ConcluirAnalise - iniciado");
                if (ModelState.IsValid)
                {
                    await _processoService.ConcluirAnalise(situacaoPedidoRequest);
                    _logger.LogInformation("AppBackOfficeSeguroCaixa - ConcluirAnalise - finalizado");
                    return Ok("Analise concluída.");
                }
                return BadRequest(ResponseError.FromModelStateError(ModelState));

            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - ConcluirAnalise: {0}");
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AppBackOfficeSeguroCaixa - ConcluirAnalise: {0}");
                return StatusCode(500, "Erro de serviço");
            }

        }

        [HttpGet]
        [Route("v1/BuscarDocumento/{nuDocumento}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetArquivoDocumentoPorIdDocumento(long nuDocumento)
        {
            try
            {
                _logger.LogInformation($"Início - GetArquivoDocumentoPorIdDocumento: {nuDocumento}");

                BuscarDocumentoResponse doc = await _documentoService.BuscaArquivoDocumentoPorIdECpf(nuDocumento, null);

                _logger.LogInformation($"Fim - GetArquivoDocumentoPorIdDocumento: {nuDocumento} finalizado com sucesso.");
                return File(doc.Content, doc.ContentType);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - GetArquivoDocumentoPorIdDocumento {nuDocumento}");
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
