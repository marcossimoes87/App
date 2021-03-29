using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb010SituacaoPedido
    {
        public Sdctb010SituacaoPedido()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb018SituacaoMotivos = new HashSet<Sdctb018SituacaoMotivo>();
            Sdctb030DocumentoPendentes = new HashSet<Sdctb030DocumentoPendente>();
        }

        public long NuSituacaoPedido { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public short NuTipoSituacaoPedido { get; set; }
        public DateTime DhSituacao { get; set; }
        public string CoMatriculaCaixa { get; set; }
        public DateTime? DhExclusao { get; set; }
        public short? NuMotivoIndeferimento { get; set; }

        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb011TipoSituacaoPedido NuTipoSituacaoPedidoNavigation { get; set; }
        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb018SituacaoMotivo> Sdctb018SituacaoMotivos { get; set; }
        public virtual ICollection<Sdctb030DocumentoPendente> Sdctb030DocumentoPendentes { get; set; }
    }
}
