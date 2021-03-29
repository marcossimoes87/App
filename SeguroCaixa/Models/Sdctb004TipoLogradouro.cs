using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb004TipoLogradouro
    {
        public Sdctb004TipoLogradouro()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public string SgTipoLogradouro { get; set; }
        public string NoTipoLogradouro { get; set; }

        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
