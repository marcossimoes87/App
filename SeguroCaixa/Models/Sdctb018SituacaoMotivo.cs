using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb018SituacaoMotivo
    {
        public Sdctb018SituacaoMotivo()
        {
            Sdctb027Notificacaos = new HashSet<Sdctb027Notificacao>();
        }

        public long NuSituacaoMotivo { get; set; }
        public long NuSituacaoPedido { get; set; }
        public short NuTipoMotivoSituacao { get; set; }
        public DateTime DhSituacao { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual Sdctb010SituacaoPedido NuSituacaoPedidoNavigation { get; set; }
        public virtual Sdctb017TipoMotivoSituacao NuTipoMotivoSituacaoNavigation { get; set; }
        public virtual ICollection<Sdctb027Notificacao> Sdctb027Notificacaos { get; set; }
    }
}
