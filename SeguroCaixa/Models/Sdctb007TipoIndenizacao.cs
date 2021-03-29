using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb007TipoIndenizacao
    {
        public Sdctb007TipoIndenizacao()
        {
            Sdctb005GrupoDocumentos = new HashSet<Sdctb005GrupoDocumento>();
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb024SolicitanteIndenizacaos = new HashSet<Sdctb024SolicitanteIndenizacao>();
        }

        public short NuTipoIndenizacao { get; set; }
        public string DeTipoIndenizacao { get; set; }
        public string DeAbreviaturaTipoIndenizaca { get; set; }

        public virtual ICollection<Sdctb005GrupoDocumento> Sdctb005GrupoDocumentos { get; set; }
        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb019ValorIndenizacao> Sdctb019ValorIndenizacaos { get; set; }
        public virtual ICollection<Sdctb024SolicitanteIndenizacao> Sdctb024SolicitanteIndenizacaos { get; set; }

        public virtual ICollection<Sdctb046VigenciaIndenizacao> Sdctb046VigenciaIndenizacaos { get; set; }
    }
}
