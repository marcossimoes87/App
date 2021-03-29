using BackOfficeSeguroCaixa.Filtros;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Request;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SeguroCaixa.DTO.Response.RespostaPadraoProcessoResponse;
using static SeguroCaixa.Enums;

namespace SeguroCaixa.Services
{
    public class ProcessoService
    {
        #region Inicializadores e Construtor
        private readonly DbEscrita _contextEscrita;
        private readonly DbLeitura _contextLeitura;
        private readonly DbLeituraHistorico _contextLeituraHistorico;
        private readonly ILogger<ProcessoService> _logger;
        private IConfiguration _configuration;

        public ProcessoService(DbEscrita context, ILogger<ProcessoService> logger, DbLeitura contextLeitura, DbLeituraHistorico contextLeituraHistorico, IConfiguration configuration)
        {
            _contextEscrita = context;
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
            _contextLeituraHistorico = contextLeituraHistorico;
        }
        #endregion

        public async Task<ProcessoPaginado> ListarProcessos(ProcessoFiltro filtro = null, ProcessoPaginacao paginacao = null, string MatriculaFuncCaixa = "")
        {
            _logger.LogInformation($"ProcessoService ListarProcessos -->filtro {filtro}, paginacao {paginacao} , MatriculaFuncCaixa {MatriculaFuncCaixa}");
            IQueryable<Sdctb008PedidoIndenizacao> query;

            query = (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                     .Include(x => x.Sdctb015Participacaos)
                     .ThenInclude(x => x.NuPessoaParticipanteNavigation)
                     .Include(x => x.NuSituacaoPedidoNavigation)
                     .ThenInclude(x => x.Sdctb018SituacaoMotivos)
                     where (pi.NuSituacaoPedidoNavigation.CoMatriculaCaixa == MatriculaFuncCaixa
                        || pi.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 2
                            && pi.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => m.NuTipoMotivoSituacao == 1))
                     select pi)
                     .AplicaFiltro(filtro);

             return ProcessoPaginado.From(paginacao, query);

        }

