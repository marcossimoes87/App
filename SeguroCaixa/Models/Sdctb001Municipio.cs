using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb001Municipio
    {
        public Sdctb001Municipio()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuMunicipio { get; set; }
        public string DeMunicipio { get; set; }
        public decimal CoIbge { get; set; }
        public string CoUf { get; set; }

        public virtual Sdctb013Uf CoUfNavigation { get; set; }
        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
