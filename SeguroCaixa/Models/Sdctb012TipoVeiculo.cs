using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb012TipoVeiculo
    {
        public Sdctb012TipoVeiculo()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuTipoVeiculo { get; set; }
        public string DeTipoVeiculo { get; set; }
        public string DeAbreviaturaTipoVeiculo { get; set; }

        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
