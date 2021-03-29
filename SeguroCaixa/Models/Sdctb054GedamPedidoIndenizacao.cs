using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb054GedamPedidoIndenizacao
    {
        public int NuGedamPedidoIndenizacao { get; set; }
        public int NuGedamPedido { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public long NuCpf { get; set; }
        public DateTime DhPedido { get; set; }

        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
    }
}