        public async Task<ProcessoResponse> RetornaProcessoPorId(long numeroProcesso)
        {
            try
            {
                var pedido = await _contextLeitura
                    .Sdctb008PedidoIndenizacao
                    .FirstOrDefaultAsync(x => x.NuPedidoIndenizacao == numeroProcesso);

                if (pedido != null)
                    return new ProcessoResponse()
                    {
                        NuPedidoIndenizacao = pedido.NuPedidoIndenizacao
                    };

                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //TODO: ao clicar em resolver no front, atualiza o status do pedido para em analise e grava a matricula do atendente
        public async Task<MensagemProcessoResponse> AtualizaStatusProcesso(short NuProcesso, decimal nuCpf, string CoMatriculaCaixa)
        {
            _logger.LogInformation($"AtualizaStatusProcesso - incio - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
            try
            {
                await ResilientTransaction.New(_contextEscrita).ExecuteAsync(async () =>
                {
                    // Busca o pedido desde que seja originado do mesmo solicitante
                    Sdctb008PedidoIndenizacao pedidoIndenizacao = await _contextEscrita.Sdctb008PedidoIndenizacao
                                                                    .Include(c => c.NuSituacaoPedidoNavigation)
                                                                    .Where(s => s.NuPedidoIndenizacao == NuProcesso)
                                                                    .FirstOrDefaultAsync();

                    // setando data fim situação pedido anterior
                    _logger.LogInformation($"AtualizaStatusProcesso - Setando data fim situação pedido anterior");
                    await SetarDataExclusaoSituacoesPedido(pedidoIndenizacao.NuPedidoIndenizacao);
                    // cadastrar situacao pedido


                    await _contextEscrita.SaveChangesAsync();

                    // Salvar nova situação do pedido
                    _logger.LogInformation($"AtualizaStatusProcesso - Salvar nova situação do pedido - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    Sdctb010SituacaoPedido sdctb010SituacaoPedido = new Sdctb010SituacaoPedido()
                    {
                        NuTipoSituacaoPedido = 3,
                        DhSituacao = DateTime.Now.AddHours(-3),
                        NuPedidoIndenizacao = pedidoIndenizacao.NuPedidoIndenizacao,
                        CoMatriculaCaixa = CoMatriculaCaixa.Trim() == "" ? null : CoMatriculaCaixa.Trim(),

                    };

                    _contextEscrita.Add(sdctb010SituacaoPedido);
                    await _contextEscrita.SaveChangesAsync();

                    // Busca o ID que acabou de ser incluído
                    pedidoIndenizacao.NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido;

                    _logger.LogInformation($"AtualizaStatusProcesso - Atualiza o pedido com a nova situação - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    // Atualiza o pedido com a nova situação
                    _contextEscrita.Update(pedidoIndenizacao);
                    await _contextEscrita.SaveChangesAsync();

                    // Atualiza o tipo do motivo da situação
                    _logger.LogInformation($"AtualizaStatusProcesso -  Atualiza o tipo do motivo da situação - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    Sdctb018SituacaoMotivo sdctb018SituacaoMotivo = new Sdctb018SituacaoMotivo()
                    {
                        NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido,
                        NuTipoMotivoSituacao = 7,
                        DhSituacao = DateTime.Now.AddHours(-3)
                    };
                    _contextEscrita.Add(sdctb018SituacaoMotivo);
                    await _contextEscrita.SaveChangesAsync();

                    mensagemProcessoResponse.CodigoRetorno = 200;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = pedidoIndenizacao.NuPedidoIndenizacao.ToString(), Mensagem = "Pedido atualizado com sucesso" };
                });
            }
            catch (Exception e)
            {

                _logger.LogError($"AtualizaProcesso - error {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = NuProcesso.ToString(), Mensagem = "Dados invalidos para gravacao" };

                _logger.LogError($"AtualizaProcesso -error-  mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");

                await _contextEscrita.Database.RollbackTransactionAsync();

                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        //TODO: carrega todos os dados do pedido para tela de atendimento
        /**/
        public async Task<DadosProcessoResponse> CarregaInformacaoProcesso(short NuProcesso, decimal nuCpf, string CoMatriculaCaixa)
        {
            _logger.LogInformation($"CarregaInformacaoProcesso - incio - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

            DadosProcessoResponse dadosProcessoResponse = new DadosProcessoResponse();
            try
            {
                Sdctb008PedidoIndenizacao sdctb008PedidoIndenizacao = await _contextLeitura.Sdctb008PedidoIndenizacao
                                                                                .Include(c => c.NuSituacaoPedidoNavigation)
                                                                                .Where(p => p.NuPedidoIndenizacao == NuProcesso
                                                                                        && p.NuPessoaSolicitanteNavigation.NuCpf == nuCpf).FirstOrDefaultAsync();

                //setando o NuCanal e protocolo do Gedan caso tenha
                dadosProcessoResponse.NuCanal = sdctb008PedidoIndenizacao.NuCanal;
                var protocoloGedan = await _contextLeitura.Sdctb054GedamPedidoIndenizacaos
                                                                                .Where(p => p.NuPedidoIndenizacao == NuProcesso).FirstOrDefaultAsync();
                protocoloGedan = protocoloGedan == null ? new Sdctb054GedamPedidoIndenizacao() : protocoloGedan;

                List<Sdctb007TipoIndenizacao> sdctb007TipoIndenizacao = await _contextEscrita.Sdctb007TipoIndenizacao.ToListAsync();

                if (!sdctb008PedidoIndenizacao.NuSituacaoPedidoNavigation.CoMatriculaCaixa.Trim().ToLower().Equals(CoMatriculaCaixa.Trim().ToLower()))
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Usuario não autorizado. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    throw new ArgumentException(String.Format("Este Pedido não está vinculado na Matricula: {0}", CoMatriculaCaixa));
                }

                _logger.LogInformation($"CarregaInformacaoProcesso - Carregando Dados Analise. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                dadosProcessoResponse.DadosAnalise = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                            join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                                            join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                            join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                            join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                            join tp in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tp.NuTipoParticipacao
                                                            join sm in _contextLeitura.Sdctb018SituacaoMotivo on sp.NuSituacaoPedido equals sm.NuSituacaoPedido
                                                            join tms in _contextLeitura.Sdctb017TipoMotivoSituacao on sm.NuTipoMotivoSituacao equals tms.NuTipoMotivoSituacao
                                                            where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa
                                                            orderby sp.NuSituacaoPedido descending
                                                            select new DadosAnaliseMeritoResponse
                                                            {
                                                                CabecalhoBloco = "Análise do mérito",
                                                                NuPedidoIndenizacao = pi.NuPedidoIndenizacao,
                                                                NuTipoIndenizacao = ti.NuTipoIndenizacao,
                                                                DeAbreviaturaTipoIndenizaca = ti.DeAbreviaturaTipoIndenizaca,
                                                                NuTipoParticipacao = pa.NuTipoParticipacao,
                                                                DeTipoParticipacao = tp.DeTipoParticipacao,
                                                                DhSituacao = sp.DhSituacao,
                                                                CoMatriculaCaixa = sp.CoMatriculaCaixa,
                                                                NuSituacaoPedido = sp.NuSituacaoPedido,
                                                                NuTipoSituacaoPedido = sm.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido,
                                                                NuTipoMotivoSituacao = tms.NuTipoMotivoSituacao,
                                                                DeTipoMotivoSituacao = tms.DeTipoMotivoSituacao,
                                                                DeAbreviaturaTipoMotivoSituacao = tms.DeAbreviaturaMotivoSituacao
                                                            }).FirstOrDefaultAsync();

                //setando o status
                ProcessoResponse processoResponse = new ProcessoResponse();
                ProcessoPaginado.SetarStatusPedido(new Sdctb008PedidoIndenizacao()
                {
                    NuSituacaoPedido = dadosProcessoResponse.DadosAnalise.NuSituacaoPedido,
                    NuSituacaoPedidoNavigation = new Sdctb010SituacaoPedido()
                    {
                        NuTipoSituacaoPedido = dadosProcessoResponse.DadosAnalise.NuTipoSituacaoPedido,
                        Sdctb018SituacaoMotivos = _contextLeitura.Sdctb018SituacaoMotivo.Where(x => x.NuSituacaoPedido == dadosProcessoResponse.DadosAnalise.NuSituacaoPedido).ToList()
                    }

                }, processoResponse);
                dadosProcessoResponse.DadosAnalise.StatusPedido = processoResponse.StatusPedido;

                _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados do acidente. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                dadosProcessoResponse.DadosAcidente = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                             join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                                             join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                             join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                             join tv in _contextLeitura.Sdctb012TipoVeiculo on pi.NuTipoVeiculo equals tv.NuTipoVeiculo
                                                             join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                             into tempVI
                                                             from vi in tempVI.DefaultIfEmpty()
                                                             where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa
                                                             orderby sp.DhSituacao descending
                                                             select new DadosAcidenteResponse
                                                             {
                                                                 CabecalhoBloco = "Dados do acidente",
                                                                 DhSinistro = pi.DhSinistro,
                                                                 NuTipoVeiculo = pi.NuTipoVeiculo,
                                                                 DeAbreviaturaTipoVeiculo = tv.DeAbreviaturaTipoVeiculo,
                                                                 DeMunicipio = pi.NuMunicipioNavigation.DeMunicipio,
                                                                 NoBairro = pi.NoBairro,
                                                                 NoLogradouro = pi.NoLogradouro,
                                                                 CoPosicaoImovel = pi.CoPosicaoImovel,
                                                                 CoEstado = pi.NuMunicipioNavigation.CoUf,
                                                                 Complemento = pi.DeComplemento,
                                                                 DtSinistro = pi.DhSinistro.ToString("yyyy-mm-dd"),
                                                                 HrSinistro = pi.DhSinistro.ToString("HH:mm")

                                                             }).FirstOrDefaultAsync();

                if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados do beneficiario. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DadosPessoais = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                 join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                                 join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                                 join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                 join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                 into tempVI
                                                                 from vi in tempVI.DefaultIfEmpty()
                                                                 where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa && pa.NuTipoParticipacao == 2
                                                                 select new DadosPessoaisBlocoResponse
                                                                 {
                                                                     CabecalhoBloco = "Documentos pessoais",
                                                                     IdPessoa = p.NuPessoa,
                                                                     Nome = p.NoPessoa,
                                                                     DataNascimento = p.DtNascimento,
                                                                     Cpf = p.NuCpf,
                                                                     Genero = p.CoGenero,
                                                                     NomeMae = p.NoMae,
                                                                     NuNacionalidade = pa.NuNacionalidade,
                                                                     NoNacionalidade = pa.NuNacionalidadeNavigation.NoNacionalidade,
                                                                     CoEstado = pi.NuMunicipioNavigation.CoUf,
                                                                     NuTipoDocumento = pa.NuTipoDocumento,
                                                                     CoDocumento = pa.CoDocumento,
                                                                     CoOrgaoEmissor = pa.CoOrgaoEmissor,
                                                                     CoUfOrgaoEmissor = pa.CoUfOrgaoEmissor,
                                                                     DtExpedicao = pa.DtExpedicao,
                                                                     TipoParticipacao = vi.NuTipoParticipacao,
                                                                     DeTipoParticipacao = vi.DeAbreviaTipoParticipacao

                                                                 }).FirstOrDefaultAsync();


                    dadosProcessoResponse.DadosPessoais.Documentos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                            join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                            join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                            join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                            join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                            join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                            join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                            where pi.NuPedidoIndenizacao == NuProcesso
                                                                            && (gd.NuGrupoDocumento == 15 || gd.NuGrupoDocumento == 13)
                                                                            && pa.NuTipoParticipacao == 2
                                                                            && tpa.NuTipoParticipacao == 2
                                                                            && dc.DhExclusao == null
                                                                            orderby td.NuTipoDocumento
                                                                            select new DocumentosInfoResponse
                                                                            {
                                                                                NuTipoDocumento = td.NuTipoDocumento,
                                                                                QtPaginas = td.QtPaginas,
                                                                                NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                                DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                                DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                                DeUrlImagem = dc.DeUrlImagem,
                                                                                NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                                DeTipoDocumento = td.DeTipoDocumento,
                                                                                IdTipoDocumento = td.NuTipoDocumento,
                                                                                NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                                DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                                DeNomeArquivo = dc.DeNomeArquivo,
                                                                                NuDocumentoExigido = de.NuDocumentoExigido,
                                                                                Aprovado = dc.IcRejeitado,
                                                                                TipoParticipacao = tpa.NuTipoParticipacao,
                                                                                DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                                            }).ToListAsync();

                    foreach (var doc in dadosProcessoResponse.DadosPessoais.Documentos)
                    {
                        doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                     where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                     select new Documento
                                                     {
                                                         IdGrupoDocumento = de.NuGrupoDocumento,
                                                         IdDocumentoExigido = de.NuDocumentoExigido,
                                                         IdTipoDocumento = de.NuTipoDocumento,
                                                         NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                         AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                         quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                     }).ToListAsync();
                    }

                    dadosProcessoResponse.DadosPessoais.CamposDocumento = await (from tid in _contextLeitura.Sdctb047TipoItemDocumentos
                                                                                 join ide in _contextLeitura.Sdctb048ItemDocumentoExigidos on tid.NuItemDocumento equals ide.NuItemDocumento
                                                                                 join std in _contextLeitura.Sdctb006TipoDocumento on ide.NuTipoDocumento equals std.NuTipoDocumento
                                                                                 join sde in _contextLeitura.Sdctb003DocumentoExigido on std.NuTipoDocumento equals sde.NuTipoDocumento
                                                                                 join sdc in _contextLeitura.Sdctb002DocumentoCapturado on sde.NuDocumentoExigido equals sdc.NuDocumentoExigido
                                                                                 join idc in _contextLeitura.Sdctb049ItemDocumentoConteudos on
                                                                                 new
                                                                                 {
                                                                                     key1 = tid.NuItemDocumento,
                                                                                     key2 = sdc.NuDocumentoCapturado
                                                                                 }
                                                                                 equals
                                                                                 new
                                                                                 {
                                                                                     key1 = idc.NuItemDocumento,
                                                                                     key2 = idc.NuDocumentoCapturado
                                                                                 }
                                                                                 into tempVI
                                                                                 from vi in tempVI.DefaultIfEmpty()
                                                                                 where ide.NuTipoDocumento == dadosProcessoResponse.DadosPessoais.Documentos.FirstOrDefault().NuTipoDocumento
                                                                                 && sdc.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                                                 && sdc.NuDocumentoCapturado == dadosProcessoResponse.DadosPessoais.Documentos.FirstOrDefault().NuDocumentoCapturado
                                                                                 select new CamposDocumentoResponse
                                                                                 {
                                                                                     NuItemDocumentoConteudo = vi.NuItemDocumentoConteudo,
                                                                                     Titulo = tid.DeItemDocumentoTitulo,
                                                                                     Conteudo = vi.DeConteudo,
                                                                                     NuItemConteudo = ide.NuItemDocumento

                                                                                 }).ToListAsync();

                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados da vitima. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DadosPessoaisVitima = await (from pa in _contextLeitura.Sdctb015Participacao
                                                                       join p in _contextLeitura.Sdctb009Pessoa on pa.NuPessoaParticipante equals p.NuPessoa
                                                                       join sp in _contextLeitura.Sdctb010SituacaoPedido on pa.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                                       join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                        into tempVI
                                                                       from vi in tempVI.DefaultIfEmpty()
                                                                       where pa.NuPedidoIndenizacao == NuProcesso && sp.CoMatriculaCaixa == CoMatriculaCaixa && pa.NuTipoParticipacao == 1
                                                                       select new DadosPessoaisBlocoResponse
                                                                       {
                                                                           CabecalhoBloco = "Documentos pessoais",
                                                                           IdPessoa = p.NuPessoa,
                                                                           Nome = p.NoPessoa,
                                                                           DataNascimento = p.DtNascimento,
                                                                           Cpf = p.NuCpf,
                                                                           Genero = p.CoGenero,
                                                                           NomeMae = p.NoMae,
                                                                           NuNacionalidade = pa.NuNacionalidade,
                                                                           NoNacionalidade = pa.NuNacionalidadeNavigation.NoNacionalidade,
                                                                           CoEstado = "",
                                                                           NuTipoDocumento = pa.NuTipoDocumento,
                                                                           CoDocumento = pa.CoDocumento,
                                                                           CoOrgaoEmissor = pa.CoOrgaoEmissor,
                                                                           CoUfOrgaoEmissor = pa.CoUfOrgaoEmissor,
                                                                           DtExpedicao = pa.DtExpedicao,
                                                                           TipoParticipacao = vi.NuTipoParticipacao,
                                                                           DeTipoParticipacao = vi.DeAbreviaTipoParticipacao

                                                                       }).FirstOrDefaultAsync();

                    dadosProcessoResponse.DadosPessoaisVitima.Documentos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                                  join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                                  join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                                  join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                                  join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                                  join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                                  join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                                  where pi.NuPedidoIndenizacao == NuProcesso
                                                                                  && gd.NuGrupoDocumento == 16
                                                                                  && (td.NuTipoDocumento == 1 || td.NuTipoDocumento == 7)
                                                                                  && pa.NuTipoParticipacao == 1
                                                                                  && gd.NuTipoParticipacao == null
                                                                                  && dc.DhExclusao == null
                                                                                  && gd.NuTipoIndenizacao == sdctb008PedidoIndenizacao.NuTipoIndenizacao
                                                                                  orderby td.NuTipoDocumento
                                                                                  select new DocumentosInfoResponse
                                                                                  {
                                                                                      NuTipoDocumento = td.NuTipoDocumento,
                                                                                      QtPaginas = td.QtPaginas,
                                                                                      NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                                      DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                                      DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                                      DeUrlImagem = dc.DeUrlImagem,
                                                                                      NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                                      DeTipoDocumento = td.DeTipoDocumento,
                                                                                      IdTipoDocumento = td.NuTipoDocumento,
                                                                                      NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                                      DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                                      DeNomeArquivo = dc.DeNomeArquivo,
                                                                                      NuDocumentoExigido = de.NuDocumentoExigido,
                                                                                      Aprovado = dc.IcRejeitado,
                                                                                      TipoParticipacao = tpa.NuTipoParticipacao,
                                                                                      DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                                                  }).ToListAsync();

                    foreach (var doc in dadosProcessoResponse.DadosPessoaisVitima.Documentos)
                    {
                        doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                     where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                     select new Documento
                                                     {
                                                         IdGrupoDocumento = de.NuGrupoDocumento,
                                                         IdDocumentoExigido = de.NuDocumentoExigido,
                                                         IdTipoDocumento = de.NuTipoDocumento,
                                                         NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                         AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                         quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                     }).ToListAsync();
                    }

                    dadosProcessoResponse.DadosPessoaisVitima.CamposDocumento = await (from tid in _contextLeitura.Sdctb047TipoItemDocumentos
                                                                                       join ide in _contextLeitura.Sdctb048ItemDocumentoExigidos on tid.NuItemDocumento equals ide.NuItemDocumento
                                                                                       join std in _contextLeitura.Sdctb006TipoDocumento on ide.NuTipoDocumento equals std.NuTipoDocumento
                                                                                       join sde in _contextLeitura.Sdctb003DocumentoExigido on std.NuTipoDocumento equals sde.NuTipoDocumento
                                                                                       join sdc in _contextLeitura.Sdctb002DocumentoCapturado on sde.NuDocumentoExigido equals sdc.NuDocumentoExigido
                                                                                       join idc in _contextLeitura.Sdctb049ItemDocumentoConteudos on
                                                                                       new
                                                                                       {
                                                                                           key1 = tid.NuItemDocumento,
                                                                                           key2 = sdc.NuDocumentoCapturado
                                                                                       }
                                                                                       equals
                                                                                       new
                                                                                       {
                                                                                           key1 = idc.NuItemDocumento,
                                                                                           key2 = idc.NuDocumentoCapturado
                                                                                       }
                                                                                       into tempVI
                                                                                       from vi in tempVI.DefaultIfEmpty()
                                                                                       where ide.NuTipoDocumento == dadosProcessoResponse.DadosPessoaisVitima.Documentos.FirstOrDefault().NuTipoDocumento
                                                                                       && sdc.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                                                       && sdc.NuDocumentoCapturado == dadosProcessoResponse.DadosPessoaisVitima.Documentos.FirstOrDefault().NuDocumentoCapturado
                                                                                       select new CamposDocumentoResponse
                                                                                       {
                                                                                           NuItemDocumentoConteudo = vi.NuItemDocumentoConteudo,
                                                                                           Titulo = tid.DeItemDocumentoTitulo,
                                                                                           Conteudo = vi.DeConteudo,
                                                                                           NuItemConteudo = ide.NuItemDocumento

                                                                                       }).ToListAsync();
                }
                else
                {

                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados beneficiario Indenização != Morte. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DadosPessoais = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                 join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                                 join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                                 join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                 join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                 into tempVI
                                                                 from vi in tempVI.DefaultIfEmpty()
                                                                 where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa
                                                                 select new DadosPessoaisBlocoResponse
                                                                 {
                                                                     CabecalhoBloco = "Documentos pessoais",
                                                                     IdPessoa = p.NuPessoa,
                                                                     Nome = p.NoPessoa,
                                                                     DataNascimento = p.DtNascimento,
                                                                     Cpf = p.NuCpf,
                                                                     Genero = p.CoGenero,
                                                                     NomeMae = p.NoMae,
                                                                     NuNacionalidade = pa.NuNacionalidade,
                                                                     NoNacionalidade = pa.NuNacionalidadeNavigation.NoNacionalidade,
                                                                     CoEstado = pi.NuMunicipioNavigation.CoUf,
                                                                     NuTipoDocumento = pa.NuTipoDocumento,
                                                                     CoDocumento = pa.CoDocumento,
                                                                     CoOrgaoEmissor = pa.CoOrgaoEmissor,
                                                                     CoUfOrgaoEmissor = pa.CoUfOrgaoEmissor,
                                                                     DtExpedicao = pa.DtExpedicao,
                                                                     TipoParticipacao = vi.NuTipoParticipacao,
                                                                     DeTipoParticipacao = vi.DeAbreviaTipoParticipacao

                                                                 }).FirstOrDefaultAsync();


                    dadosProcessoResponse.DadosPessoais.Documentos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                            join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                            join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                            join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                            join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                            join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                            join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                            where pi.NuPedidoIndenizacao == NuProcesso
                                                                            && (gd.NuGrupoDocumento == 1 || gd.NuGrupoDocumento == 5 || gd.NuGrupoDocumento == 9 || gd.NuGrupoDocumento == 13)
                                                                            && dc.DhExclusao == null
                                                                            orderby td.NuTipoDocumento
                                                                            select new DocumentosInfoResponse
                                                                            {
                                                                                NuTipoDocumento = td.NuTipoDocumento,
                                                                                QtPaginas = td.QtPaginas,
                                                                                NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                                DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                                DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                                DeUrlImagem = dc.DeUrlImagem,
                                                                                NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                                DeTipoDocumento = td.DeTipoDocumento,
                                                                                IdTipoDocumento = td.NuTipoDocumento,
                                                                                NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                                DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                                DeNomeArquivo = dc.DeNomeArquivo,
                                                                                NuDocumentoExigido = de.NuDocumentoExigido,
                                                                                Aprovado = dc.IcRejeitado,
                                                                                TipoParticipacao = tpa.NuTipoParticipacao,
                                                                                DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                                            }).ToListAsync();

                    foreach (var doc in dadosProcessoResponse.DadosPessoais.Documentos)
                    {
                        doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                     where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                     select new Documento
                                                     {
                                                         IdGrupoDocumento = de.NuGrupoDocumento,
                                                         IdDocumentoExigido = de.NuDocumentoExigido,
                                                         IdTipoDocumento = de.NuTipoDocumento,
                                                         NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                         AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                         quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                     }).ToListAsync();
                    }

                    dadosProcessoResponse.DadosPessoais.CamposDocumento = await (from tid in _contextLeitura.Sdctb047TipoItemDocumentos
                                                                                 join ide in _contextLeitura.Sdctb048ItemDocumentoExigidos on tid.NuItemDocumento equals ide.NuItemDocumento
                                                                                 join std in _contextLeitura.Sdctb006TipoDocumento on ide.NuTipoDocumento equals std.NuTipoDocumento
                                                                                 join sde in _contextLeitura.Sdctb003DocumentoExigido on std.NuTipoDocumento equals sde.NuTipoDocumento
                                                                                 join sdc in _contextLeitura.Sdctb002DocumentoCapturado on sde.NuDocumentoExigido equals sdc.NuDocumentoExigido
                                                                                 join idc in _contextLeitura.Sdctb049ItemDocumentoConteudos on
                                                                                 new
                                                                                 {
                                                                                     key1 = tid.NuItemDocumento,
                                                                                     key2 = sdc.NuDocumentoCapturado
                                                                                 }
                                                                                 equals
                                                                                 new
                                                                                 {
                                                                                     key1 = idc.NuItemDocumento,
                                                                                     key2 = idc.NuDocumentoCapturado
                                                                                 }
                                                                                 into tempVI
                                                                                 from vi in tempVI.DefaultIfEmpty()
                                                                                 where ide.NuTipoDocumento == dadosProcessoResponse.DadosPessoais.Documentos.FirstOrDefault().NuTipoDocumento
                                                                                 && sdc.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                                                 && sdc.NuDocumentoCapturado == dadosProcessoResponse.DadosPessoais.Documentos.FirstOrDefault().NuDocumentoCapturado
                                                                                 select new CamposDocumentoResponse
                                                                                 {
                                                                                     NuItemDocumentoConteudo = vi.NuItemDocumentoConteudo,
                                                                                     Titulo = tid.DeItemDocumentoTitulo,
                                                                                     Conteudo = vi.DeConteudo,
                                                                                     NuItemConteudo = ide.NuItemDocumento

                                                                                 }).ToListAsync();
                }



