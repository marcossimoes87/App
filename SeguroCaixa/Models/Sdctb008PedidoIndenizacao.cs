using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb008PedidoIndenizacao
    {
        public Sdctb008PedidoIndenizacao()
        {
            Sdctb002DocumentoCapturados = new HashSet<Sdctb002DocumentoCapturado>();
            Sdctb010SituacaoPedidos = new HashSet<Sdctb010SituacaoPedido>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
            Sdctb019ValorIndenizacaos = new HashSet<Sdctb019ValorIndenizacao>();
            Sdctb023DeclaraHerdeiros = new HashSet<Sdctb023DeclaraHerdeiro>();
            Sdctb026DeclaraHerdeiroComplementos = new HashSet<Sdctb026DeclaraHerdeiroComplemento>();
            Sdctb029Acordos = new HashSet<Sdctb029Acordo>();
            Sdctb054GedamPedidoIndenizacaos = new HashSet<Sdctb054GedamPedidoIndenizacao>();
        }

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

        public virtual Sdctb016Canal NuCanalNavigation { get; set; }
        public virtual Sdctb001Municipio NuMunicipioNavigation { get; set; }
        public virtual Sdctb009Pessoa NuPessoaSolicitanteNavigation { get; set; }
        public virtual Sdctb010SituacaoPedido NuSituacaoPedidoNavigation { get; set; }
        public virtual Sdctb007TipoIndenizacao NuTipoIndenizacaoNavigation { get; set; }
        public virtual Sdctb012TipoVeiculo NuTipoVeiculoNavigation { get; set; }
        public virtual Sdctb004TipoLogradouro SgTipoLogradouroNavigation { get; set; }
        public virtual ICollection<Sdctb002DocumentoCapturado> Sdctb002DocumentoCapturados { get; set; }
        public virtual ICollection<Sdctb010SituacaoPedido> Sdctb010SituacaoPedidos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
        public virtual ICollection<Sdctb019ValorIndenizacao> Sdctb019ValorIndenizacaos { get; set; }
        public virtual ICollection<Sdctb023DeclaraHerdeiro> Sdctb023DeclaraHerdeiros { get; set; }
        public virtual ICollection<Sdctb026DeclaraHerdeiroComplemento> Sdctb026DeclaraHerdeiroComplementos { get; set; }
        public virtual ICollection<Sdctb029Acordo> Sdctb029Acordos { get; set; }
        public virtual ICollection<Sdctb054GedamPedidoIndenizacao> Sdctb054GedamPedidoIndenizacaos { get; set; }
    }
}
