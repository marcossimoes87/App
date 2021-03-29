using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Request;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Services
{
    public class DocumentoService
    {
        #region Inicializadores e Construtor
        private readonly DbEscrita _context;
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<DocumentoService> _logger;
        private IConfiguration _configuration;

        public DocumentoService(DbEscrita context, ILogger<DocumentoService> logger, DbLeitura contextLeitura, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
        }
        #endregion

        public async Task<List<ListaDocumentoResponse>> ListaDocumentoExigido(ListaDocumentoRequest listaDocumentoRequest)
        {
            _logger.LogInformation($"Start ListaDocumentoExigido - listaDocumentoRequest: {listaDocumentoRequest.ToJson()}");

            try
            {
                List<ListaDocumentoResponse> listaDocumentoResponse = new List<ListaDocumentoResponse>();

                List<Sdctb005GrupoDocumento> sdctb005GrupoDocumentos = await _contextLeitura
                    .Sdctb005GrupoDocumento
                    .Where(c => c.NuTipoIndenizacao == listaDocumentoRequest.IdTipoPedido && (c.NuTipoParticipacao == listaDocumentoRequest.IdTipoParticipacao || c.NuTipoParticipacao == null))
                    .OrderBy(o => o.NuGrupoDocumento)
                    .ToListAsync();

                //  _logger.LogDebug($"ListaDocumentoExigido - {sdctb005GrupoDocumentos.ToJson()}");

                foreach (Sdctb005GrupoDocumento item in sdctb005GrupoDocumentos)
                {
                    List<Sdctb003DocumentoExigido> sdctb003DocumentoExigido = await _contextLeitura
                        .Sdctb003DocumentoExigido
                        .Include(c => c.NuTipoDocumentoNavigation)
                        .Where(x => x.NuGrupoDocumento == item.NuGrupoDocumento && x.NuTipoDocumentoNavigation.DhExclusao == null)
                        .OrderBy(o => o.NuDocumentoExigido)
                        .ToListAsync();

                    List<Documento> documentos = new List<Documento>();

                    foreach (Sdctb003DocumentoExigido doc in sdctb003DocumentoExigido)
                    {
                        documentos.Add(new Documento()
                        {
                            AbreviaturaTipoDocumento = doc.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                            IdDocumentoExigido = doc.NuDocumentoExigido,
                            IdGrupoDocumento = doc.NuGrupoDocumento,
                            IdTipoDocumento = doc.NuTipoDocumento,
                            NomeTipoDocumento = doc.NuTipoDocumentoNavigation.DeTipoDocumento,
                            quantidadePaginas = doc.NuTipoDocumentoNavigation.QtPaginas
                        });
                    }

                    ListaDocumentoResponse grupoDocumento = new ListaDocumentoResponse()
                    {
                        AbreviaturaGrupoDocumento = item.DeAbreviaturaGrupoDocumento,
                        DocumentoUnico = item.IcDocumentoUnico,
                        Obrigatorio = item.IcObrigatorio,
                        IdGrupoDocumento = item.NuGrupoDocumento,
                        NomeGrupoDocumento = item.DeGrupoDocumento,
                        documentos = documentos,
                        NuOrdem = item.NuOrdem
                    };

                    listaDocumentoResponse.Add(grupoDocumento);
                }

                _logger.LogInformation($"ListaDocumentoExigido - retorno {listaDocumentoResponse.ToJson()}");

                return listaDocumentoResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"ListaDocumentoExigido - erro {e.Message}");
                throw e;
            }
        }

        public async Task<MensagemUploadResponse> SalvaImagemDocumentoRequest(ImagemDocumentoRequest imagemDocumentoRequest, decimal nuCpf)
        {
            _logger.LogInformation($"Start SalvaImagemDocumentoRequest - imagemDocumentoRequest: {imagemDocumentoRequest.ToJson()}, CPF {nuCpf}");

            List<RespostaDocumentoUpload> docs = new List<RespostaDocumentoUpload>();
            bool sucesso = false;

            try
            {
                var getCpfs = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                     join p in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals p.NuPedidoIndenizacao
                                     join p2 in _contextLeitura.Sdctb009Pessoa on p.NuPessoaParticipante equals p2.NuPessoa
                                     where pi.NuPedidoIndenizacao == imagemDocumentoRequest.NumeroProcesso
                                     select p2)
                                   .ToListAsync();

                bool checarCpf = getCpfs.Any(x => x.NuCpf == nuCpf);


                if (imagemDocumentoRequest.Documento.Any(x => x.Length / (1024 * 1024) > 10))
                {
                    _logger.LogInformation($"SalvaImagemDocumentoRequest:  Codigo Retorno {StatusCodes.Status413PayloadTooLarge}");

                    return new MensagemUploadResponse
                    {
                        CodigoRetorno = StatusCodes.Status413PayloadTooLarge,
                        Documentos = docs
                    };
                }

                _logger.LogDebug($"SalvaImagemDocumentoRequest - checarCpf {checarCpf.ToJson()}");

                if (checarCpf)
                {
                    string connectionString = _configuration["storageConfig:connection"];
                    string containerName = _configuration["storageConfig:containerName"];

                    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

                    if (!await container.ExistsAsync())
                    {
                        await container.CreateIfNotExistsAsync();
                    }

                    foreach (var img in imagemDocumentoRequest.Documento)
                    {

                        var tipodocumento = await _contextLeitura.Sdctb006TipoDocumento.Where(x => x.NuTipoDocumento == short.Parse(imagemDocumentoRequest.TipoDocumento)).FirstOrDefaultAsync();
                        var count = await _contextLeitura.Sdctb002DocumentoCapturado
                            .Include(x => x.NuDocumentoExigidoNavigation)
                            .Where(x => x.NuPedidoIndenizacao == imagemDocumentoRequest.NumeroProcesso && x.NuDocumentoExigidoNavigation.NuTipoDocumento == tipodocumento.NuTipoDocumento).CountAsync();

                        var extensao = string.IsNullOrEmpty(System.IO.Path.GetExtension(img.FileName)) ? $".{imagemDocumentoRequest.ExtensaoDocumento}" : System.IO.Path.GetExtension(img.FileName);

                        BlobClient blob = container.GetBlobClient($"{nuCpf}/{imagemDocumentoRequest.NumeroProcesso}/{tipodocumento.DeTipoDocumento.RemoveAcento().Trim()}_{imagemDocumentoRequest.Pagina}{extensao}");

                        if ((count > 0) && (imagemDocumentoRequest.Pagina == 0))
                        {
                            blob = container.GetBlobClient($"{nuCpf}/{imagemDocumentoRequest.NumeroProcesso}/{tipodocumento.DeTipoDocumento.RemoveAcento().Trim()}_{count}_{imagemDocumentoRequest.Pagina}{extensao}");
                        }

                        BlobUploadOptions options = new BlobUploadOptions();
                        options.HttpHeaders = new BlobHttpHeaders() { ContentType = img.ContentType };

                        var response = await blob.UploadAsync(img.OpenReadStream(), options);

                        if (response.GetRawResponse().Status.IsSuccessStatusCode())
                        {
                            try
                            {
                                Sdctb002DocumentoCapturado sdctb002DocumentoCapturado = new Sdctb002DocumentoCapturado();
                                sdctb002DocumentoCapturado.NuPedidoIndenizacao = imagemDocumentoRequest.NumeroProcesso;
                                sdctb002DocumentoCapturado.NuDocumentoExigido = imagemDocumentoRequest.IdDocumentoExigido;
                                sdctb002DocumentoCapturado.DeUrlImagem = blob.Uri.AbsoluteUri;
                                sdctb002DocumentoCapturado.DhInclusao = DateTime.Now.AddHours(-3);
                                sdctb002DocumentoCapturado.DeCaminhoBlob = blob.Uri.LocalPath.Replace($"/{containerName}/", string.Empty);
                                sdctb002DocumentoCapturado.DeNomeArquivo = sdctb002DocumentoCapturado.DeCaminhoBlob.Split('/').LastOrDefault();

                                _context.Add(sdctb002DocumentoCapturado);

                                var retorno = await _context.SaveChangesAsync();

                                // Verifica se o upload é de um arquivo pendente e atualiza a tabela de documentos pendentes
                                var documentoPendente = await _context.Sdctb030DocumentoPendente
                                    .Include(c => c.NuDocumentoCapturadoNavigation)
                                        .Where(d => d.NuSituacaoPedidoNavigation.NuPedidoIndenizacao == imagemDocumentoRequest.NumeroProcesso
                                                && d.NuDocumentoExigido == imagemDocumentoRequest.IdDocumentoExigido && d.DhExclusao == null).FirstOrDefaultAsync();

                                if (documentoPendente != null)
                                {
                                    //Sdctb030DocumentoPendente sdctb030DocumentoPendente = await _contextLeitura.Sdctb030DocumentoPendente
                                    //                                                        .Where(d => d.NuDocumentoPendente == nuDocumentoPendente)
                                    //                                                        .FirstOrDefaultAsync();

                                    sdctb002DocumentoCapturado.NuDocumentoPendente = documentoPendente.NuDocumentoPendente;
                                    _context.Update(sdctb002DocumentoCapturado);
                                    documentoPendente.DhExclusao = DateTime.Now.AddHours(-3);
                                    if (documentoPendente.NuDocumentoCapturadoNavigation.Where(c => c.NuDocumentoPendente == documentoPendente.NuDocumentoPendente && c.DhExclusao == null).Count() > 0)
                                    {
                                        documentoPendente.NuDocumentoCapturadoNavigation.Where(c => c.NuDocumentoPendente == documentoPendente.NuDocumentoPendente && c.DhExclusao == null).FirstOrDefault().DhExclusao = DateTime.Now.AddHours(-3);
                                    }

                                    _context.Update(documentoPendente);
                                    await _context.SaveChangesAsync();
                                }

                                if (retorno != 0)
                                {
                                    docs.Add(new RespostaDocumentoUpload
                                    {
                                        IdDocumentoCapturado = sdctb002DocumentoCapturado.NuDocumentoCapturado,
                                        UrlDocumento = blob.Uri.AbsoluteUri
                                    });

                                    sucesso = true;
                                }
                                else
                                {
                                    sucesso = false;
                                    await blob.DeleteIfExistsAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                sucesso = false;
                                await blob.DeleteIfExistsAsync();
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"SalvaImagemDocumentoRequest - docs {docs.ToJson()} :  Codigo Retorno {StatusCodes.Status406NotAcceptable}");

                    return new MensagemUploadResponse
                    {
                        CodigoRetorno = StatusCodes.Status406NotAcceptable,
                        Documentos = docs
                    };
                }

                if (sucesso)
                {
                    _logger.LogInformation($"SalvaImagemDocumentoRequest - docs {docs.ToJson()} :  Codigo Retorno {StatusCodes.Status201Created}");

                    return new MensagemUploadResponse
                    {
                        CodigoRetorno = StatusCodes.Status201Created,
                        Documentos = docs
                    };
                }
                else
                {
                    _logger.LogInformation($"SalvaImagemDocumentoRequest - docs {docs.ToJson()} :  Codigo Retorno {StatusCodes.Status500InternalServerError}");

                    return new MensagemUploadResponse
                    {
                        CodigoRetorno = StatusCodes.Status500InternalServerError,
                        Documentos = docs
                    };
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"SalvaImagemDocumentoRequest - Erro {e.Message}");

                _logger.LogError($"SalvaImagemDocumentoRequest - Erro {docs.ToJson()} :  Codigo Retorno {StatusCodes.Status500InternalServerError}");
                return new MensagemUploadResponse
                {
                    CodigoRetorno = StatusCodes.Status500InternalServerError,
                    Documentos = docs
                };
            }
        }

        public async Task<PendenciaDocumentoResponse> GetPendencia(decimal CPF, long nuPedido)
        {

            _logger.LogInformation($"Start GetPendencia - Numero Pedido {nuPedido} CPF: {CPF}");


            PendenciaDocumentoResponse pendencia = new PendenciaDocumentoResponse();

            try
            {
                //TODO: refatorar a consulta abaixo, para trazer a pendencia de verdade
                pendencia = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                   join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                   join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                   join gd in _contextLeitura.Sdctb005GrupoDocumento on ti.NuTipoIndenizacao equals gd.NuTipoIndenizacao
                                   join de in _contextLeitura.Sdctb003DocumentoExigido on gd.NuGrupoDocumento equals de.NuGrupoDocumento
                                   join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                   where pi.NuPessoaSolicitanteNavigation.NuCpf == CPF
                                   orderby pi.DhPedido descending
                                   select new PendenciaDocumentoResponse
                                   {
                                       IdPedido = pi.NuPedidoIndenizacao,
                                       DePedido = ti.DeTipoIndenizacao,
                                       Documentos = new List<Documento>()
                                   {
                                       new Documento
                                       {
                                           NomeTipoDocumento = td.DeTipoDocumento,
                                           AbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                           IdDocumentoExigido = de.NuDocumentoExigido,
                                           IdGrupoDocumento = gd.NuGrupoDocumento,
                                           IdTipoDocumento = td.NuTipoDocumento,
                                           quantidadePaginas = td.QtPaginas ?? Convert.ToInt16(1)
                                       }
                                   }
                                   }
                            ).FirstOrDefaultAsync();

                _logger.LogInformation($"GetPendencia - Retorno {pendencia.ToJson()}");

                return pendencia;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetPendencia - erro {e.Message}");
                throw;
            }

        }

        public async Task<System.IO.Stream> BuscaArquivosDocumentosPorProcesso(long nuProcesso, decimal? nuCpf)
        {
            //_logger.LogInformation($"Start BuscaArquivosDocumentosPorProcesso - Numero Processo {nuProcesso} CPF: {nuCpf}");

            try
            {
                List<Sdctb002DocumentoCapturado> listaDocumentoCapturados = await _contextLeitura.Sdctb002DocumentoCapturado
                                .Include(x => x.NuPedidoIndenizacaoNavigation)
                                .ThenInclude(x => x.NuPessoaSolicitanteNavigation)
                                   .Where(x => x.NuPedidoIndenizacaoNavigation.NuPessoaSolicitanteNavigation.NuCpf == nuCpf
                                   && x.NuPedidoIndenizacao == nuProcesso)
                                   .ToListAsync();

                _logger.LogDebug($"BuscaArquivosDocumentosPorProcesso - listaDocumentoCapturados {listaDocumentoCapturados}");

                if (listaDocumentoCapturados != null)
                {
                    string connectionString = _configuration["storageConfig:connection"];
                    string containerName = _configuration["storageConfig:containerName"];

                    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

                    await container.CreateIfNotExistsAsync();

                    var memoryStream = new System.IO.MemoryStream();

                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var doc in listaDocumentoCapturados)
                        {
                            var zipFile = archive.CreateEntry(doc.DeCaminhoBlob.Replace('/', '_'));

                            BlobClient blob = container.GetBlobClient(doc.DeCaminhoBlob);
                            BlobDownloadInfo downloadedFile = await blob.DownloadAsync();

                            using (var entryStream = zipFile.Open())
                            {
                                await downloadedFile.Content.CopyToAsync(entryStream);
                            }
                        }
                    }

                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                    _logger.LogInformation($"BuscaArquivosDocumentosPorProcesso - finalizado");

                    return memoryStream;
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"BuscaArquivosDocumentosPorProcesso - erro {e.Message}");
                throw;
            }

        }

        public async Task<BuscarDocumentoResponse> BuscaArquivoDocumentoPorIdECpf(long nuDocumento, decimal? nuCpf)
        {
            //_logger.LogInformation($"Start BuscaArquivoDocumentoPorId - Numero Documento {nuDocumento} CPF: {nuCpf}");

            try
            {
                Sdctb002DocumentoCapturado doc = await _contextLeitura.Sdctb002DocumentoCapturado
                                 .Include(x => x.NuPedidoIndenizacaoNavigation)
                                 .ThenInclude(x => x.NuPessoaSolicitanteNavigation)
                                    .Where(x =>
                                    (x.NuPedidoIndenizacaoNavigation.NuPessoaSolicitanteNavigation.NuCpf == nuCpf && x.NuDocumentoCapturado == nuDocumento) ||
                                    (x.NuDocumentoCapturado == nuDocumento && nuCpf == null))
                                .FirstOrDefaultAsync();

                _logger.LogDebug($"BuscaArquivoDocumentoPorId - doc  {doc}");

                if (doc != null)
                {
                    string connectionString = _configuration["storageConfig:connection"];
                    string containerName = _configuration["storageConfig:containerName"];
                    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

                    await container.CreateIfNotExistsAsync();

                    BlobClient blob = container.GetBlobClient(doc.DeCaminhoBlob);

                    BlobDownloadInfo downloadedFile = await blob.DownloadAsync();

                    _logger.LogInformation($"BuscaArquivoDocumentoPorId - Finalizado");

                    return new BuscarDocumentoResponse
                    {
                        Content = downloadedFile.Content,
                        ContentType = downloadedFile.ContentType,
                        ContentLength = downloadedFile.ContentLength
                    };
                }

                _logger.LogInformation($"BuscaArquivoDocumentoPorId - retorno   null");

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"BuscaArquivoDocumentoPorId - erro {e.Message}");
                throw;
            }

        }

        public async Task<RemoverDocumentoResponse> RemoveDocumentoPorId(long nuDocumento, decimal nuCpf)
        {
            _logger.LogInformation($"Start RemoveDocumentoPorId - Numero Documento {nuDocumento} CPF: {nuCpf}");

            Sdctb002DocumentoCapturado doc = new Sdctb002DocumentoCapturado();
            try
            {
                RemoverDocumentoResponse response = new RemoverDocumentoResponse();

                doc = await (from dc in _contextLeitura.Sdctb002DocumentoCapturado
                             join pa in _contextLeitura.Sdctb015Participacao on dc.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                             join pe in _contextLeitura.Sdctb009Pessoa on pa.NuPessoaParticipante equals pe.NuPessoa
                             where dc.NuDocumentoCapturado == nuDocumento && pe.NuCpf == nuCpf
                             select dc
                           ).FirstOrDefaultAsync();

                _logger.LogDebug($"RemoveDocumentoPorId - Sdctb002DocumentoCapturado doc   {doc}");

                if (doc != null)
                {
                    doc.DhExclusao = DateTime.Now.AddHours(-3);
                    _context.Update(doc);

                    if (await _context.SaveChangesAsync() != 0)
                    {
                        var nomeDocumento = doc.DeCaminhoBlob.Split('/').LastOrDefault();
                        response.NomeDocumento = nomeDocumento;
                        response.Mensagem = "Documento removido.";
                        response.StatusCode = 200;

                        string connectionString = _configuration["storageConfig:connection"];
                        string containerName = _configuration["storageConfig:containerName"];
                        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                        BlobClient blob = container.GetBlobClient(doc.DeCaminhoBlob);
                        BlobDownloadInfo downloadedFile = await blob.DownloadAsync();
                        var caminhoBlobRenomeado = doc.DeCaminhoBlob.Replace(nomeDocumento, $"{DateTime.Now.Ticks}_{nomeDocumento}");
                        BlobClient blobTemp = container.GetBlobClient(caminhoBlobRenomeado);
                        BlobUploadOptions options = new BlobUploadOptions();
                        options.HttpHeaders = new BlobHttpHeaders() { ContentType = downloadedFile.ContentType };
                        var uploadResponse = await blob.UploadAsync(downloadedFile.Content, options);

                        if (uploadResponse.GetRawResponse().Status.IsSuccessStatusCode())
                        {
                            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                        }

                        _logger.LogInformation($"RemoveDocumentoPorId - retorno: response   {response}");

                        return response;
                    }
                }

                response.Mensagem = "Não foi possível remover o documento.";
                response.StatusCode = 500;

                _logger.LogInformation($"RemoveDocumentoPorId - retorno: response   {response}");

                return response;

            }
            catch (Exception e)
            {
                _logger.LogError($"RemoveDocumentoPorId - erro {e.Message}");
                throw;
            }

        }

        public async Task<List<DocumentoEnviadoResponse>> ListaDeDocumentosEnviadosPorProcesso(long nuProcesso, decimal nuCpf)
        {
            _logger.LogInformation($"Start ListaDeDocumentosEnviadosPorProcesso - Numero Processo {nuProcesso} CPF: {nuCpf}");

            try
            {
                List<DocumentoEnviadoResponse> docsEnviados = new List<DocumentoEnviadoResponse>();

                List<Sdctb002DocumentoCapturado> docs = await _contextLeitura.Sdctb002DocumentoCapturado
                    .Include(x => x.NuPedidoIndenizacaoNavigation)
                    .ThenInclude(x => x.NuPessoaSolicitanteNavigation)
                    .Include(x => x.NuDocumentoExigidoNavigation)
                    .ThenInclude(x => x.NuTipoDocumentoNavigation)
                    .Where(x => x.NuPedidoIndenizacao == nuProcesso && x.DhExclusao == null
                    && x.NuPedidoIndenizacaoNavigation.NuPessoaSolicitanteNavigation.NuCpf == nuCpf)
                    .ToListAsync();

                _logger.LogDebug($"ListaDeDocumentosEnviadosPorProcesso - docsEnviados   {docsEnviados}");

                docs.ForEach(d =>
                {
                    docsEnviados.Add(new DocumentoEnviadoResponse
                    {
                        DataEnvio = d.DhInclusao,
                        NomeDocumento = d.DeCaminhoBlob.Split("/").LastOrDefault(),
                        NomeTipoDocumento = d.NuDocumentoExigidoNavigation.NuTipoDocumentoNavigation.DeTipoDocumento
                    });
                });

                _logger.LogInformation($"ListaDeDocumentosEnviadosPorProcesso - retorno:docsEnviados   {docsEnviados}");

                return docsEnviados;
            }
            catch (Exception e)
            {
                _logger.LogError($"ListaDeDocumentosEnviadosPorProcesso - erro {e.Message}");
                throw;
            }

        }

        public async Task<List<Documento>> ListarDocumentosPendentesPorPedido(long nuPedido, decimal nuCpf)
        {
            try
            {

                List<Documento> DocumentosPendentes = new List<Documento>();
                DocumentosPendentes = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                             join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                             join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                             join dp in _contextLeitura.Sdctb030DocumentoPendente on sp.NuSituacaoPedido equals dp.NuSituacaoPedido
                                             join de in _contextLeitura.Sdctb003DocumentoExigido on dp.NuDocumentoExigido equals de.NuDocumentoExigido
                                             join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento into tempVi
                                             from vi in tempVi.DefaultIfEmpty()
                                             where pi.NuPedidoIndenizacao == nuPedido && p.NuCpf == nuCpf && dp.DhExclusao == null
                                             select new Documento
                                             {
                                                 IdGrupoDocumento = de.NuGrupoDocumento,
                                                 IdDocumentoExigido = de.NuDocumentoExigido,
                                                 IdTipoDocumento = vi.NuTipoDocumento,
                                                 NomeTipoDocumento = vi.DeTipoDocumento,
                                                 AbreviaturaTipoDocumento = vi.DeAbreviaturaTipoDocumento,
                                                 quantidadePaginas = vi.QtPaginas,
                                             }).ToListAsync();

                return await ConcatenaTipoParticipacao(DocumentosPendentes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AppSeguroCaixa - ListarDocumentosPendentes: {nuPedido} - {nuCpf}");
                return null;
            }
        }

        private async Task<List<Documento>> ConcatenaTipoParticipacao(List<Documento> DocumentosPendentes)
        {

            List<Sdctb005GrupoDocumento> sdctb005Grupos = await _contextLeitura.Sdctb005GrupoDocumento.ToListAsync();
            foreach (var doc in DocumentosPendentes)
            {

                switch (sdctb005Grupos.Where(x => x.NuGrupoDocumento == doc.IdGrupoDocumento).FirstOrDefault().NuTipoParticipacao)
                {
                    case 1:
                        doc.NomeTipoDocumento += " - Vítima";
                        break;
                    case 2:
                        doc.NomeTipoDocumento += " - Beneficiário";
                        break;
                    case 3:
                        doc.NomeTipoDocumento += " - Representante Legal";
                        break;
                    case 4:
                        doc.NomeTipoDocumento += " - Procurador";
                        break;
                    default:
                        doc.NomeTipoDocumento += " - Vítima";
                        break;
                }

            }

            return DocumentosPendentes;
        }

        public async Task<BuscarDocumentoResponse> BuscaArquivoModeloProcuracao()
        {
            _logger.LogInformation($"Start BuscaArquivoModeloProcuracao ");

            try
            {
                string connectionString = _configuration["storageConfig:connection"];
                string containerName = "documentos";
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

                await container.CreateIfNotExistsAsync();
                BlobClient blob = container.GetBlobClient("MO-38-379_v001.doc");
                if (blob != null)
                {

                    BlobDownloadInfo downloadedFile = await blob.DownloadAsync();

                    _logger.LogInformation($"BuscaArquivoModeloProcuracao - Finalizado");

                    return new BuscarDocumentoResponse
                    {
                        Content = downloadedFile.Content,
                        ContentType = downloadedFile.ContentType,
                        ContentLength = downloadedFile.ContentLength
                    };
                }

                _logger.LogInformation($"BuscaArquivoModeloProcuracao - retorno   null");

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"BuscaArquivoModeloProcuracao - erro {e.Message}");
                throw;
            }

        }
    }
}
