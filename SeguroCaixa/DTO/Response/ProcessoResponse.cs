using SeguroCaixa.DTO.Request;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeguroCaixa.DTO.Response
{
    public class StatusSitPedTipoMotivo
    {
        private EnumStatusPedido _indice;
        public EnumStatusPedido Indice
        {
            get { return _indice; }
            set
            {
                _indice = value;
                if (value == EnumStatusPedido.Disponivel)
                    descricao = "Disponivel";
                else if (value == EnumStatusPedido.Pendente)
                    descricao = "Pendente";
                else if (value == EnumStatusPedido.EmAtendimento)
                    descricao = "Em Atendimento";
                else if (value == EnumStatusPedido.PericiaMedica)
                    descricao = "Perícia Médica";
                else if (value == EnumStatusPedido.Deferido)
                    descricao = "Deferido";
                else if (value == EnumStatusPedido.Indeferido)
                    descricao = "Indeferido";
            }
        }
        public string descricao { get; set; }
    }
    public enum EnumStatusPedido : int
    {
        Disponivel = 0,
        EmAtendimento = 1,
        Pendente = 2,
        PericiaMedica = 3,
        Deferido = 4,
        CreditoEfetuado = 5,
        Indeferido = 6
    }
    public class ProcessoResponse
    {
        public long NuPedidoIndenizacao { get; set; }
        public short NuTipoIndenizacao { get; set; }
        public short NuCanal { get; set; }
        public long NuPessoaSolicitante { get; set; }
        public DateTime DhPedido { get; set; }
        public short NuMunicipio { get; set; }
        public short NuTipoVeiculo { get; set; }
        public DateTime DhSinistro { get; set; }
        public long? NuSituacaoPedido { get; set; }
        public string SgTipoLogradouro { get; set; }
        public string NoLogradouro { get; set; }
        public string CoPosicaoImovel { get; set; }
        public string DeComplemento { get; set; }
        public string NoBairro { get; set; }
        public bool? IcAutorizaCredito { get; set; }
        public bool? IcAutorizaConversao { get; set; }
        public short? NuBanco { get; set; }
        public short? NuTipoConta { get; set; }
        public short? NuAgencia { get; set; }
        public long? NuConta { get; set; }
        public string CoMatriculaCaixa { get; set; }
        public StatusSitPedTipoMotivo StatusPedido { get; set; } = new StatusSitPedTipoMotivo();
        public TipoPedidoResponse TipoPedidoResponse
        { get; set; }
        public virtual IList<ParticipacaoResponse> ParticipacoesResponse { get; set; } = new List<ParticipacaoResponse>();

    }
    public class ProcessoPaginacao
    {
        private readonly int TAMANHO_PADRAO = 25;
        private int _pagina = 0;
        private int _tamanho = 0;

        public int Pagina
        {
            get
            {
                return (_pagina <= 0) ? 1 : _pagina;
            }
            set
            {
                _pagina = value;
            }
        }
        public int Tamanho
        {
            get
            {
                return (_tamanho <= 0) ? TAMANHO_PADRAO : _tamanho;
            }
            set
            {
                _tamanho = value;
            }
        }

        public int QtdeParaDescartar => Pagina > 0 ? Tamanho * (Pagina - 1) : Tamanho;
    }

    public class ProcessoPaginado
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public IList<ProcessoResponse> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }

        public static ProcessoPaginado From(ProcessoPaginacao parametros, IQueryable<Sdctb008PedidoIndenizacao> origem)
        {
            if (parametros == null)
            {
                parametros = new ProcessoPaginacao();
            }
            int totalItens = origem.Count();
            //260 itens / 25 itens por página >> 10,4 e seu teto é 11.
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)parametros.Tamanho);
            bool temPaginaAnterior = (parametros.Pagina > 1);
            bool temProximaPagina = (parametros.Pagina < totalPaginas);

            var listaTransf = origem.Skip(parametros.QtdeParaDescartar).Take(parametros.Tamanho)
                .OrderByDescending(x => x.NuPedidoIndenizacao).ToList();
            List<ProcessoResponse> listaResposnse = new List<ProcessoResponse>();
            foreach (var item in listaTransf)
            {
                ProcessoResponse processo = new ProcessoResponse()
                {
                    NuPedidoIndenizacao = item.NuPedidoIndenizacao,
                    NuTipoIndenizacao = item.NuTipoIndenizacao,
                    NuCanal = item.NuCanal,
                    NuPessoaSolicitante = item.NuPessoaSolicitante,
                    DhPedido = item.DhPedido,
                    NuMunicipio = item.NuMunicipio,
                    NuTipoVeiculo = item.NuTipoVeiculo,
                    DhSinistro = item.DhSinistro,
                    NuSituacaoPedido = item.NuSituacaoPedido,
                    SgTipoLogradouro = item.SgTipoLogradouro,
                    NoLogradouro = item.NoLogradouro,
                    CoPosicaoImovel = item.CoPosicaoImovel,
                    DeComplemento = item.DeComplemento,
                    NoBairro = item.NoBairro,
                    IcAutorizaCredito = item.IcAutorizaCredito,
                    IcAutorizaConversao = item.IcAutorizaConversao,
                    NuBanco = item.NuBanco,
                    NuTipoConta = item.NuTipoConta,
                    NuAgencia = item.NuAgencia,
                    NuConta = item.NuConta,
                    CoMatriculaCaixa = item.NuSituacaoPedidoNavigation.CoMatriculaCaixa,
                    TipoPedidoResponse = new TipoPedidoResponse()
                    {
                        Id = item.NuTipoIndenizacao,
                        Descricao = item.NuTipoIndenizacaoNavigation.DeTipoIndenizacao,
                        Titulo = item.NuTipoIndenizacaoNavigation.DeAbreviaturaTipoIndenizaca
                    }
                };

                foreach (var participacao in item.Sdctb015Participacaos)
                {

                    processo.ParticipacoesResponse.Add(new ParticipacaoResponse()
                    {
                        NuTipoParticipacao = participacao.NuTipoParticipacao,
                        PessoaResponse = new PessoaResponse()
                        {
                            NuCpf = participacao.NuPessoaParticipanteNavigation.NuCpf
                        }
                    });

                }

                SetarStatusPedido(item, processo);

                listaResposnse.Add(processo);
            }
            return new ProcessoPaginado
            {
                Total = totalItens,
                TotalPaginas = totalPaginas,
                TamanhoPagina = parametros.Tamanho,
                NumeroPagina = parametros.Pagina,
                Resultado = listaResposnse,
                Anterior = temPaginaAnterior
                    ? $"processo/v1/ListarProcessos?pagina={parametros.Pagina - 1}&tamanho={parametros.Tamanho}"
                    : "",
                Proximo = temProximaPagina
                    ? $"processo/v1/ListarProcessos?pagina={parametros.Pagina + 1}&tamanho={parametros.Tamanho}"
                    : ""
            };
        }

        public static void SetarStatusPedido(Sdctb008PedidoIndenizacao item, ProcessoResponse processo)
        {

            long idSitPed = (long)item.NuSituacaoPedido;
            var listaMotivos = item.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Where(x => x.NuSituacaoPedido == idSitPed).ToList();
            long nuTipoSituacaoPedido = item.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido;

            if (nuTipoSituacaoPedido == 2 && listaMotivos.Any(x => new List<long> { 1 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.Disponivel };
            }
            else if (nuTipoSituacaoPedido == 3 && listaMotivos.Any(x => new List<long> { 7 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.EmAtendimento };
            }
            else if (new List<long> { 1, 4 }.Contains(nuTipoSituacaoPedido) && listaMotivos.Any(x => new List<long> { 4, 9 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.Pendente };
            }
            else if (nuTipoSituacaoPedido == 3 && listaMotivos.Any(x => new List<long> { 8 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.PericiaMedica };
            }
            else if (nuTipoSituacaoPedido == 5 && listaMotivos.Any(x => new List<long> { 3 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.Deferido };
            }
            else if (nuTipoSituacaoPedido == 8 && listaMotivos.Any(x => new List<long> { 5, 6 }.Contains(x.NuTipoMotivoSituacao)))
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.CreditoEfetuado };
            }
            else if (nuTipoSituacaoPedido == 6)
            {
                processo.StatusPedido = new StatusSitPedTipoMotivo() { Indice = EnumStatusPedido.Indeferido };
            }
        }
    }
}