                if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados residencia. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DadosResidencia = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                   join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                                   join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                                   join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                   join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                   where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa && pa.NuTipoParticipacao == 2
                                                                   select new DadosComprovanteResidenciaResponse
                                                                   {
                                                                       CabecalhoBloco = "Comprovante de Residência",
                                                                       DeBairro = pa.NoBairro,
                                                                       DeCep = pa.NuCep,
                                                                       IdMunicipio = pa.NuMunicipioNavigation.NuMunicipio,
                                                                       DeMunicipio = pa.NuMunicipioNavigation.DeMunicipio,
                                                                       DeLogradouro = pa.NoLogradouro,
                                                                       DeNumero = pa.CoPosicaoImovel,
                                                                       CoEstado = pi.NuMunicipioNavigation.CoUf,
                                                                       TipoParticipacao = tpa.NuTipoParticipacao,
                                                                       DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                                   }).FirstOrDefaultAsync();
                }
                else
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados residencia. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DadosResidencia = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                   join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                                                   join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                                                   join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                   join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                   where pi.NuPedidoIndenizacao == NuProcesso && p.NuCpf == nuCpf && sp.CoMatriculaCaixa == CoMatriculaCaixa
                                                                   select new DadosComprovanteResidenciaResponse
                                                                   {
                                                                       CabecalhoBloco = "Comprovante de Residência",
                                                                       DeBairro = pa.NoBairro,
                                                                       DeCep = pa.NuCep,
                                                                       IdMunicipio = pa.NuMunicipioNavigation.NuMunicipio,
                                                                       DeMunicipio = pa.NuMunicipioNavigation.DeMunicipio,
                                                                       DeLogradouro = pa.NoLogradouro,
                                                                       DeNumero = pa.CoPosicaoImovel,
                                                                       CoEstado = pi.NuMunicipioNavigation.CoUf,
                                                                       TipoParticipacao = tpa.NuTipoParticipacao,
                                                                       DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                                   }).FirstOrDefaultAsync();
                }


                dadosProcessoResponse.DadosResidencia.Documentos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                          join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                          join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                          join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                          join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                          where pi.NuPedidoIndenizacao == NuProcesso
                                                                          && td.NuTipoDocumento == 8
                                                                          && dc.DhExclusao == null
                                                                          orderby dc.DhInclusao
                                                                          select new DocumentosInfoResponse
                                                                          {
                                                                              NuTipoDocumento = td.NuTipoDocumento,
                                                                              QtPaginas = td.QtPaginas,
                                                                              NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                              DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                              DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                              DeUrlImagem = dc.DeUrlImagem,
                                                                              NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                              DeTipoDocumento = td.DeTipoDocumento,
                                                                              IdTipoDocumento = td.NuTipoDocumento,
                                                                              NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                              DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                              DeNomeArquivo = dc.DeNomeArquivo,
                                                                              NuDocumentoExigido = de.NuDocumentoExigido,
                                                                              Aprovado = dc.IcRejeitado
                                                                          }).ToListAsync();

                foreach (var doc in dadosProcessoResponse.DadosResidencia.Documentos)
                {
                    doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                 where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                 select new Documento
                                                 {
                                                     IdGrupoDocumento = de.NuGrupoDocumento,
                                                     IdDocumentoExigido = de.NuDocumentoExigido,
                                                     IdTipoDocumento = de.NuTipoDocumento,
                                                     NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                     AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                     quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                 }).ToListAsync();
                }

                if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando documentos em anexo. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.DocumentosAnexos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                    join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                    join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                    join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                    join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                    join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                    join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                    where pi.NuPedidoIndenizacao == NuProcesso
                                                                    && (gd.NuGrupoDocumento == 16 || gd.NuGrupoDocumento == 17)
                                                                    && (td.NuTipoDocumento != 1 && td.NuTipoDocumento != 7)
                                                                    && dc.DhExclusao == null
                                                                    && gd.NuTipoIndenizacao == sdctb008PedidoIndenizacao.NuTipoIndenizacao
                                                                    && gd.NuTipoParticipacao == null
                                                                    && tpa.NuTipoParticipacao == 1
                                                                    orderby td.NuTipoDocumento
                                                                    select new DocumentosInfoResponse()
                                                                    {
                                                                        QtPaginas = td.QtPaginas,
                                                                        DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                        DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                        DeUrlImagem = dc.DeUrlImagem,
                                                                        NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                        NuTipoDocumento = td.NuTipoDocumento,
                                                                        NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                        DeTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                        IdTipoDocumento = td.NuTipoDocumento,
                                                                        NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                        DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                        DeNomeArquivo = dc.DeNomeArquivo,
                                                                        NuDocumentoExigido = de.NuDocumentoExigido,
                                                                        Aprovado = dc.IcRejeitado,
                                                                        TipoParticipacao = tpa.NuTipoParticipacao,
                                                                        DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao

                                                                    }).ToListAsync();

                    foreach (var doc in dadosProcessoResponse.DocumentosAnexos)
                    {
                        doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                     where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                     select new Documento
                                                     {
                                                         IdGrupoDocumento = de.NuGrupoDocumento,
                                                         IdDocumentoExigido = de.NuDocumentoExigido,
                                                         IdTipoDocumento = de.NuTipoDocumento,
                                                         NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                         AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                         quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                     }).ToListAsync();
                    }
                }
                else
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando documentos em anexo. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    dadosProcessoResponse.DocumentosAnexos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                    join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                                    join de in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals de.NuDocumentoExigido
                                                                    join gd in _contextLeitura.Sdctb005GrupoDocumento on de.NuGrupoDocumento equals gd.NuGrupoDocumento
                                                                    join td in _contextLeitura.Sdctb006TipoDocumento on de.NuTipoDocumento equals td.NuTipoDocumento
                                                                    join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                                    join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                                    where pi.NuPedidoIndenizacao == NuProcesso
                                                                    && (td.NuTipoDocumento == 13 || td.NuTipoDocumento == 14 || td.NuTipoDocumento == 15 || td.NuTipoDocumento == 22 || td.NuTipoDocumento == 18 || td.NuTipoDocumento == 20)
                                                                    && dc.DhExclusao == null
                                                                    && gd.NuTipoIndenizacao == sdctb008PedidoIndenizacao.NuTipoIndenizacao
                                                                    orderby td.NuTipoDocumento
                                                                    select new DocumentosInfoResponse()
                                                                    {
                                                                        QtPaginas = td.QtPaginas,
                                                                        DeAbreviaturaTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                        DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                        DeUrlImagem = dc.DeUrlImagem,
                                                                        NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                        NuTipoDocumento = td.NuTipoDocumento,
                                                                        NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                        DeTipoDocumento = td.DeAbreviaturaTipoDocumento,
                                                                        IdTipoDocumento = td.NuTipoDocumento,
                                                                        NuGrupoDocumento = gd.NuGrupoDocumento,
                                                                        DeAbreviaturaGrupoDocumento = gd.DeAbreviaturaGrupoDocumento,
                                                                        DeNomeArquivo = dc.DeNomeArquivo,
                                                                        NuDocumentoExigido = de.NuDocumentoExigido,
                                                                        Aprovado = dc.IcRejeitado,
                                                                        TipoParticipacao = tpa.NuTipoParticipacao,
                                                                        DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao

                                                                    }).ToListAsync();

                    foreach (var doc in dadosProcessoResponse.DocumentosAnexos)
                    {
                        doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                     where de.NuGrupoDocumento == doc.NuGrupoDocumento
                                                     select new Documento
                                                     {
                                                         IdGrupoDocumento = de.NuGrupoDocumento,
                                                         IdDocumentoExigido = de.NuDocumentoExigido,
                                                         IdTipoDocumento = de.NuTipoDocumento,
                                                         NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                         AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                         quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                     }).ToListAsync();
                    }
                }



                _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados despesa medica. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                dadosProcessoResponse.DespesasMedicas = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                               join dc in _contextLeitura.Sdctb002DocumentoCapturado on pi.NuPedidoIndenizacao equals dc.NuPedidoIndenizacao
                                                               join dg in _contextLeitura.Sdctb003DocumentoExigido on dc.NuDocumentoExigido equals dg.NuDocumentoExigido
                                                               join tp in _contextLeitura.Sdctb006TipoDocumento on dg.NuTipoDocumento equals tp.NuTipoDocumento
                                                               join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                                               join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                                               where dc.NuPedidoIndenizacao == NuProcesso
                                                               && tp.NuTipoDocumento == 12
                                                               && dc.DhExclusao == null
                                                               select new DadosDespesasMedicasResponse
                                                               {
                                                                   NuDocumentoCapturado = dc.NuDocumentoCapturado,
                                                                   NuDocumentoExigido = dc.NuDocumentoExigido,
                                                                   NuDocumentoPendente = dc.NuDocumentoPendente,
                                                                   NuPagina = dc.NuPagina,
                                                                   DeUrlImagem = dc.DeUrlImagem,
                                                                   DeCaminhoBlob = dc.DeCaminhoBlob,
                                                                   NuGedamPedido = protocoloGedan.NuGedamPedido,
                                                                   DeNomeArquivo = dc.DeNomeArquivo,
                                                                   DhInclusao = dc.DhInclusao,
                                                                   DhExclusao = dc.DhExclusao,
                                                                   Aprovado = dc.IcRejeitado,
                                                                   TipoParticipacao = tpa.NuTipoParticipacao,
                                                                   DeTipoParticipacao = tpa.DeAbreviaTipoParticipacao
                                                               }).ToListAsync();
                foreach (var doc in dadosProcessoResponse.DespesasMedicas)
                {
                    doc.DocumentosGrupo = await (from de in _contextLeitura.Sdctb003DocumentoExigido
                                                 where de.NuDocumentoExigido == doc.NuDocumentoExigido
                                                 select new Documento
                                                 {
                                                     IdGrupoDocumento = de.NuGrupoDocumento,
                                                     IdDocumentoExigido = de.NuDocumentoExigido,
                                                     IdTipoDocumento = de.NuTipoDocumento,
                                                     NomeTipoDocumento = de.NuTipoDocumentoNavigation.DeTipoDocumento,
                                                     AbreviaturaTipoDocumento = de.NuTipoDocumentoNavigation.DeAbreviaturaTipoDocumento,
                                                     quantidadePaginas = de.NuTipoDocumentoNavigation.QtPaginas

                                                 }).ToListAsync();
                }

                foreach (var doc in dadosProcessoResponse.DespesasMedicas)
                {
                    doc.Emitentes = await (from e in _contextLeitura.Sdctb034Emitentes
                                           where e.NuDocumentoCapturado == doc.NuDocumentoCapturado
                                           select new DadosEmitentesResponse
                                           {
                                               NuEmitente = e.NuEmitente,
                                               NuDocumentoCapturado = e.NuDocumentoCapturado,
                                               NuIdentificacao = e.NuIdentificacao,
                                               NuRecibo = e.NuRecibo
                                           }).ToListAsync();

                }
                foreach (var doc in dadosProcessoResponse.DespesasMedicas)
                {
                    foreach (var emit in doc.Emitentes)
                    {
                        emit.ItensDespesa = await (from it in _contextLeitura.Sdctb035ItemDespesas
                                                   where it.NuEmitente == emit.NuEmitente
                                                   select new DadosItemDespesasResponse
                                                   {
                                                       NuItemDespesa = it.NuItemDespesa,
                                                       NuEmitente = it.NuEmitente,
                                                       NuTipoItemDespesa = it.NuTipoItemDespesa,
                                                       VrItemDespesa = it.VrItemDespesa
                                                   }).ToListAsync();
                    }
                }

                if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 3)
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando conclusão analise para DAMS + Invalidez. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                    dadosProcessoResponse.ConclusaoAnalise = new List<DadosConclusaoAnaliseResponse>();
                    DadosConclusaoAnaliseResponse dadosConclusaoAnaliseResponse = await (from svi in _contextLeitura.Sdctb046VigenciaIndenizacaos
                                                                                         join svi2 in _contextLeitura.Sdctb019ValorIndenizacao
                                                                                          on
                                                                                            new
                                                                                            {
                                                                                                key1 = svi.NuTipoIndenizacao,
                                                                                                key2 = sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                                                            }
                                                                                        equals
                                                                                            new
                                                                                            {
                                                                                                key1 = svi2.NuTipoIndenizacaoPgto,
                                                                                                key2 = svi2.NuPedidoIndenizacao
                                                                                            }
                                                                                         into tempVI
                                                                                         from vi in tempVI.DefaultIfEmpty()
                                                                                         where
                                                                                         svi.NuTipoIndenizacao == 1
                                                                                         && vi.DhExclusao == null
                                                                                         && svi.DtFimVigencia == null
                                                                                         select new DadosConclusaoAnaliseResponse
                                                                                         {
                                                                                             CabecalhoBloco = "Conclusão da análise",
                                                                                             NuValorIndenizacao = vi.NuValorIndenizacao,
                                                                                             NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao,
                                                                                             NuTipoIndenizacaoPgto = vi.NuTipoIndenizacaoPgto,
                                                                                             DeTipoIndenizacaoPgto = "Tenho comprovantes de despesas médicas e hospitalares e quero solicitar o reembolso dos gastos que tive com o meu tratamento devido ao acidente",
                                                                                             DeAbreviaturaTipoIndenizacaoPgto = "DESPESAS MÉDICAS E HOSPITALARES",
                                                                                             VrIndenizacao = vi.VrIndenizacao,
                                                                                             VrDisponivelIndenizacao = svi.VrLimiteIndenizacao,
                                                                                             DtPrevistaCredito = vi.DtPrevistaCredito,
                                                                                             DtEfetivaCredito = vi.DtEfetivaCredito,
                                                                                             DhExclusao = vi.DhExclusao
                                                                                         }).FirstAsync();

                    dadosProcessoResponse.ConclusaoAnalise.Add(dadosConclusaoAnaliseResponse);
                    dadosConclusaoAnaliseResponse = await (from svi in _contextLeitura.Sdctb046VigenciaIndenizacaos
                                                           join svi2 in _contextLeitura.Sdctb019ValorIndenizacao
                                                           on
                                                               new
                                                               {
                                                                   key1 = svi.NuTipoIndenizacao,
                                                                   key2 = sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                               }
                                                           equals
                                                               new
                                                               {
                                                                   key1 = svi2.NuTipoIndenizacaoPgto,
                                                                   key2 = svi2.NuPedidoIndenizacao
                                                               }
                                                           into tempVI
                                                           from vi in tempVI.DefaultIfEmpty()
                                                           where
                                                           svi.NuTipoIndenizacao == 2
                                                           && vi.DhExclusao == null
                                                           && svi.DtFimVigencia == null
                                                           select new DadosConclusaoAnaliseResponse
                                                           {
                                                               CabecalhoBloco = "Conclusão da análise",
                                                               NuValorIndenizacao = vi.NuValorIndenizacao,
                                                               NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao,
                                                               NuTipoIndenizacaoPgto = vi.NuTipoIndenizacaoPgto,
                                                               DeTipoIndenizacaoPgto = "Tive lesão que resultou em invalidez permanente parcial ou total por causa do acidente de trânsito",
                                                               DeAbreviaturaTipoIndenizacaoPgto = "INVALIDEZ PERMANENTE",
                                                               VrIndenizacao = vi.VrIndenizacao,
                                                               VrDisponivelIndenizacao = svi.VrLimiteIndenizacao,
                                                               DtPrevistaCredito = vi.DtPrevistaCredito,
                                                               DtEfetivaCredito = vi.DtEfetivaCredito,
                                                               DhExclusao = vi.DhExclusao
                                                           }).FirstAsync();
                    dadosProcessoResponse.ConclusaoAnalise.Add(dadosConclusaoAnaliseResponse);

                    if (sdctb008PedidoIndenizacao.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 5 && _contextLeitura.Sdctb019ValorIndenizacao.Where(c => c.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao)
                        .Any(x => x.DeObservacao != null))
                    {

                        dadosProcessoResponse.Observacao = await (from svi2 in _contextLeitura.Sdctb019ValorIndenizacao
                                                                  where svi2.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao
                                                                  && svi2.DeObservacao != null
                                                                  select new ObservacaoIndenizacaoResponse
                                                                  {
                                                                      CabecalhoBloco = "Observação",
                                                                      DeObservacao = svi2.DeObservacao
                                                                  }).DefaultIfEmpty().FirstAsync();


                    }

                }
                else if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando conclusao analise para Morte. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    var valorIndenizacaoPagaAnterior = await (from svi in _contextLeitura.Sdctb019ValorIndenizacao
                                                              join spi in _contextLeitura.Sdctb008PedidoIndenizacao on svi.NuPedidoIndenizacao equals spi.NuPedidoIndenizacao
                                                              join p in _contextLeitura.Sdctb015Participacao on spi.NuPedidoIndenizacao equals p.NuPedidoIndenizacao
                                                              join sp in _contextLeitura.Sdctb009Pessoa on p.NuPessoaParticipante equals sp.NuPessoa
                                                              where p.NuTipoParticipacao == 1
                                                              && spi.NuTipoIndenizacao == 4
                                                              && sp.NuCpf == dadosProcessoResponse.DadosPessoaisVitima.Cpf
                                                              && svi.DhExclusao == null
                                                              && spi.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 5
                                                              select new
                                                              {
                                                                  svi.VrIndenizacao
                                                              }).ToListAsync();

                    dadosProcessoResponse.ConclusaoAnalise = await (from spi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                    join sti in _contextLeitura.Sdctb007TipoIndenizacao on spi.NuTipoIndenizacao equals sti.NuTipoIndenizacao
                                                                    join svi in _contextLeitura.Sdctb046VigenciaIndenizacaos on sti.NuTipoIndenizacao equals svi.NuTipoIndenizacao
                                                                    join svi2 in _contextLeitura.Sdctb019ValorIndenizacao on spi.NuPedidoIndenizacao equals svi2.NuPedidoIndenizacao
                                                                    into tempVI
                                                                    from vi in tempVI.DefaultIfEmpty()
                                                                    where spi.NuPedidoIndenizacao == NuProcesso
                                                                         && vi.DhExclusao == null
                                                                         && svi.DtFimVigencia == null
                                                                         && svi.NuTipoIndenizacao == sdctb008PedidoIndenizacao.NuTipoIndenizacao
                                                                    select new DadosConclusaoAnaliseResponse
                                                                    {
                                                                        CabecalhoBloco = "Conclusão da análise",
                                                                        NuValorIndenizacao = vi.NuValorIndenizacao,
                                                                        NuPedidoIndenizacao = spi.NuPedidoIndenizacao,
                                                                        NuTipoIndenizacaoPgto = vi.NuTipoIndenizacaoPgto,
                                                                        DeTipoIndenizacaoPgto = sti.DeTipoIndenizacao,
                                                                        DeAbreviaturaTipoIndenizacaoPgto = sti.DeAbreviaturaTipoIndenizaca,
                                                                        VrIndenizacao = vi.VrIndenizacao,
                                                                        VrDisponivelIndenizacao = Convert.ToDecimal(svi.VrLimiteIndenizacao) - valorIndenizacaoPagaAnterior.Select(x => x.VrIndenizacao).Sum(),
                                                                        DtPrevistaCredito = vi.DtPrevistaCredito,
                                                                        DtEfetivaCredito = vi.DtEfetivaCredito,
                                                                        DhExclusao = vi.DhExclusao
                                                                    }).ToListAsync();


                    bool dependentes = await (from sdh in _contextLeitura.Sdctb023DeclaraHerdeiro
                                              where sdh.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao && sdh.DhExclusao == null
                                              select sdh).AnyAsync();
                    if (dependentes)
                    {
                        _logger.LogInformation($"CarregaInformacaoProcesso - Carregando dados dependentes. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");

                        dadosProcessoResponse.DadosDependentes = new DadosDependentesResponse();
                        dadosProcessoResponse.DadosDependentes.CabecalhoBloco = "Dados dependentes";
                        dadosProcessoResponse.DadosDependentes.Dependentes = await (from sdh in _contextLeitura.Sdctb023DeclaraHerdeiro
                                                                                    join sp in _contextLeitura.Sdctb009Pessoa on sdh.NuPessoaHerdeiro equals sp.NuPessoa
                                                                                    join sp2 in _contextLeitura.Sdctb022Parentesco on sdh.NuParentesco equals sp2.NuParentesco
                                                                                    where sdh.NuPedidoIndenizacao == sdctb008PedidoIndenizacao.NuPedidoIndenizacao && sdh.DhExclusao == null
                                                                                    select new DependentesResponse
                                                                                    {
                                                                                        NuCpf = sp.NuCpf,
                                                                                        NoPessoa = sp.NoPessoa,
                                                                                        NuParentesco = sp2.NuParentesco,
                                                                                        NoParentesco = sp2.NoParentesco,
                                                                                        NuDeclaracaoHerdeiro = sdh.NuDeclaracaoHerdeiro,
                                                                                        NuPessoaHerdeiro = sdh.NuPessoaHerdeiro

                                                                                    }).ToListAsync();
                    }
                }
                else
                {
                    _logger.LogInformation($"CarregaInformacaoProcesso - Carregando conclusao analise. - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa}");
                    dadosProcessoResponse.ConclusaoAnalise = await (from spi in _contextLeitura.Sdctb008PedidoIndenizacao
                                                                    join sti in _contextLeitura.Sdctb007TipoIndenizacao on spi.NuTipoIndenizacao equals sti.NuTipoIndenizacao
                                                                    join svi in _contextLeitura.Sdctb046VigenciaIndenizacaos on sti.NuTipoIndenizacao equals svi.NuTipoIndenizacao
                                                                    join svi2 in _contextLeitura.Sdctb019ValorIndenizacao on spi.NuPedidoIndenizacao equals svi2.NuPedidoIndenizacao
                                                                    into tempVI
                                                                    from vi in tempVI.DefaultIfEmpty()
                                                                    where spi.NuPedidoIndenizacao == NuProcesso
                                                                         && vi.DhExclusao == null
                                                                         && svi.DtFimVigencia == null
                                                                         && svi.NuTipoIndenizacao == sdctb008PedidoIndenizacao.NuTipoIndenizacao
                                                                    select new DadosConclusaoAnaliseResponse
                                                                    {
                                                                        CabecalhoBloco = "Conclusão da análise",
                                                                        NuValorIndenizacao = vi.NuValorIndenizacao,
                                                                        NuPedidoIndenizacao = spi.NuPedidoIndenizacao,
                                                                        NuTipoIndenizacaoPgto = vi.NuTipoIndenizacaoPgto,
                                                                        DeTipoIndenizacaoPgto = sti.DeTipoIndenizacao,
                                                                        DeAbreviaturaTipoIndenizacaoPgto = sti.DeAbreviaturaTipoIndenizaca,
                                                                        VrIndenizacao = vi.VrIndenizacao,
                                                                        VrDisponivelIndenizacao = svi.VrLimiteIndenizacao,
                                                                        DtPrevistaCredito = vi.DtPrevistaCredito,
                                                                        DtEfetivaCredito = vi.DtEfetivaCredito,
                                                                        DhExclusao = vi.DhExclusao
                                                                    }).ToListAsync();
                }

                dadosProcessoResponse.TipoIndeferimento = await (from tid in _contextLeitura.Sdctb044TipoIndeferimentos
                                                                 select new TipoIndeferimentoResponse
                                                                 {
                                                                     NuTipoIndeferimento = tid.NuTipoIndeferimento,
                                                                     DeTipoIndeferimento = tid.DeTipoIndeferimento

                                                                 }).ToListAsync();

                dadosProcessoResponse.TipoMotivoIndeferimento = await (from tid in _contextLeitura.Sdctb045MotivoIndeferimentos
                                                                       select new MotivoIndeferimentoResponse
                                                                       {
                                                                           NuMotivoIndeferimento = tid.NuMotivoIndeferimento,
                                                                           NuTipoIndeferimento = tid.NuTipoIndeferimento,
                                                                           DeMotivoIndeferimento = tid.DeMotivoIndeferimento

                                                                       }).ToListAsync();

                dadosProcessoResponse.TipoItemDespesa = await (from tpIte in _contextLeitura.Sdctb036TipoItemDespesas
                                                               select new TipoItemDespesaResponse()
                                                               {
                                                                   DeTipoItemDespesa = tpIte.DeTipoItemDespesa,
                                                                   NuTipoItemDespesa = tpIte.NuTipoItemDespesa
                                                               }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"CarregaInformacaoProcesso - error {e.Message}");
                throw new(e.Message);
            }
            _logger.LogInformation($"CarregaInformacaoProcesso - Fim - Processo {NuProcesso},CPF {nuCpf}, Matricula{CoMatriculaCaixa} carregado com sucesso");
            return dadosProcessoResponse;
        }

        //TODO: Busca Base Historica Oriunda da Líder
        public async Task<ProcessoPaginadoHistorico> ListaHistoricoPorCpf(long? NuCpf, ProcessoPaginacaoHistorico paginacao)
        {
            _logger.LogInformation($"ListaHistoricoPorCpf - incio - CPF {NuCpf}");


            BaseHistoricoFiltro filtro = new BaseHistoricoFiltro();
            filtro.NuCpf = NuCpf;

            IQueryable<BaseHistoricoResponse> baseHistoricoResponse = null;
            try
            {
                baseHistoricoResponse = (from ps in _contextLeituraHistorico.Dpvtb013PrepSinistro
                                         join pp in _contextLeituraHistorico.Dpvtb014PrepPessoa
                                         on ps.IdSinistro equals pp.IdSinistro
                                         select new BaseHistoricoResponse
                                         {
                                             DataPedido = ps.DtCriacaoAvisoSinistro,
                                             DataAcidente = ps.DtOcorrenciaSinistro,
                                             CpfVitima = pp.NuCpfPessoa,
                                             CpfBeneficiario = pp.NuCpfPessoa,
                                             Descricao = ps.DsNaturezaSinistro,
                                             Status = ps.NmSituacaoAtualSinistro,
                                             DataSituacao = ps.DtUltimoMovimentoSinistro,
                                             Motivo = ps.DsMotivoNegativa,
                                             Valor = pp.VlPagoPessoa
                                         }
               ).AplicaFiltro(filtro);



            }
            catch (Exception e)
            {
                _logger.LogError($"ListaHistoricoPorCpf - erro", e.Message);
                throw new(e.Message);
            }
            //_logger.LogInformation($"ListaHistoricoPorCpf - resultado: {baseHistoricoResponse.ToJson()}");

            return ProcessoPaginadoHistorico.From(paginacao, baseHistoricoResponse);
        }

        public async Task ConcluirAnalise(SituacaoPedidoRequest situacaoPedidoRequest)
        {
            _logger.LogInformation($"ConcluirAnalise - incio - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");

            Sdctb008PedidoIndenizacao sdctb008PedidoIndenizacao = await _contextLeitura.Sdctb008PedidoIndenizacao
                        .Where(x => x.NuPedidoIndenizacao == situacaoPedidoRequest.NuPedidoIndenizacao).FirstAsync();
            //regra 1
            //Caso esteja deferindo o pedido o valor da indenização não pode ser igual a zero enem superior ao disponivel para idenização
            if ((short)Enums.TipoSituacaoPedido.SolicitacaoDeferida == situacaoPedidoRequest.NuTipoSituacaoPedido)
            {
                _logger.LogInformation($"ConcluirAnalise - Verificações Regra 1 para teto de valores e indenizações zeradas. - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");

                decimal ValorAnterior = -1;
                foreach (var indenizacao in situacaoPedidoRequest.indenizacoes)
                {
                    if (!sdctb008PedidoIndenizacao.NuTipoIndenizacao.Equals(3))
                    {
                        if (indenizacao.vrIndenizacao == 0)
                            throw new ArgumentException($"Valor da indenização não pode ser igual a zero.", "400");
                    }
                    else
                    {
                        if (indenizacao.vrIndenizacao.Equals(0) && ValorAnterior.Equals(0))
                        {
                            throw new ArgumentException($"Não é possivel salvar indenização igual 0 sem descrição.", "400");
                        }
                        else
                        {
                            try
                            {
                                if (indenizacao.nuValorIndenizacao != null)
                                {
                                    Sdctb019ValorIndenizacao sdctb019ValorIndenizacao = await _contextLeitura.Sdctb019ValorIndenizacao
                                    .Where(x => x.NuValorIndenizacao == indenizacao.nuValorIndenizacao).FirstAsync();

                                    ValorAnterior = sdctb019ValorIndenizacao.VrIndenizacao;

                                    if (indenizacao.vrIndenizacao.Equals(0))
                                    {
                                        sdctb019ValorIndenizacao.DeObservacao = situacaoPedidoRequest.DeObservacao;
                                        _contextEscrita.Sdctb019ValorIndenizacao.Update(sdctb019ValorIndenizacao);
                                        if (await _contextEscrita.SaveChangesAsync() == 0)
                                            throw new Exception($"Erro ao tentar gravar observação indenização. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");

                                    }
                                }
                                else
                                {
                                    Sdctb019ValorIndenizacao sdctb019ValorIndenizacao = await _contextLeitura.Sdctb019ValorIndenizacao
                                    .Where(x => x.NuTipoIndenizacaoPgto == indenizacao.nuTipoIndenizacaoPgto
                                            && x.NuPedidoIndenizacao == situacaoPedidoRequest.NuPedidoIndenizacao).FirstAsync();

                                    ValorAnterior = sdctb019ValorIndenizacao.VrIndenizacao;

                                    if (indenizacao.vrIndenizacao.Equals(0))
                                    {
                                        sdctb019ValorIndenizacao.DeObservacao = situacaoPedidoRequest.DeObservacao;
                                        _contextEscrita.Sdctb019ValorIndenizacao.Update(sdctb019ValorIndenizacao);
                                        if (await _contextEscrita.SaveChangesAsync() == 0)
                                            throw new Exception($"Erro ao tentar gravar observação indenização. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");

                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                            }
                        }

                    }
                    _logger.LogInformation($"ConcluirAnalise - VerificarValorTetoIndenizacao - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");
                    await VerificarValorTetoIndenizacao((TipoIndenizacao)indenizacao.nuTipoIndenizacaoPgto, indenizacao.vrIndenizacao);
                }


            }

            _logger.LogInformation($"ConcluirAnalise - SetarDataExclusaoSituacoesPedido - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");
            await SetarDataExclusaoSituacoesPedido(situacaoPedidoRequest.NuPedidoIndenizacao);

            _logger.LogInformation($"ConcluirAnalise - Criando Nova situação para o pedido - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");
            Sdctb010SituacaoPedido situacaoPedido = new Sdctb010SituacaoPedido();

            situacaoPedido.CoMatriculaCaixa = situacaoPedidoRequest.CoMatriculaCaixa;
            situacaoPedido.NuPedidoIndenizacao = situacaoPedidoRequest.NuPedidoIndenizacao;
            situacaoPedido.NuMotivoIndeferimento = situacaoPedidoRequest.NuMotivoIndeferimento;
            situacaoPedido.NuTipoSituacaoPedido = situacaoPedidoRequest.NuTipoSituacaoPedido;
            situacaoPedido.DhSituacao = DateTime.Now.AddHours(-3);

            _contextEscrita.Sdctb010SituacaoPedido.Add(situacaoPedido);

            if (await _contextEscrita.SaveChangesAsync() == 0)
                throw new Exception($"Erro ao tentar gravar situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");

            long indSituacaoPedido = _contextEscrita.Sdctb010SituacaoPedido.Max(x => x.NuSituacaoPedido);

            _logger.LogInformation($"ConcluirAnalise - SetarUltimaSitucaoDoPedido - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");
            await SetarUltimaSitucaoDoPedido(situacaoPedidoRequest, indSituacaoPedido);

            if (situacaoPedidoRequest.ListaSituacaoMotivoRequest.Count > 0)
            {
                _logger.LogInformation($"ConcluirAnalise - Novo motivo para a situação - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");

                foreach (var motivo in situacaoPedidoRequest.ListaSituacaoMotivoRequest)
                {
                    Sdctb018SituacaoMotivo situacaoMotivo = new Sdctb018SituacaoMotivo();
                    situacaoMotivo.DhSituacao = DateTime.Now.AddHours(-3);
                    situacaoMotivo.NuTipoMotivoSituacao = motivo.NuTipoMotivoSituacao;
                    situacaoMotivo.NuSituacaoPedido = indSituacaoPedido;

                    _contextEscrita.Sdctb018SituacaoMotivo.Add(situacaoMotivo);

                    if (await _contextEscrita.SaveChangesAsync() == 0)
                        throw new Exception($"Erro ao tentar gravar situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");
                }

            }
            else if (situacaoPedidoRequest.NuTipoSituacaoPedido.Equals(6))
            {
                _logger.LogInformation($"ConcluirAnalise - Novo motivo para a situação tipo 6 - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");

                Sdctb018SituacaoMotivo situacaoMotivo = new Sdctb018SituacaoMotivo();
                situacaoMotivo.DhSituacao = DateTime.Now.AddHours(-3);
                situacaoMotivo.NuTipoMotivoSituacao = 10;
                situacaoMotivo.NuSituacaoPedido = indSituacaoPedido;

                _contextEscrita.Sdctb018SituacaoMotivo.Add(situacaoMotivo);

                if (await _contextEscrita.SaveChangesAsync() == 0)
                    throw new Exception($"Erro ao tentar gravar situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");
            }





            //Caso seja pendente devo setar as pendencias
            if (situacaoPedidoRequest.NuTipoSituacaoPedido == 4)// se for pendente
            {
                _logger.LogInformation($"ConcluirAnalise - Incluindo pendencias de documentos - Pedido {situacaoPedidoRequest.NuPedidoIndenizacao}");

                int indPendente = 0;
                foreach (var doc in situacaoPedidoRequest.ListaDocumentoExigidoRequest)
                {
                    var listaCapturados = await _contextLeitura.Sdctb002DocumentoCapturado
                        .Where(x => x.NuDocumentoExigido == doc.NuDocumentoExigido
                        && x.NuPedidoIndenizacao == situacaoPedidoRequest.NuPedidoIndenizacao && x.NuDocumentoCapturado == doc.NuDocumentoCapturado).ToListAsync();

                    Sdctb030DocumentoPendente documentoPendente = new Sdctb030DocumentoPendente();
                    documentoPendente.NuDocumentoExigido = doc.NuDocumentoExigido;
                    documentoPendente.NuSituacaoPedido = indSituacaoPedido;


                    _contextEscrita.Sdctb030DocumentoPendente.Add(documentoPendente);
                    if (await _contextEscrita.SaveChangesAsync() == 0)
                        throw new Exception($"Erro ao tentar gravar situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");

                    if (listaCapturados.Count > 0)
                    {
                        long indPendencia = situacaoPedido.Sdctb030DocumentoPendentes.Max(x => x.NuDocumentoPendente);
                        foreach (var capturado in listaCapturados)
                        {
                            capturado.NuDocumentoPendente = indPendencia;
                            // seta null para impedir que tente atualizar a tabela 008 que ja esta sendo atualizada em metodo anterior
                            capturado.NuPedidoIndenizacaoNavigation = null;
                            _contextEscrita.Sdctb002DocumentoCapturado.Update(capturado);

                        }
                        if (await _contextEscrita.SaveChangesAsync() == 0)
                            throw new Exception($"Erro ao tentar gravar situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");
                    }
                }
            }

        }

        private async Task SetarUltimaSitucaoDoPedido(SituacaoPedidoRequest situacaoPedidoRequest, long indSituacaoPedido)
        {
            var pedido = await _contextEscrita.Sdctb008PedidoIndenizacao
                 .FirstOrDefaultAsync(x => x.NuPedidoIndenizacao == situacaoPedidoRequest.NuPedidoIndenizacao);
            pedido.NuSituacaoPedido = indSituacaoPedido;
            _contextEscrita.Sdctb008PedidoIndenizacao.Update(pedido);

            if (await _contextEscrita.SaveChangesAsync() == 0)
                throw new Exception($"Erro ao tentar setar última situação do pedido. Id: {situacaoPedidoRequest.NuPedidoIndenizacao}");
        }

        private async Task SetarDataExclusaoSituacoesPedido(long nuPedidoIndenizacao)
        {
            bool gravar = false;
            var listaSituacoesAnteriores = _contextEscrita.Sdctb010SituacaoPedido
                 .Where(x => x.NuPedidoIndenizacao == nuPedidoIndenizacao).ToList();
            foreach (var situacao in listaSituacoesAnteriores)
            {
                if (situacao.DhExclusao == null)
                {
                    gravar = true;
                    situacao.DhExclusao = DateTime.Now.AddHours(-3);
                }
                await SetarDataExclusaoMotivosSituacoesPedido(situacao.NuSituacaoPedido);
            }
            if (gravar)
            {
                _contextEscrita.Sdctb010SituacaoPedido.UpdateRange(listaSituacoesAnteriores);
                if (await _contextEscrita.SaveChangesAsync() == 0)
                    throw new Exception($"Erro ao tentar gravar data exclusão situação do pedido. Id: {nuPedidoIndenizacao}");
            }
        }
        private async Task SetarDataExclusaoMotivosSituacoesPedido(long nuSitucaoPedido)
        {
            bool gravar = false;
            var listaMotivosAnteriores = _contextEscrita.Sdctb018SituacaoMotivo
                 .Where(x => x.NuSituacaoPedido == nuSitucaoPedido).ToList();
            foreach (var motivo in listaMotivosAnteriores)
            {
                if (motivo.DhExclusao == null)
                {
                    gravar = true;
                    motivo.DhExclusao = DateTime.Now.AddHours(-3);
                }
            }
            if (gravar)
            {
                _contextEscrita.Sdctb018SituacaoMotivo.UpdateRange(listaMotivosAnteriores);
                if (await _contextEscrita.SaveChangesAsync() == 0)
                    throw new Exception($"Erro ao tentar gravar data de exclusão dos motivos da situação do pedido. Id: {nuSitucaoPedido}");
            }
        }
        public async Task<DadosProcessoResponse> SalvarAnalise(SituacaoMomentoPedidoRequest situacaoMomentoPedido)
        {
            _logger.LogInformation($"SalvarAnalise - incio ");

            foreach (var indenizacao in situacaoMomentoPedido.indenizacoes)
            {
                _logger.LogInformation($"SalvarAnalise - Verificando Indenizações - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");

                await VerificarValorTetoIndenizacao((TipoIndenizacao)indenizacao.nuTipoIndenizacaoPgto, indenizacao.vrIndenizacao);
                if (indenizacao.nuTipoIndenizacaoPgto == 1 && situacaoMomentoPedido.IcConcluirAnalise)
                {
                    await VerificaEmitenteEValores(situacaoMomentoPedido, indenizacao);
                }
            }

            try
            {
                await ResilientTransaction.New(_contextEscrita).ExecuteAsync(async () =>
                {

                    //atualizando status dos documentos
                    List<Sdctb002DocumentoCapturado> sdctb002DocumentoCapturado = await _contextEscrita.Sdctb002DocumentoCapturado
                                                                                    .Include(x => x.NuDocumentoExigidoNavigation)
                                                                                    .Where(s => s.NuPedidoIndenizacao == situacaoMomentoPedido.nuPedidoIndenizacao).ToListAsync();

                    _logger.LogInformation($"SalvarAnalise - Atualizando documentos IcRejeitado - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");

                    foreach (var doc in situacaoMomentoPedido.documentos)
                    {

                        foreach (var capturado in sdctb002DocumentoCapturado)
                        {
                            if (capturado.NuDocumentoCapturado == doc.nuDocumentoCapturado)
                            {
                                capturado.IcRejeitado = doc.fgDocumentoAprovado;
                                break;
                            }
                        }
                    }
                    _contextEscrita.UpdateRange(sdctb002DocumentoCapturado);
                    await _contextEscrita.SaveChangesAsync();



                    //salvando indenização
                    _logger.LogInformation($"SalvarAnalise - salvando indenização - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");
                    foreach (var indenizacao in situacaoMomentoPedido.indenizacoes)
                    {
                        if (indenizacao.nuValorIndenizacao == null && !(indenizacao.nuTipoIndenizacaoPgto == 0))
                        {
                            Sdctb019ValorIndenizacao sdctb019ValorIndenizacao = new Sdctb019ValorIndenizacao()
                            {
                                NuPedidoIndenizacao = situacaoMomentoPedido.nuPedidoIndenizacao,
                                NuTipoIndenizacaoPgto = indenizacao.nuTipoIndenizacaoPgto,
                                VrIndenizacao = indenizacao.vrIndenizacao,
                                DtPrevistaCredito = DateTime.Now.AddDays(30),
                            };

                            _contextEscrita.Add(sdctb019ValorIndenizacao);
                            await _contextEscrita.SaveChangesAsync();
                        }
                        else if (indenizacao.nuValorIndenizacao.HasValue)
                        {
                            try
                            {
                                Sdctb019ValorIndenizacao sdctb019ValorIndenizacao = await _contextLeitura.Sdctb019ValorIndenizacao
                               .Where(x => x.NuValorIndenizacao == indenizacao.nuValorIndenizacao).FirstAsync();

                                sdctb019ValorIndenizacao.VrIndenizacao = indenizacao.vrIndenizacao;
                                _contextEscrita.Update(sdctb019ValorIndenizacao);
                                await _contextEscrita.SaveChangesAsync();

                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                            }
                        }
                    }

                    //salvando dados pessoais
                    _logger.LogInformation($"SalvarAnalise - salvando dados pessoais - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");
                    foreach (var campo in situacaoMomentoPedido.dadosPessoais.camposDocumento)
                    {
                        if (campo.nuItemDocumentoConteudo == null && !(campo.conteudo == null))
                        {
                            Sdctb049ItemDocumentoConteudo sdctb049ItemDocumentoConteudo = new Sdctb049ItemDocumentoConteudo()
                            {
                                NuDocumentoCapturado = situacaoMomentoPedido.dadosPessoais.nuDocumentoCapturado,
                                NuItemDocumento = campo.nuItemConteudo,
                                DeConteudo = campo.conteudo,

                            };
                            _contextEscrita.Add(sdctb049ItemDocumentoConteudo);
                            await _contextEscrita.SaveChangesAsync();
                        }
                        else if (!campo.nuItemDocumentoConteudo.Equals(null) && !(campo.conteudo == null))
                        {
                            Sdctb049ItemDocumentoConteudo sdctb049ItemDocumentoConteudo = await _contextEscrita.Sdctb049ItemDocumentoConteudos
                            .Where(s => s.NuItemDocumentoConteudo == campo.nuItemDocumentoConteudo).FirstAsync();
                            sdctb049ItemDocumentoConteudo.DeConteudo = campo.conteudo;
                            _contextEscrita.Update(sdctb049ItemDocumentoConteudo);
                            await _contextEscrita.SaveChangesAsync();
                        }
                    };

                    // se indenização tipo Morte, atualiza os dados da vitima
                    if (situacaoMomentoPedido.indenizacoes.FirstOrDefault().nuTipoIndenizacaoPgto == 4)
                    {
                        _logger.LogInformation($"SalvarAnalise - atualiza os dados da vitima para indenizacao Morte - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");

                        foreach (var campo in situacaoMomentoPedido.dadosPessoaisVitima.camposDocumento)
                        {
                            if (campo.nuItemDocumentoConteudo == null && !(campo.conteudo == null))
                            {
                                Sdctb049ItemDocumentoConteudo sdctb049ItemDocumentoConteudo = new Sdctb049ItemDocumentoConteudo()
                                {
                                    NuDocumentoCapturado = situacaoMomentoPedido.dadosPessoaisVitima.nuDocumentoCapturado,
                                    NuItemDocumento = campo.nuItemConteudo,
                                    DeConteudo = campo.conteudo,

                                };
                                _contextEscrita.Add(sdctb049ItemDocumentoConteudo);
                                await _contextEscrita.SaveChangesAsync();
                            }
                            else if (!campo.nuItemDocumentoConteudo.Equals(null) && !(campo.conteudo == null))
                            {
                                Sdctb049ItemDocumentoConteudo sdctb049ItemDocumentoConteudo = await _contextEscrita.Sdctb049ItemDocumentoConteudos
                                .Where(s => s.NuItemDocumentoConteudo == campo.nuItemDocumentoConteudo).FirstAsync();
                                sdctb049ItemDocumentoConteudo.DeConteudo = campo.conteudo;
                                _contextEscrita.Update(sdctb049ItemDocumentoConteudo);
                                await _contextEscrita.SaveChangesAsync();
                            }
                        };
                    }


                    //cadastrar emissor e itens despesa
                    _logger.LogInformation($"SalvarAnalise - cadastrar emissor e itens despesa - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");
                    foreach (var emitente in situacaoMomentoPedido.emitentes)
                    {
                        //exclusão Emitente
                        if (emitente.IcExcluido)
                        {
                            await ExcluirItensDispesa(emitente.nuEmitente);

                            var emitenteASerExcluido = await _contextEscrita
                            .Sdctb034Emitentes.FirstOrDefaultAsync(x => x.NuEmitente == emitente.nuEmitente);
                            if (emitenteASerExcluido != null)
                            {
                                _contextEscrita.Sdctb034Emitentes.Remove(emitenteASerExcluido);
                                await _contextEscrita.SaveChangesAsync();
                            }
                            continue;
                        }

                        Sdctb034Emitente sdctb034Emitente = new Sdctb034Emitente();
                        if (emitente.nuEmitente == null)
                        {
                            sdctb034Emitente.NuDocumentoCapturado = emitente.nuDocumentoCapturado;
                            sdctb034Emitente.NuIdentificacao = emitente.nuIdentificacao;
                            sdctb034Emitente.NuRecibo = emitente.nuRecibo;
                            _contextEscrita.Add(sdctb034Emitente);
                            await _contextEscrita.SaveChangesAsync();

                            sdctb034Emitente.NuEmitente = _contextEscrita.Sdctb034Emitentes.Max(x => x.NuEmitente);//setando nova nova chave
                        }
                        else
                        {
                            sdctb034Emitente = await _contextEscrita.Sdctb034Emitentes
                            .Where(s => s.NuEmitente == emitente.nuEmitente).FirstAsync();
                            sdctb034Emitente.NuIdentificacao = emitente.nuIdentificacao;
                            sdctb034Emitente.NuRecibo = emitente.nuRecibo;
                            _contextEscrita.Update(sdctb034Emitente);

                            //await ExcluirItensDispesa(sdctb034Emitente.NuEmitente);
                        }



                        foreach (var despesa in emitente.itensDespesa)
                        {
                            if (despesa.IcExcluido)
                            {
                                var item = await _contextLeitura.Sdctb035ItemDespesas.FirstOrDefaultAsync(x => x.NuItemDespesa == despesa.nuItemDespesa);
                                if (item != null)
                                    _contextEscrita.Sdctb035ItemDespesas.Remove(item);
                                continue;
                            }

                            if (despesa.nuItemDespesa == null)
                            {
                                Sdctb035ItemDespesa sdctb035ItemDespesa = new Sdctb035ItemDespesa()
                                {
                                    NuEmitente = sdctb034Emitente.NuEmitente,
                                    NuTipoItemDespesa = despesa.nuTipoItemDespesa,
                                    VrItemDespesa = despesa.vrItemDespesa,
                                };
                                _contextEscrita.Sdctb035ItemDespesas.Add(sdctb035ItemDespesa);
                            }
                            else
                            {
                                Sdctb035ItemDespesa item = await _contextLeitura.Sdctb035ItemDespesas.FirstOrDefaultAsync(x => x.NuItemDespesa == despesa.nuItemDespesa);
                                if (item != null)
                                {
                                    item.NuEmitente = sdctb034Emitente.NuEmitente;
                                    item.NuTipoItemDespesa = despesa.nuTipoItemDespesa;
                                    item.VrItemDespesa = despesa.vrItemDespesa;
                                    _contextEscrita.Sdctb035ItemDespesas.Update(item);
                                }
                            }
                        }
                        await _contextEscrita.SaveChangesAsync();
                    }


                    if (situacaoMomentoPedido.necessitaPericia.HasValue)
                    {
                        _logger.LogInformation($"SalvarAnalise - Vareficando Envio/Retirada de pericia medica - Pedido {situacaoMomentoPedido.nuPedidoIndenizacao}");
                        // Busca o pedido desde que seja originado do mesmo solicitante
                        Sdctb008PedidoIndenizacao pedidoIndenizacao = await _contextEscrita.Sdctb008PedidoIndenizacao
                                                                        .Include(c => c.NuSituacaoPedidoNavigation)
                                                                        .ThenInclude(c => c.Sdctb018SituacaoMotivos)
                                                                        .Where(s => s.NuPedidoIndenizacao == situacaoMomentoPedido.nuPedidoIndenizacao)
                                                                        .FirstOrDefaultAsync();

                        if (situacaoMomentoPedido.necessitaPericia == true && !pedidoIndenizacao.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.OrderByDescending(c => c.DhSituacao).FirstOrDefault().NuTipoMotivoSituacao.Equals(8))
                        {
                            Sdctb010SituacaoPedido CoMatricula = await _contextLeitura.Sdctb010SituacaoPedido
                           .Where(y => y.NuPedidoIndenizacao == situacaoMomentoPedido.nuPedidoIndenizacao && y.DhExclusao == null).FirstAsync();

                            // setando data fim situação pedido anterior
                            await SetarDataExclusaoSituacoesPedido(situacaoMomentoPedido.nuPedidoIndenizacao);
                            // cadastrar situacao pedido

                            _logger.LogInformation($"AtualizaStatusProcesso - Salvar nova situação do pedido - Processo {situacaoMomentoPedido.nuPedidoIndenizacao}");
                            await _contextEscrita.SaveChangesAsync();

                            // Salvar nova situação do pedido
                            Sdctb010SituacaoPedido sdctb010SituacaoPedido = new Sdctb010SituacaoPedido()
                            {
                                NuTipoSituacaoPedido = 3,
                                DhSituacao = DateTime.Now.AddHours(-3),
                                NuPedidoIndenizacao = situacaoMomentoPedido.nuPedidoIndenizacao,
                                CoMatriculaCaixa = CoMatricula.CoMatriculaCaixa,

                            };

                            _contextEscrita.Add(sdctb010SituacaoPedido);
                            await _contextEscrita.SaveChangesAsync();

                            // Busca o ID que acabou de ser incluído
                            pedidoIndenizacao.NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido;

                            _logger.LogInformation($"AtualizaStatusProcesso - Atualiza o pedido com a nova situação - Processo {situacaoMomentoPedido.nuPedidoIndenizacao}");
                            // Atualiza o pedido com a nova situação
                            _contextEscrita.Update(pedidoIndenizacao);
                            await _contextEscrita.SaveChangesAsync();

                            // Atualiza o tipo do motivo da situação
                            Sdctb018SituacaoMotivo sdctb018SituacaoMotivo = new Sdctb018SituacaoMotivo()
                            {
                                NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido,
                                NuTipoMotivoSituacao = 8,
                                DhSituacao = DateTime.Now.AddHours(-3)
                            };
                            _contextEscrita.Add(sdctb018SituacaoMotivo);
                            await _contextEscrita.SaveChangesAsync();
                        }
                        else if (situacaoMomentoPedido.necessitaPericia == false && pedidoIndenizacao.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.OrderByDescending(c => c.DhSituacao).FirstOrDefault().NuTipoMotivoSituacao.Equals(8))
                        {
                            Sdctb010SituacaoPedido situacaoPedido = await _contextLeitura.Sdctb010SituacaoPedido
                          .Where(y => y.NuPedidoIndenizacao == situacaoMomentoPedido.nuPedidoIndenizacao && y.DhExclusao == null).FirstAsync();

                            // setando data fim situação pedido anterior
                            await SetarDataExclusaoSituacoesPedido(situacaoMomentoPedido.nuPedidoIndenizacao);
                            // cadastrar situacao pedido

                            _logger.LogInformation($"AtualizaStatusProcesso - Salvar nova situação do pedido - Processo {situacaoMomentoPedido.nuPedidoIndenizacao}");
                            await _contextEscrita.SaveChangesAsync();

                            // Salvar nova situação do pedido
                            Sdctb010SituacaoPedido sdctb010SituacaoPedido = new Sdctb010SituacaoPedido()
                            {
                                NuTipoSituacaoPedido = 3,
                                DhSituacao = DateTime.Now.AddHours(-3),
                                NuPedidoIndenizacao = situacaoMomentoPedido.nuPedidoIndenizacao,
                                CoMatriculaCaixa = situacaoPedido.CoMatriculaCaixa,

                            };

                            _contextEscrita.Add(sdctb010SituacaoPedido);
                            await _contextEscrita.SaveChangesAsync();

                            // Busca o ID que acabou de ser incluído
                            pedidoIndenizacao.NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido;

                            _logger.LogInformation($"AtualizaStatusProcesso - Atualiza o pedido com a nova situação - Processo {situacaoMomentoPedido.nuPedidoIndenizacao}");
                            // Atualiza o pedido com a nova situação
                            _contextEscrita.Update(pedidoIndenizacao);
                            await _contextEscrita.SaveChangesAsync();
                            Sdctb018SituacaoMotivo sdctb018SituacaoMotivo = new Sdctb018SituacaoMotivo()
                            {
                                NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido,
                                NuTipoMotivoSituacao = 7,
                                DhSituacao = DateTime.Now.AddHours(-3)
                            };
                            _contextEscrita.Add(sdctb018SituacaoMotivo);
                            await _contextEscrita.SaveChangesAsync();
                        }
                    }

                });
            }
            catch (Exception e)
            {

                _logger.LogError($"SalvarAnalise - error {e.Message}");
                throw new(e.Message);
                //await _contextEscrita.Database.RollbackTransactionAsync();

            }
            _logger.LogInformation($"AtualizaStatusProcesso - Fim sucesso -  CarregaInformacaoProcesso - Processo {situacaoMomentoPedido.nuPedidoIndenizacao}");
            return await CarregaInformacaoProcesso(situacaoMomentoPedido.nuPedidoIndenizacao, situacaoMomentoPedido.nuCpf, situacaoMomentoPedido.coMatriculaCaixa);



        }

        private async Task VerificaEmitenteEValores(SituacaoMomentoPedidoRequest situacaopedido, IndenizacaoConfirmacaoProcessoRequest indenizacoes)
        {

            List<Sdctb002DocumentoCapturado> documentoCapturados = await _contextLeitura.Sdctb002DocumentoCapturado
                .Include(x => x.NuDocumentoExigidoNavigation)
                .Where(x => x.NuPedidoIndenizacao == situacaopedido.nuPedidoIndenizacao && x.NuDocumentoExigidoNavigation.NuTipoDocumento == 12).ToListAsync();

            bool Isrejeitado = true;

            foreach (var dcp in documentoCapturados)
            {
                foreach (var doc in situacaopedido.documentos)
                {
                    if (dcp.NuDocumentoCapturado == doc.nuDocumentoCapturado)
                    {
                        Isrejeitado = doc.fgDocumentoAprovado.HasValue ? doc.fgDocumentoAprovado.Value : Isrejeitado;
                        break;
                    }
                }
                if (!Isrejeitado)
                {
                    break;
                }
            }

            decimal valorTotalRecibos = 0;

            foreach (var emitente in situacaopedido.emitentes)
            {
                foreach (var item in emitente.itensDespesa)
                {
                    if (!item.IcExcluido)
                    {
                        valorTotalRecibos += item.vrItemDespesa;
                    }
                }

            }

            if ((situacaopedido.emitentes.Count == 0) && (Isrejeitado))
                throw new ArgumentException($"Não é possivel concluir uma solicitação sem emitentes cadastrados");

            if (Math.Round(valorTotalRecibos, 2) < indenizacoes.vrIndenizacao)
                throw new ArgumentException($"Valor da Indenização DAMS, não pode ser maior que o valor total dos recibos.");
        }

        private async Task ExcluirItensDispesa(long? nuEmitente)
        {
            List<Sdctb035ItemDespesa> sdctb035ItemDespesas = await _contextLeitura.Sdctb035ItemDespesas
            .Where(c => c.NuEmitente == nuEmitente).ToListAsync();

            if (sdctb035ItemDespesas.Count > 0)
            {
                _contextEscrita.RemoveRange(sdctb035ItemDespesas);
                await _contextEscrita.SaveChangesAsync();
            }
        }

        private async Task VerificarValorTetoIndenizacao(TipoIndenizacao indenizacao, decimal vrIndenizacao)
        {
            //No ato de salvar o estado do processo não deve permitir salvar valor maior do que o disponivel para idenização
            var vigenciaIndenizacao = await _contextLeitura.Sdctb046VigenciaIndenizacaos
             .FirstOrDefaultAsync(x => x.NuTipoIndenizacao == (short)indenizacao);

            var tipoIndenizacao = await _contextLeitura.Sdctb007TipoIndenizacao
         .FirstOrDefaultAsync(x => x.NuTipoIndenizacao == (short)indenizacao);

            if (vrIndenizacao > vigenciaIndenizacao.VrLimiteIndenizacao)
                throw new ArgumentException($"Valor da indenização ({tipoIndenizacao.DeAbreviaturaTipoIndenizaca}) não pode ser superior ao permitido pelo teto de {vigenciaIndenizacao.VrLimiteIndenizacao}");

        }
    }
}