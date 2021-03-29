using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb011TipoSituacaoPedido
    {
        public Sdctb011TipoSituacaoPedido()
        {
            Sdctb010SituacaoPedidos = new HashSet<Sdctb010SituacaoPedido>();
        }

        public short NuTipoSituacaoPedido { get; set; }
        public string DeSituacaoPedido { get; set; }
        public string DeAbreviaturaSituacaoPedido { get; set; }
        public bool IcExibe { get; set; }
        public DateTime? DhExclusao { get; set; }
        public string DeIcone { get; set; }

        public virtual ICollection<Sdctb010SituacaoPedido> Sdctb010SituacaoPedidos { get; set; }
    }
}
