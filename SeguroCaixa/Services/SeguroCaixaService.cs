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

namespace SeguroCaixa.Services
{
    public class SeguroCaixaService
    {
        #region Inicializadores e Construtor
        private readonly DbEscrita _context;
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<SeguroCaixaService> _logger;
        private IConfiguration _configuration;

        public SeguroCaixaService(DbEscrita context, ILogger<SeguroCaixaService> logger, DbLeitura contextLeitura, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
        }
        #endregion

        //@TODO Refatorar consulta processo para modelo dos documentos exigidos
        public async Task<ProcessoResponse> ConsultaProcesso(int numeroProcesso)
        {
            _logger.LogInformation($"Início ConsultaProcesso - numero do processo: {numeroProcesso}");

            try
            {
                if (numeroProcesso <= 0)
                {
                    new System.ArgumentException(ErrosMsg.Erro1, "numeroProcesso");
                }
                Sdctb008PedidoIndenizacao sdctb008PedidoIndenizacao = await _contextLeitura
                    .Sdctb008PedidoIndenizacao
                    .Include(s => s.Sdctb015Participacaos)
                    .Include(a => a.NuPessoaSolicitanteNavigation)
                    .Include(d => d.Sdctb002DocumentoCapturados)
                    .Where(x => x.NuPedidoIndenizacao == numeroProcesso)
                    .FirstOrDefaultAsync();

                ProcessoResponse processoResponse = new ProcessoResponse();

                /*
                ProcessoResponse processoResponse = new ProcessoResponse
                    {
                        DadosPessoa = new DadosPessoaisVitimaRequest
                        {
                            Bairro = sdctb008PedidoIndenizacao.s.NoBairro,
                            Complemento = par.DeComplemento,
                            IdPessoa = par.NuPessoa,
                            Logradouro = par.NoLogradouro,
                            Municipio = (short)par.NuMunicipio,
                            Numero = par.CoPosicaoImovel,
                            TipoLogradouro = par.SgTipoLogradouro
                        },
                        DadosAcidente = new DadosAcidenteRequest
                        {
                            Municipio = c.NuMunicipio,
                            DataHoraAcidente = c.DhSinistro
                            // @TODO adicionar restante dos dados do endereco do pedido
                        },
                        IdTipoParticipacao = par.NuTipoParticipacao,
                        IdTipoPedido = c.NuTipoPedido,
                        IdTipoVeiculo = c.NuTipoVeiculo,
                        ListaImagemDocumeto = new List<ImagemDocumentoAnexo>()
                    }).FirstOrDefaultAsync();
                */

                _logger.LogDebug($"ConsultaProcesso - {sdctb008PedidoIndenizacao.ToJson()}");

                _logger.LogInformation($"ConsultaProcesso - retorno {processoResponse.ToJson()}");

                return processoResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"ConsultaProcesso - error {e.Message}");

                throw e;
            }
        }

        /**
         * Retorna o tipo de beneficiario que deve ser exibido no fluxo a depender do tipo de indenizacao padronizado
         */
        public async Task<List<TipoBeneficiarioResponse>> TiposBeneficiario(short? IdTipoIndenizacao)
        {
            _logger.LogInformation($"Start TiposBeneficiario - IdTipoIndenizacao: {IdTipoIndenizacao}");

            try
            {
                List<TipoBeneficiarioResponse> tipoBeneficiarios = await _contextLeitura
                    .Sdctb024SolicitanteIndenizacao
                    .Include(c => c.NuTipoParticipacaoNavigation)
                    .Where(x => x.NuTipoIndenizacao == IdTipoIndenizacao && x.DhExclusao == null)
                    .Select(s => new TipoBeneficiarioResponse()
                    {
                        Codigo = s.NuTipoParticipacao,
                        Descricao = s.NuTipoParticipacaoNavigation.DeTipoParticipacao
                    })
                    .OrderBy(o => o.Codigo)
                    .ToListAsync();

                _logger.LogInformation($"TiposBeneficiario - retorno {tipoBeneficiarios.ToJson()}");

                return tipoBeneficiarios;
            }
            catch (Exception e)
            {
                _logger.LogError($"TiposBeneficiario - erro", e.Message);
                throw;
            }
        }

