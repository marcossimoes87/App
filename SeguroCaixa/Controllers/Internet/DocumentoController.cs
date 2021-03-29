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
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : BaseController
    {
        private readonly DocumentoService _documentoService;
        private readonly ILogger<DocumentoController> _logger;

        public DocumentoController(ILogger<DocumentoController> logger, DocumentoService documentoService, IMemoryCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _logger = logger;
            _documentoService = documentoService;
        }     

        [HttpPost]
        [Route("v1/listadocumentos")]
        public async Task<ActionResult<List<ListaDocumentoResponse>>> ListaDocumentosExigidos([FromBody] ListaDocumentoRequest listaDocumentoRequest)
        {
            try
            {
                _logger.LogInformation($"Início - ListaDocumentosExigidos: {listaDocumentoRequest}");
                List<ListaDocumentoResponse> listaDocumentoResponse = new List<ListaDocumentoResponse>();

                string cacheKey = string.Format(CacheTypes.DocumentoExigido, listaDocumentoRequest.IdTipoPedido, listaDocumentoRequest.IdTipoParticipacao);

                List<TipoBeneficiarioResponse> tiposBeneficiario = GetCache<List<TipoBeneficiarioResponse>>(cacheKey);
                if (tiposBeneficiario == null)
                {
                    listaDocumentoResponse = await _documentoService.ListaDocumentoExigido(listaDocumentoRequest);
                    SetCache(cacheKey, listaDocumentoResponse);
                }

                _logger.LogInformation($"Fim - ListaDocumentosExigidos: {listaDocumentoResponse}");
                return StatusCode(200, listaDocumentoResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - ListaDocumentosExigidos {listaDocumentoRequest}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPost]
        [Route("v1/uploadDocumentos")]
        public async Task<ActionResult<MensagemUploadResponse>> UploadDocumentos([FromForm] ImagemDocumentoRequest imagemDocumentoRequest)
        {
            try
            {
                _logger.LogInformation($"Início - UploadDocumentos: {imagemDocumentoRequest}");
                                

                MensagemUploadResponse resposta = await _documentoService.SalvaImagemDocumentoRequest(imagemDocumentoRequest, CPF);

                _logger.LogInformation($"Fim - UploadDocumentos: {resposta}");

                return StatusCode(resposta.CodigoRetorno, resposta);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - AppSeguroCaixa - uploadDocumentos: {imagemDocumentoRequest}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/Remover/{nuDocumento}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<RemoverDocumentoResponse>> RemoverDocumento(long nuDocumento)
        {
            try
            {
                _logger.LogInformation($"Início - RemoverDocumento: {nuDocumento}");

                RemoverDocumentoResponse response = await _documentoService.RemoveDocumentoPorId(nuDocumento, CPF);

                _logger.LogInformation($"Fim - RemoverDocumento: {response}");

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - RemoverDocumento {nuDocumento}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpPost]
        [Route("v1/Remover/Lista")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<RemoverDocumentoResponse>> RemoverListaDocumento(List<DocumentoRequest> documentos)
        {
            try
            {
                _logger.LogInformation($"Início - RemoverListaDocumento: {documentos}");

                List<RemoverDocumentoResponse> listResponse = new List<RemoverDocumentoResponse>();
                foreach (DocumentoRequest documento in documentos)
                {
                    RemoverDocumentoResponse response = await _documentoService.RemoveDocumentoPorId(documento.NuDocumento, CPF);
                    listResponse.Add(response);
                }

                _logger.LogInformation($"Fim - RemoverListaDocumento: {listResponse}");

                return StatusCode(200, listResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - RemoverListaDocumento {documentos}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/ListaDocumentosEnviados/{nuProcesso}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<DocumentoEnviadoResponse>>> ListaDocumentosEnviados(long nuProcesso)
        {
            try
            {
                _logger.LogInformation($"Início - ListaDocumentosEnviados: {nuProcesso}");

                List<DocumentoEnviadoResponse> response = await _documentoService.ListaDeDocumentosEnviadosPorProcesso(nuProcesso, CPF);

                _logger.LogInformation($"Fim - ListaDocumentosEnviados: {response}");

                return StatusCode(200, response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - ListaDocumentosEnviados {nuProcesso}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/BuscarTodos/{nuProcesso}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetArquivoDocumentosPorProcesso(long nuProcesso)
        {
            try
            {
                _logger.LogInformation($"Início - GetArquivoDocumentosPorProcesso: {nuProcesso}");

                var doc = await _documentoService.BuscaArquivosDocumentosPorProcesso(nuProcesso, CPF);
                return File(doc, "application/zip");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - GetArquivoDocumentosPorProcesso {nuProcesso}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/Buscar/{nuDocumento}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetArquivoDocumentoPorIdDocumento(long nuDocumento)
        {
            try
            {
                _logger.LogInformation($"Início - GetArquivoDocumentoPorIdDocumento: {nuDocumento}");

                BuscarDocumentoResponse doc = await _documentoService.BuscaArquivoDocumentoPorIdECpf(nuDocumento, CPF);

                return File(doc.Content, doc.ContentType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - GetArquivoDocumentoPorIdDocumento {nuDocumento}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/Pendencias/{nuPedido}")]
        public async Task<ActionResult> GetPendencias(long nuPedido)
        {
            try
            {
                _logger.LogInformation($"Início - GetPendencias: {CPF} - {nuPedido}");

                PendenciaDocumentoResponse pendencia = await _documentoService.GetPendencia(CPF, nuPedido);

                _logger.LogInformation($"Fim - GetPendencias: {pendencia}");

                return StatusCode(200, pendencia);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - GetPendencias {nuPedido}");
                return StatusCode(500, "Erro de serviço");
            }
        }


        [HttpGet]
        [Route("v1/documentospendentes/{nuPedido}")]
        public async Task<ActionResult> DocumentosPendentesPorPedido(long nuPedido)
        {
            try
            {
                _logger.LogInformation("AppSeguroCaixa - DocumentosPendentesPorPedido - iniciado");

                List<Documento> listaDocumentosPendentes = await _documentoService.ListarDocumentosPendentesPorPedido(nuPedido, CPF);

                _logger.LogInformation("AppSeguroCaixa - DocumentosPendentesPorPedido - finalizado", nuPedido);

                return StatusCode(200, listaDocumentosPendentes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AppSeguroCaixa - DocumentosPendentesPorPedido: {0}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/buscamodeloprocuracao")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetArquivoModeloProcuracao()
        {
            try
            {
                _logger.LogInformation($"Início - GetArquivoModeloProcuracao");

                BuscarDocumentoResponse doc = await _documentoService.BuscaArquivoModeloProcuracao();

                return File(doc.Content, doc.ContentType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro - GetArquivoDocumentoPorIdDocumento");
                return StatusCode(500, "Erro ao baixar o modelo de procuração.");
            }
        }

    }
}
