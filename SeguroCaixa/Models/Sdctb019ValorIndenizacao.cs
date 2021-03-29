using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb019ValorIndenizacao
    {
        public long NuValorIndenizacao { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public short NuTipoIndenizacaoPgto { get; set; }
        public decimal VrIndenizacao { get; set; }
        public DateTime DtPrevistaCredito { get; set; }
        public DateTime? DtEfetivaCredito { get; set; }
        public DateTime? DhExclusao { get; set; }
        public string DeObservacao { get; set; }

        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb007TipoIndenizacao NuTipoIndenizacaoPgtoNavigation { get; set; }
    }
}
