using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb016Canal
    {
        public Sdctb016Canal()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
        }

        public short NuCanal { get; set; }
        public string DeCanal { get; set; }

        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
    }
}