        public async Task<List<TipoVeiculoAcidenteResponse>> TipoVeiculoAcidente()
        {
            _logger.LogInformation($"Start TipoVeiculoAcidente ");

            try
            {
                List<TipoVeiculoAcidenteResponse> tipoVeiculoAcidenteResponse = await (
                    from c in _contextLeitura.Sdctb012TipoVeiculo
                    orderby c.DeTipoVeiculo
                    select
                        new TipoVeiculoAcidenteResponse
                        {
                            Descricao = c.DeTipoVeiculo,
                            Codigo = c.NuTipoVeiculo
                        }).ToListAsync();

                _logger.LogInformation($"TipoVeiculoAcidente - retorno {tipoVeiculoAcidenteResponse.ToJson()}");

                return tipoVeiculoAcidenteResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"TipoVeiculoAcidente - erro", e.Message);
                throw;
            }
        }

        public async Task<List<TipoPedidoResponse>> TipoPedidoIndenizacao()
        {
            _logger.LogInformation($"Start TipoPedidoIndenizacao ");

            try
            {
                List<TipoPedidoResponse> tipoPedidos = await (from c in _contextLeitura.Sdctb007TipoIndenizacao
                                                              orderby c.NuTipoIndenizacao
                                                              select new TipoPedidoResponse
                                                              {
                                                                  Descricao = c.DeTipoIndenizacao,
                                                                  Id = c.NuTipoIndenizacao,
                                                                  Titulo = c.DeAbreviaturaTipoIndenizaca
                                                              }).ToListAsync();

                _logger.LogInformation($"TipoPedidoIndenizacao - retorno {tipoPedidos.ToJson()}");

                return tipoPedidos;
            }
            catch (Exception e)
            {
                _logger.LogError($"TipoPedidoIndenizacao - erro", e.Message);
                throw;
            }

        }

        public async Task<MensagemProcessoResponse> ProcessaRequisicaoSeguro(DadosRequisicaoSeguroRequest dadosRequisicaoSeguro, decimal nuCpf)
        {
            _logger.LogInformation($"Start ProcessaRequisicaoSeguro -  DadosRequisicaoSeguroRequest {dadosRequisicaoSeguro.ToJson()}");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();

            if (!await _contextLeitura.Sdctb009Pessoa.AnyAsync(p => p.NuPessoa == dadosRequisicaoSeguro.DadosPessoa.IdPessoa && p.NuCpf == nuCpf || p.NuCpf == dadosRequisicaoSeguro.DadosProcurador.Cpf 
            || p.NuPessoa == dadosRequisicaoSeguro.DadosRepresentanteLegal.IdPessoa && p.NuCpf == nuCpf))
            {
                mensagemProcessoResponse.CodigoRetorno = 401;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Não autorizado." };

                _logger.LogError($"ProcessaRequisicaoSeguro - error - DadosRequisicaoSeguroRequest {mensagemProcessoResponse.ToJson()}");

                return mensagemProcessoResponse;
            }

            try
            {
                await ResilientTransaction.New(_context).ExecuteAsync(async () =>
                {
                    // cadastrar pedido
                    Sdctb008PedidoIndenizacao sdctb008PedidoIndenizacao = new Sdctb008PedidoIndenizacao();

                    sdctb008PedidoIndenizacao.NuCanal = (short)dadosRequisicaoSeguro.Canal;
                    sdctb008PedidoIndenizacao.NuMunicipio = dadosRequisicaoSeguro.DadosAcidente.Municipio;
                    sdctb008PedidoIndenizacao.NuPessoaSolicitante = dadosRequisicaoSeguro.DadosPessoa.IdPessoa;
                    sdctb008PedidoIndenizacao.NuTipoIndenizacao = dadosRequisicaoSeguro.IdTipoPedido;
                    sdctb008PedidoIndenizacao.NuTipoVeiculo = dadosRequisicaoSeguro.IdTipoVeiculo;
                    sdctb008PedidoIndenizacao.DhPedido = DateTime.Now.AddHours(-3);
                    sdctb008PedidoIndenizacao.DhSinistro = dadosRequisicaoSeguro.DadosAcidente.DataHoraAcidente;
                    sdctb008PedidoIndenizacao.SgTipoLogradouro = dadosRequisicaoSeguro.DadosAcidente.TipoLogradouro;
                    sdctb008PedidoIndenizacao.NoLogradouro = dadosRequisicaoSeguro.DadosAcidente.Logradouro;
                    sdctb008PedidoIndenizacao.CoPosicaoImovel = dadosRequisicaoSeguro.DadosAcidente.Numero;
                    sdctb008PedidoIndenizacao.DeComplemento = dadosRequisicaoSeguro.DadosAcidente.Complemento;
                    sdctb008PedidoIndenizacao.NoBairro = dadosRequisicaoSeguro.DadosAcidente.Bairro;

                    _logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb008PedidoIndenizacao {sdctb008PedidoIndenizacao.ToJson()}");

                    _context.Add(sdctb008PedidoIndenizacao);
                    await _context.SaveChangesAsync();

                    // salvar acordos assinados
                    await SalvarAcordos(sdctb008PedidoIndenizacao.NuPedidoIndenizacao);

                    // cadastrar situacao pedido
                    Sdctb010SituacaoPedido sdctb010SituacaoPedido = new Sdctb010SituacaoPedido();
                    sdctb010SituacaoPedido.DhSituacao = DateTime.Now.AddHours(-3);
                    sdctb010SituacaoPedido.NuTipoSituacaoPedido = 1; // solicitacao pendente de documentos
                    sdctb010SituacaoPedido.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;

                    _logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb010SituacaoPedido {sdctb010SituacaoPedido.ToJson()}");

                    _context.Add(sdctb010SituacaoPedido);
                    await _context.SaveChangesAsync();

                    // atualizar situacao no pedido
                    sdctb008PedidoIndenizacao.NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido;

                    // cadastrar participacao pessoa
                    Sdctb015Participacao sdctb015Participacao = new Sdctb015Participacao();

                    sdctb015Participacao.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                    sdctb015Participacao.NuPessoaParticipante = dadosRequisicaoSeguro.DadosPessoa.IdPessoa;
                    sdctb015Participacao.NuTipoParticipacao = dadosRequisicaoSeguro.DadosPessoa.IdTipoParticipacao;
                    sdctb015Participacao.SgTipoLogradouro = dadosRequisicaoSeguro.DadosPessoa.TipoLogradouro;

                    if (dadosRequisicaoSeguro.DadosRepresentanteLegal.IdTipoParticipacao == null )
                    {
                        sdctb015Participacao.NuTelefone = dadosRequisicaoSeguro.Telefone;
                        sdctb015Participacao.NuDdd = dadosRequisicaoSeguro.Ddd;
                        sdctb015Participacao.DeEmail = dadosRequisicaoSeguro.Email;
                        sdctb015Participacao.NuCep = dadosRequisicaoSeguro.Cep;
                    }                   

                    sdctb015Participacao.NuMunicipio = dadosRequisicaoSeguro.DadosPessoa.Municipio;
                    sdctb015Participacao.NoLogradouro = dadosRequisicaoSeguro.DadosPessoa.Logradouro;
                    sdctb015Participacao.NoBairro = dadosRequisicaoSeguro.DadosPessoa.Bairro;
                    sdctb015Participacao.DeComplemento = dadosRequisicaoSeguro.DadosPessoa.Complemento;
                    sdctb015Participacao.CoPosicaoImovel = dadosRequisicaoSeguro.DadosPessoa.Numero;

                    // Novos atributos de acordo com fluxo de sinistro por morte com beneficiario (TipoIndenizacao = 4)
                    sdctb015Participacao.NuNacionalidade = dadosRequisicaoSeguro.DadosPessoa.IdNacionalidade;
                    sdctb015Participacao.NuOcupacao = dadosRequisicaoSeguro.DadosPessoa.IdOcupacao;
                    sdctb015Participacao.IcFormal = dadosRequisicaoSeguro.DadosPessoa.IcFormal;
                    sdctb015Participacao.VrRenda = dadosRequisicaoSeguro.DadosPessoa.Renda;
                    sdctb015Participacao.DtRenda = dadosRequisicaoSeguro.DadosPessoa.DataRenda;
                    sdctb015Participacao.VrPatrimonio = dadosRequisicaoSeguro.DadosPessoa.Patrimonio;
                    sdctb015Participacao.NuTipoVeiculoProprietario = dadosRequisicaoSeguro.DadosPessoa.IdVeiculoPatrimonio;
                    sdctb015Participacao.AaFabricacao = dadosRequisicaoSeguro.DadosPessoa.AnoFabricacaoVeiculoPatrimonio;
                    sdctb015Participacao.AaModelo = dadosRequisicaoSeguro.DadosPessoa.AnoModeloVeiculoPatrimonio;
                    sdctb015Participacao.NuNif = dadosRequisicaoSeguro.DadosPessoa.Nif;

                    _logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb015Participacao {sdctb015Participacao.ToJson()}");

                    //cadastra procurador caso possuir
                    if (dadosRequisicaoSeguro.DadosProcurador.IdTipoParticipacao != null)
                    {
                        Sdctb009Pessoa pessoa = await _contextLeitura
                            .Sdctb009Pessoa
                            .Where(x => x.NuCpf == dadosRequisicaoSeguro.DadosProcurador.Cpf)
                            .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteProcurador = new Sdctb015Participacao();
                        participanteProcurador.NuPessoaParticipante = pessoa.NuPessoa;
                        participanteProcurador.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                        participanteProcurador.NuTipoParticipacao = (short)dadosRequisicaoSeguro.DadosProcurador.IdTipoParticipacao;

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteProcurador {participanteProcurador.ToJson()}");

                        _context.Add(participanteProcurador);
                        await _context.SaveChangesAsync();
                    }

                    //cadastrar representante legal caso exista
                    if (dadosRequisicaoSeguro.DadosRepresentanteLegal.IdTipoParticipacao != null)
                    {
                        Sdctb009Pessoa pessoa = await _contextLeitura
                            .Sdctb009Pessoa
                            .Where(x => x.NuPessoa == dadosRequisicaoSeguro.DadosRepresentanteLegal.IdPessoa)
                            .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteRepresentanteLegal = new Sdctb015Participacao();

                        participanteRepresentanteLegal.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                        participanteRepresentanteLegal.NuPessoaParticipante = (long)dadosRequisicaoSeguro.DadosRepresentanteLegal.IdPessoa;
                        participanteRepresentanteLegal.NuTipoParticipacao = (short)dadosRequisicaoSeguro.DadosRepresentanteLegal.IdTipoParticipacao;
                        participanteRepresentanteLegal.SgTipoLogradouro = dadosRequisicaoSeguro.DadosRepresentanteLegal.TipoLogradouro;
                        participanteRepresentanteLegal.NuTelefone = dadosRequisicaoSeguro.Telefone;
                        participanteRepresentanteLegal.NuDdd = dadosRequisicaoSeguro.Ddd;
                        participanteRepresentanteLegal.DeEmail = dadosRequisicaoSeguro.Email;
                        participanteRepresentanteLegal.NuCep = dadosRequisicaoSeguro.Cep;
                        participanteRepresentanteLegal.NuMunicipio = dadosRequisicaoSeguro.DadosRepresentanteLegal.Municipio;
                        participanteRepresentanteLegal.NoLogradouro = dadosRequisicaoSeguro.DadosRepresentanteLegal.Logradouro;
                        participanteRepresentanteLegal.NoBairro = dadosRequisicaoSeguro.DadosRepresentanteLegal.Bairro;
                        participanteRepresentanteLegal.DeComplemento = dadosRequisicaoSeguro.DadosRepresentanteLegal.Complemento;
                        participanteRepresentanteLegal.CoPosicaoImovel = dadosRequisicaoSeguro.DadosRepresentanteLegal.Numero;
                        participanteRepresentanteLegal.NuNacionalidade = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdNacionalidade;
                        participanteRepresentanteLegal.NuOcupacao = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdOcupacao;
                        participanteRepresentanteLegal.IcFormal = dadosRequisicaoSeguro.DadosRepresentanteLegal.IcFormal;
                        participanteRepresentanteLegal.VrRenda = dadosRequisicaoSeguro.DadosRepresentanteLegal.Renda;
                        participanteRepresentanteLegal.DtRenda = dadosRequisicaoSeguro.DadosRepresentanteLegal.DataRenda;
                        participanteRepresentanteLegal.VrPatrimonio = dadosRequisicaoSeguro.DadosRepresentanteLegal.Patrimonio;
                        participanteRepresentanteLegal.NuTipoVeiculoProprietario = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdVeiculoPatrimonio;
                        participanteRepresentanteLegal.AaFabricacao = dadosRequisicaoSeguro.DadosRepresentanteLegal.AnoFabricacaoVeiculoPatrimonio;
                        participanteRepresentanteLegal.AaModelo = dadosRequisicaoSeguro.DadosRepresentanteLegal.AnoModeloVeiculoPatrimonio;
                        participanteRepresentanteLegal.NuNif = dadosRequisicaoSeguro.DadosRepresentanteLegal.Nif;

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteProcurador {participanteRepresentanteLegal.ToJson()}");

                        _context.Add(participanteRepresentanteLegal);
                        await _context.SaveChangesAsync();
                    }

                    if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                    {
                        _logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb008PedidoIndenizacao.NuTipoIndenizacao {sdctb008PedidoIndenizacao.NuTipoIndenizacao}");

                        sdctb015Participacao.NuParentesco = dadosRequisicaoSeguro.DadosPessoa.IdParentescoVitima;

                        Sdctb009Pessoa pessoa = await _contextLeitura
                            .Sdctb009Pessoa
                            .Where(x => x.NuCpf == dadosRequisicaoSeguro.DadosPessoa.CpfVitima)
                            .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteVitima = new Sdctb015Participacao();
                        participanteVitima.NuPessoaParticipante = pessoa.NuPessoa;
                        participanteVitima.NuEstadoCivil = dadosRequisicaoSeguro.DadosPessoa.IdEstadoCivilVitima;
                        participanteVitima.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                        participanteVitima.NuTipoParticipacao = 1;

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteVitima {participanteVitima.ToJson()}");

                        _context.Add(participanteVitima);
                        await _context.SaveChangesAsync();

                        foreach (DadosDependentesRequest item in dadosRequisicaoSeguro.DadosDependentes)
                        {
                            Sdctb009Pessoa sdctb009Pessoa = new Sdctb009Pessoa();
                            if (item.CpfParente != null)
                            {
                                sdctb009Pessoa = await _contextLeitura
                                    .Sdctb009Pessoa
                                    .Where(c => c.NuCpf == item.CpfParente)
                                    .FirstOrDefaultAsync();
                            }

                            Sdctb023DeclaraHerdeiro sdctb023DeclaraHerdeiro = new Sdctb023DeclaraHerdeiro()
                            {
                                NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao,
                                NuParentesco = item.IdParentesco,
                                NuPessoaHerdeiro = sdctb009Pessoa == null ? null : sdctb009Pessoa.NuPessoa
                            };

                            _context.Add(sdctb023DeclaraHerdeiro);
                            await _context.SaveChangesAsync();
                        }
                    }

                    _context.Add(sdctb015Participacao);
                    await _context.SaveChangesAsync();

                    mensagemProcessoResponse.CodigoRetorno = 201;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = sdctb008PedidoIndenizacao.NuPedidoIndenizacao.ToString(), Mensagem = "Solicitação realizada com sucesso" };


                    _logger.LogInformation($"ProcessaRequisicaoSeguro - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcessaRequisicaoSeguro - error -  {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = $"Dados invalidos para gravacao - {e.Message}" };

                _logger.LogError($"ProcessaRequisicaoSeguro - error - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");

                await _context.Database.RollbackTransactionAsync();

                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        public async Task<MensagemProcessoResponse> AtualizaProcessaRequisicaoSeguro(DadosRequisicaoSeguroRequest dadosRequisicaoSeguro, decimal nuCpf)
        {
            _logger.LogInformation($"Start ProcessaRequisicaoSeguro -  DadosRequisicaoSeguroRequest {dadosRequisicaoSeguro.ToJson()}");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();

            if (!await _contextLeitura.Sdctb009Pessoa.AnyAsync(p => p.NuPessoa == dadosRequisicaoSeguro.DadosPessoa.IdPessoa && p.NuCpf == nuCpf || p.NuCpf == dadosRequisicaoSeguro.DadosProcurador.Cpf
            || p.NuPessoa == dadosRequisicaoSeguro.DadosRepresentanteLegal.IdPessoa && p.NuCpf == nuCpf))
            {
                mensagemProcessoResponse.CodigoRetorno = 401;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Não autorizado." };

                _logger.LogError($"ProcessaRequisicaoSeguro - error - DadosRequisicaoSeguroRequest {mensagemProcessoResponse.ToJson()}");

                return mensagemProcessoResponse;
            }

            try
            {
                await ResilientTransaction.New(_context).ExecuteAsync(async () =>
                {
                    // cadastrar pedido
                    Sdctb008PedidoIndenizacao sdctb008PedidoIndenizacao = await _context.Sdctb008PedidoIndenizacao
                    .Include(c => c.Sdctb015Participacaos)
                    .Where(p => (p.NuPedidoIndenizacao == dadosRequisicaoSeguro.NuPedido && p.NuPessoaSolicitanteNavigation.NuCpf == nuCpf)
                    || (p.NuPedidoIndenizacao == dadosRequisicaoSeguro.NuPedido && p.Sdctb015Participacaos.Any(x => x.NuPessoaParticipante == p.NuPessoaSolicitanteNavigation.NuPessoa))).FirstOrDefaultAsync();

                    sdctb008PedidoIndenizacao.NuCanal = (short)dadosRequisicaoSeguro.Canal;
                    sdctb008PedidoIndenizacao.NuMunicipio = dadosRequisicaoSeguro.DadosAcidente.Municipio;
                    sdctb008PedidoIndenizacao.NuPessoaSolicitante = dadosRequisicaoSeguro.DadosPessoa.IdPessoa;
                    sdctb008PedidoIndenizacao.NuTipoIndenizacao = dadosRequisicaoSeguro.IdTipoPedido;
                    sdctb008PedidoIndenizacao.NuTipoVeiculo = dadosRequisicaoSeguro.IdTipoVeiculo;
                    sdctb008PedidoIndenizacao.DhPedido = DateTime.Now.AddHours(-3);
                    sdctb008PedidoIndenizacao.DhSinistro = dadosRequisicaoSeguro.DadosAcidente.DataHoraAcidente;
                    sdctb008PedidoIndenizacao.SgTipoLogradouro = dadosRequisicaoSeguro.DadosAcidente.TipoLogradouro;
                    sdctb008PedidoIndenizacao.NoLogradouro = dadosRequisicaoSeguro.DadosAcidente.Logradouro;
                    sdctb008PedidoIndenizacao.CoPosicaoImovel = dadosRequisicaoSeguro.DadosAcidente.Numero;
                    sdctb008PedidoIndenizacao.DeComplemento = dadosRequisicaoSeguro.DadosAcidente.Complemento;
                    sdctb008PedidoIndenizacao.NoBairro = dadosRequisicaoSeguro.DadosAcidente.Bairro;

                    //_logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb008PedidoIndenizacao {sdctb008PedidoIndenizacao.ToJson()}");

                    _context.Update(sdctb008PedidoIndenizacao);
                    await _context.SaveChangesAsync();

                    // cadastrar participacao pessoa
                    List<Sdctb015Participacao> ListaParticipantes = await _context.Sdctb015Participacao
                            .Where(p => p.NuPedidoIndenizacao == dadosRequisicaoSeguro.NuPedido)
                            .ToListAsync();
                    _logger.LogDebug($"ProcessaRequisicaoSeguro - sdctb008PedidoIndenizacao.NuTipoIndenizacao {sdctb008PedidoIndenizacao.NuTipoIndenizacao}");

                    //atualiza procurador
                    if (dadosRequisicaoSeguro.DadosProcurador.IdTipoParticipacao != null)
                    {
                        Sdctb009Pessoa pessoa = await _contextLeitura
                            .Sdctb009Pessoa
                            .Where(x => x.NuCpf == dadosRequisicaoSeguro.DadosProcurador.Cpf)
                            .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteProcurador = new Sdctb015Participacao();
                        participanteProcurador.NuPessoaParticipante = pessoa.NuPessoa;
                        participanteProcurador.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                        participanteProcurador.NuTipoParticipacao = (short)dadosRequisicaoSeguro.DadosProcurador.IdTipoParticipacao;

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteProcurador {participanteProcurador.ToJson()}");

                        _context.Update(participanteProcurador);
                        await _context.SaveChangesAsync();
                    }

                    //atualiza representante legal
                    if (dadosRequisicaoSeguro.DadosRepresentanteLegal.IdTipoParticipacao != null)
                    {
                        Sdctb009Pessoa pessoa = await _contextLeitura
                            .Sdctb009Pessoa
                            .Where(x => x.NuPessoa == dadosRequisicaoSeguro.DadosRepresentanteLegal.IdPessoa)
                            .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteRepresentanteLegal = ListaParticipantes.Where(p => p.NuTipoParticipacao == 3).FirstOrDefault();                   
                        
                        participanteRepresentanteLegal.SgTipoLogradouro = dadosRequisicaoSeguro.DadosRepresentanteLegal.TipoLogradouro;
                        participanteRepresentanteLegal.NuTelefone = dadosRequisicaoSeguro.Telefone;
                        participanteRepresentanteLegal.NuDdd = dadosRequisicaoSeguro.Ddd;
                        participanteRepresentanteLegal.DeEmail = dadosRequisicaoSeguro.Email;
                        participanteRepresentanteLegal.NuCep = dadosRequisicaoSeguro.Cep;
                        participanteRepresentanteLegal.NuMunicipio = dadosRequisicaoSeguro.DadosRepresentanteLegal.Municipio;
                        participanteRepresentanteLegal.NoLogradouro = dadosRequisicaoSeguro.DadosRepresentanteLegal.Logradouro;
                        participanteRepresentanteLegal.NoBairro = dadosRequisicaoSeguro.DadosRepresentanteLegal.Bairro;
                        participanteRepresentanteLegal.DeComplemento = dadosRequisicaoSeguro.DadosRepresentanteLegal.Complemento;
                        participanteRepresentanteLegal.CoPosicaoImovel = dadosRequisicaoSeguro.DadosRepresentanteLegal.Numero;
                        participanteRepresentanteLegal.NuNacionalidade = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdNacionalidade;
                        participanteRepresentanteLegal.NuOcupacao = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdOcupacao;
                        participanteRepresentanteLegal.IcFormal = dadosRequisicaoSeguro.DadosRepresentanteLegal.IcFormal;
                        participanteRepresentanteLegal.VrRenda = dadosRequisicaoSeguro.DadosRepresentanteLegal.Renda;
                        participanteRepresentanteLegal.DtRenda = dadosRequisicaoSeguro.DadosRepresentanteLegal.DataRenda;
                        participanteRepresentanteLegal.VrPatrimonio = dadosRequisicaoSeguro.DadosRepresentanteLegal.Patrimonio;
                        participanteRepresentanteLegal.NuTipoVeiculoProprietario = dadosRequisicaoSeguro.DadosRepresentanteLegal.IdVeiculoPatrimonio;
                        participanteRepresentanteLegal.AaFabricacao = dadosRequisicaoSeguro.DadosRepresentanteLegal.AnoFabricacaoVeiculoPatrimonio;
                        participanteRepresentanteLegal.AaModelo = dadosRequisicaoSeguro.DadosRepresentanteLegal.AnoModeloVeiculoPatrimonio;
                        participanteRepresentanteLegal.NuNif = dadosRequisicaoSeguro.DadosRepresentanteLegal.Nif;

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteRepresentanteLegal {participanteRepresentanteLegal}");

                        _context.Update(participanteRepresentanteLegal);
                        await _context.SaveChangesAsync();
                    }

                    if (sdctb008PedidoIndenizacao.NuTipoIndenizacao == 4)
                    {
                        Sdctb015Participacao sdctb015Participacao = ListaParticipantes.Where(p => p.NuTipoParticipacao == 2).FirstOrDefault();
                        sdctb015Participacao.NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao;
                        sdctb015Participacao.NuPessoaParticipante = dadosRequisicaoSeguro.DadosPessoa.IdPessoa;
                        sdctb015Participacao.NuTipoParticipacao = dadosRequisicaoSeguro.DadosPessoa.IdTipoParticipacao;
                        sdctb015Participacao.SgTipoLogradouro = dadosRequisicaoSeguro.DadosPessoa.TipoLogradouro;                        
                        sdctb015Participacao.NuTelefone = dadosRequisicaoSeguro.Telefone;
                        sdctb015Participacao.NuDdd = dadosRequisicaoSeguro.Ddd;
                        sdctb015Participacao.DeEmail = dadosRequisicaoSeguro.Email;
                        sdctb015Participacao.NuCep = dadosRequisicaoSeguro.Cep;
                        sdctb015Participacao.NuMunicipio = dadosRequisicaoSeguro.DadosPessoa.Municipio;
                        sdctb015Participacao.NoLogradouro = dadosRequisicaoSeguro.DadosPessoa.Logradouro;
                        sdctb015Participacao.NoBairro = dadosRequisicaoSeguro.DadosPessoa.Bairro;
                        sdctb015Participacao.DeComplemento = dadosRequisicaoSeguro.DadosPessoa.Complemento;
                        sdctb015Participacao.CoPosicaoImovel = dadosRequisicaoSeguro.DadosPessoa.Numero;

                        // Novos atributos de acordo com fluxo de sinistro por morte com beneficiario (TipoIndenizacao = 4)
                        sdctb015Participacao.NuNacionalidade = dadosRequisicaoSeguro.DadosPessoa.IdNacionalidade;
                        sdctb015Participacao.NuOcupacao = dadosRequisicaoSeguro.DadosPessoa.IdOcupacao;
                        sdctb015Participacao.IcFormal = dadosRequisicaoSeguro.DadosPessoa.IcFormal;
                        sdctb015Participacao.VrRenda = dadosRequisicaoSeguro.DadosPessoa.Renda;
                        sdctb015Participacao.DtRenda = dadosRequisicaoSeguro.DadosPessoa.DataRenda;
                        sdctb015Participacao.VrPatrimonio = dadosRequisicaoSeguro.DadosPessoa.Patrimonio;
                        sdctb015Participacao.NuTipoVeiculoProprietario = dadosRequisicaoSeguro.DadosPessoa.IdVeiculoPatrimonio;
                        sdctb015Participacao.AaFabricacao = dadosRequisicaoSeguro.DadosPessoa.AnoFabricacaoVeiculoPatrimonio;
                        sdctb015Participacao.AaModelo = dadosRequisicaoSeguro.DadosPessoa.AnoModeloVeiculoPatrimonio;
                        sdctb015Participacao.NuNif = dadosRequisicaoSeguro.DadosPessoa.Nif;

                        sdctb015Participacao.NuParentesco = dadosRequisicaoSeguro.DadosPessoa.IdParentescoVitima;

                        // Busca a vítima pelo tipo de indenização e pelo número do pedido já gravado

                        Sdctb009Pessoa pessoa = await _contextLeitura
                          .Sdctb009Pessoa
                          .Where(x => x.NuCpf == dadosRequisicaoSeguro.DadosPessoa.CpfVitima)
                          .FirstOrDefaultAsync();

                        Sdctb015Participacao participanteVitima = ListaParticipantes.Where(p => p.NuTipoParticipacao == 1).FirstOrDefault();
                        participanteVitima.NuPessoaParticipante = pessoa.NuPessoa;
                        participanteVitima.NuEstadoCivil = dadosRequisicaoSeguro.DadosPessoa.IdEstadoCivilVitima;

                        _context.UpdateRange(ListaParticipantes);
                        await _context.SaveChangesAsync();

                        _logger.LogDebug($"ProcessaRequisicaoSeguro - participanteVitima {participanteVitima}");

                        // Remove todos os herdeiros anteriores
                        List<Sdctb023DeclaraHerdeiro> listaHerdeiros = await _context.Sdctb023DeclaraHerdeiro.Where(h => h.NuPedidoIndenizacao == dadosRequisicaoSeguro.NuPedido).ToListAsync();

                        _context.RemoveRange(listaHerdeiros);
                        await _context.SaveChangesAsync();

                        // Readiciona os novos herdeiros
                        foreach (DadosDependentesRequest item in dadosRequisicaoSeguro.DadosDependentes)
                        {
                            Sdctb009Pessoa sdctb009Pessoa = new Sdctb009Pessoa();
                            if (item.CpfParente != null)
                            {
                                sdctb009Pessoa = await _contextLeitura
                                    .Sdctb009Pessoa
                                    .Where(c => c.NuCpf == item.CpfParente)
                                    .FirstOrDefaultAsync();
                            }

                            Sdctb023DeclaraHerdeiro sdctb023DeclaraHerdeiro = new Sdctb023DeclaraHerdeiro()
                            {
                                NuPedidoIndenizacao = sdctb008PedidoIndenizacao.NuPedidoIndenizacao,
                                NuParentesco = item.IdParentesco,
                                NuPessoaHerdeiro = sdctb009Pessoa == null ? null : sdctb009Pessoa.NuPessoa
                            };

                            _context.Add(sdctb023DeclaraHerdeiro);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        Sdctb015Participacao sdctb015Participacao = ListaParticipantes.Where(p => p.NuTipoParticipacao == 1).FirstOrDefault();
                      
                        sdctb015Participacao.SgTipoLogradouro = dadosRequisicaoSeguro.DadosPessoa.TipoLogradouro;

                        if (dadosRequisicaoSeguro.DadosRepresentanteLegal.IdTipoParticipacao == null)
                        {
                            sdctb015Participacao.NuTelefone = dadosRequisicaoSeguro.Telefone;
                            sdctb015Participacao.NuDdd = dadosRequisicaoSeguro.Ddd;
                            sdctb015Participacao.DeEmail = dadosRequisicaoSeguro.Email;
                            sdctb015Participacao.NuCep = dadosRequisicaoSeguro.Cep;
                        }
                        
                        sdctb015Participacao.NuMunicipio = dadosRequisicaoSeguro.DadosPessoa.Municipio;
                        sdctb015Participacao.NoLogradouro = dadosRequisicaoSeguro.DadosPessoa.Logradouro;
                        sdctb015Participacao.NoBairro = dadosRequisicaoSeguro.DadosPessoa.Bairro;
                        sdctb015Participacao.DeComplemento = dadosRequisicaoSeguro.DadosPessoa.Complemento;
                        sdctb015Participacao.CoPosicaoImovel = dadosRequisicaoSeguro.DadosPessoa.Numero;
                        // Novos atributos de acordo com fluxo de sinistro por morte com beneficiario (TipoIndenizacao = 4)
                        sdctb015Participacao.NuNacionalidade = dadosRequisicaoSeguro.DadosPessoa.IdNacionalidade;
                        sdctb015Participacao.NuOcupacao = dadosRequisicaoSeguro.DadosPessoa.IdOcupacao;
                        sdctb015Participacao.IcFormal = dadosRequisicaoSeguro.DadosPessoa.IcFormal;
                        sdctb015Participacao.VrRenda = dadosRequisicaoSeguro.DadosPessoa.Renda;
                        sdctb015Participacao.DtRenda = dadosRequisicaoSeguro.DadosPessoa.DataRenda;
                        sdctb015Participacao.VrPatrimonio = dadosRequisicaoSeguro.DadosPessoa.Patrimonio;
                        sdctb015Participacao.NuTipoVeiculoProprietario = dadosRequisicaoSeguro.DadosPessoa.IdVeiculoPatrimonio;
                        sdctb015Participacao.AaFabricacao = dadosRequisicaoSeguro.DadosPessoa.AnoFabricacaoVeiculoPatrimonio;
                        sdctb015Participacao.AaModelo = dadosRequisicaoSeguro.DadosPessoa.AnoModeloVeiculoPatrimonio;
                        sdctb015Participacao.NuNif = dadosRequisicaoSeguro.DadosPessoa.Nif;
                    }

                    _context.UpdateRange(ListaParticipantes);
                    await _context.SaveChangesAsync();

                    mensagemProcessoResponse.CodigoRetorno = 201;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = sdctb008PedidoIndenizacao.NuPedidoIndenizacao.ToString(), Mensagem = "Solicitação realizada com sucesso" };


                    _logger.LogInformation($"ProcessaRequisicaoSeguro - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcessaRequisicaoSeguro - error -  {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Dados invalidos para gravacao" };

                _logger.LogError($"ProcessaRequisicaoSeguro - error - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");

                await _context.Database.RollbackTransactionAsync();

                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        //TODO: verificar qual a próxima situação e parametrizar a atualização conforme a necessidade
        public async Task<MensagemProcessoResponse> AtualizaProcesso(ConfirmarProcessoRequest confirmarProcesso, decimal nuCpf)
        {
            _logger.LogInformation($"Start AtualizaProcesso -  confirmarProcesso {confirmarProcesso.ToJson()} CPF {nuCpf}");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
            try
            {
                await ResilientTransaction.New(_context).ExecuteAsync(async () =>
                {
                    // Busca o pedido desde que seja originado do mesmo solicitante
                    Sdctb008PedidoIndenizacao pedidoIndenizacao = await _context.Sdctb008PedidoIndenizacao
                                                                    .Include(c => c.NuSituacaoPedidoNavigation)
                                                                    .Include(c => c.Sdctb015Participacaos)
                                                                    .ThenInclude(c => c.NuPessoaParticipanteNavigation)
                                                                    .Where(s => (s.NuPedidoIndenizacao == confirmarProcesso.NuPedidoIndenizacao
                                                                            && s.NuPessoaSolicitanteNavigation.NuCpf == nuCpf)
                                                                                    || (s.NuPedidoIndenizacao == confirmarProcesso.NuPedidoIndenizacao &&
                                                                                    s.Sdctb015Participacaos.Any(p => p.NuPessoaParticipanteNavigation.NuCpf == nuCpf)))
                                                                    .FirstOrDefaultAsync();

                    _logger.LogDebug($"AtualizaProcesso - pedidoIndenizacao {pedidoIndenizacao}");

                    // Caso não exista pedido, retornar erro
                    if (pedidoIndenizacao?.NuPedidoIndenizacao == null)
                    {
                        mensagemProcessoResponse.CodigoRetorno = 400;
                        mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "O pedido não pôde ser concluído. Refaça a solicitação." };

                        _logger.LogInformation($"AtualizaProcesso - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                        return;
                    }

                    //atualizar flags de autorização dos termos de abertura de conta no caixa tem
                    pedidoIndenizacao.IcAutorizaConversao = true;
                    pedidoIndenizacao.IcAutorizaCredito = true;
                    pedidoIndenizacao.NuSituacaoPedidoNavigation.DhExclusao = DateTime.Now.AddHours(-3);

                    short statusPedido = 0; 
                    if (pedidoIndenizacao.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 1)
                    {
                        statusPedido = 2;
                    }
                    else if (pedidoIndenizacao.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 4)
                    {
                        statusPedido = 3;
                    }

                    await _context.SaveChangesAsync();

                    //setando DHFim para ultimo motivo situação do pedido
                    Sdctb018SituacaoMotivo sdctb018SituacaoMotivoFim = await _context.Sdctb018SituacaoMotivo
                    .Where(c => c.NuSituacaoPedido == pedidoIndenizacao.NuSituacaoPedidoNavigation.NuSituacaoPedido).FirstOrDefaultAsync();

                    if (sdctb018SituacaoMotivoFim != null)
                    {
                        sdctb018SituacaoMotivoFim.DhExclusao = DateTime.Now.AddHours(-3);
                        _context.Update(sdctb018SituacaoMotivoFim);
                        await _context.SaveChangesAsync();
                    }


                    // Salvar nova situação do pedido
                    Sdctb010SituacaoPedido sdctb010SituacaoPedido = new Sdctb010SituacaoPedido()
                    {
                        NuTipoSituacaoPedido = statusPedido,
                        DhSituacao = DateTime.Now.AddHours(-3),
                        NuPedidoIndenizacao = confirmarProcesso.NuPedidoIndenizacao,
                        CoMatriculaCaixa = pedidoIndenizacao.NuSituacaoPedidoNavigation.CoMatriculaCaixa == "" ? "" : pedidoIndenizacao.NuSituacaoPedidoNavigation.CoMatriculaCaixa

                    };
                    _context.Add(sdctb010SituacaoPedido);
                    await _context.SaveChangesAsync();

                    // Busca o ID que acabou de ser incluído
                    pedidoIndenizacao.NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido;

                    // Atualiza o pedido com a nova situação
                    _context.Update(pedidoIndenizacao);
                    await _context.SaveChangesAsync();


                    // Atualiza o tipo do motivo da situação
                    Sdctb018SituacaoMotivo sdctb018SituacaoMotivo = new Sdctb018SituacaoMotivo()
                    {
                        NuSituacaoPedido = sdctb010SituacaoPedido.NuSituacaoPedido,
                        NuTipoMotivoSituacao = sdctb010SituacaoPedido.NuTipoSituacaoPedido.Equals(Convert.ToInt16(2)) ? Convert.ToInt16(1) : Convert.ToInt16(7),
                        DhSituacao = DateTime.Now.AddHours(-3)
                    };
                    _context.Add(sdctb018SituacaoMotivo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"AtualizaProcesso - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");

                    mensagemProcessoResponse.CodigoRetorno = 200;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = confirmarProcesso.NuPedidoIndenizacao.ToString(), Mensagem = "Pedido atualizado com sucesso" };
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"AtualizaProcesso - error {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = confirmarProcesso.NuPedidoIndenizacao.ToString(), Mensagem = "Dados invalidos para gravacao" };

                _logger.LogError($"AtualizaProcesso -error-  mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");

                await _context.Database.RollbackTransactionAsync();

                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        public async Task SalvarAcordos(long nuPedido)
        {
            _logger.LogInformation($"Start SalvarAcordos -  nuPedido {nuPedido}");
            try
            {
                List<Sdctb029Acordo> sdctb029Acordo = new List<Sdctb029Acordo>();
                sdctb029Acordo.Add(new Sdctb029Acordo() { NuTipoAcordo = 1, NuPedidoIndenizacao = nuPedido, DhInicio = DateTime.Now.AddHours(-3) });
                sdctb029Acordo.Add(new Sdctb029Acordo() { NuTipoAcordo = 2, NuPedidoIndenizacao = nuPedido, DhInicio = DateTime.Now.AddHours(-3) });
                sdctb029Acordo.Add(new Sdctb029Acordo() { NuTipoAcordo = 3, NuPedidoIndenizacao = nuPedido, DhInicio = DateTime.Now.AddHours(-3) });

                _context.Sdctb029Acordo.AddRange(sdctb029Acordo);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"End SalvarAcordos");
            }
            catch (Exception e)
            {
                _logger.LogError($"SalvarAcordos - error {e.Message}");
                return;
            }
        }

        public async Task<MensagemProcessoResponse> VerificaSolicitacaoIncompleta(decimal nuCpf)
        {
            _logger.LogInformation($"Start VerificaSolicitacaoIncompleta = CPF {nuCpf} ");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
            try
            {
                long nuProcesso = await _contextLeitura.Sdctb008PedidoIndenizacao.Where(p => p.NuPessoaSolicitanteNavigation.NuCpf == nuCpf
                                                                                        && p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 1).OrderByDescending(p => p.DhPedido)
                                                                            .Select(p => p.NuPedidoIndenizacao).FirstOrDefaultAsync();

                if (nuProcesso == default(long))
                {
                    mensagemProcessoResponse.CodigoRetorno = 204;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Não existem solicitações incompletas" };
                    return mensagemProcessoResponse;
                }

                mensagemProcessoResponse.CodigoRetorno = 200;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = nuProcesso.ToString(), Mensagem = "Pedido atualizado com sucesso" };

                _logger.LogInformation($"AtualizaProcesso - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.LogError($"VerificaSolicitacaoIncompleta - error {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Dados invalidos" };

                _logger.LogError($"VerificaSolicitacaoIncompleta -error-  mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        public async Task<MensagemProcessoResponse> VerificaPendenciaDocumentacao(decimal nuCpf)
        {
            _logger.LogInformation($"Start VerificaPendenciaDocumentacao = CPF {nuCpf} ");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
            try
            {
                long nuProcesso = await _context.Sdctb008PedidoIndenizacao.Where(p => p.NuPessoaSolicitanteNavigation.NuCpf == nuCpf
                                                                                        && p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 4)
                                                                            .Select(p => p.NuPedidoIndenizacao).FirstOrDefaultAsync();

                if (nuProcesso == default(long))
                {
                    mensagemProcessoResponse.CodigoRetorno = 204;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Não existem pendencias" };
                    return mensagemProcessoResponse;
                }

                mensagemProcessoResponse.CodigoRetorno = 200;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = nuProcesso.ToString(), Mensagem = "Pedido atualizado com sucesso" };

                _logger.LogInformation($"AtualizaProcesso - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.LogError($"VerificaPendenciaDocumentacao - error {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Dados invalidos" };

                _logger.LogError($"VerificaPendenciaDocumentacao -error-  mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

        public async Task<MensagemProcessoResponse> VerificaSolicitacoesEmAndamento(decimal nuCpf)
        {
            _logger.LogInformation($"Start VerificaSolicitacoesEmAndamento = CPF {nuCpf} ");

            MensagemProcessoResponse mensagemProcessoResponse = new MensagemProcessoResponse();
            try
            {
                long nuProcesso = await _contextLeitura.Sdctb008PedidoIndenizacao.Where(p => p.NuPessoaSolicitanteNavigation.NuCpf == nuCpf
                                                                                        && p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido > 1)
                                                                            .Select(p => p.NuPedidoIndenizacao).FirstOrDefaultAsync();

                if (nuProcesso == default(long))
                {
                    mensagemProcessoResponse.CodigoRetorno = 204;
                    mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Não existem solicitações em andamento" };
                    return mensagemProcessoResponse;
                }

                mensagemProcessoResponse.CodigoRetorno = 200;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = nuProcesso.ToString(), Mensagem = "Pedido atualizado com sucesso" };

                _logger.LogInformation($"AtualizaProcesso - mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.LogError($"VerificaSolicitacoesEmAndamento - error {e.Message}");

                mensagemProcessoResponse.CodigoRetorno = 400;
                mensagemProcessoResponse.MensagemRetorno = new MensagemRetornoResponse() { NumeroProcesso = null, Mensagem = "Dados invalidos" };

                _logger.LogError($"VerificaSolicitacoesEmAndamento -error-  mensagemProcessoResponse {mensagemProcessoResponse.ToJson()}");
                return mensagemProcessoResponse;
            }
            return mensagemProcessoResponse;
        }

    }
}