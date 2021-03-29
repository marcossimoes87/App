using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb014TipoParticipacao
    {
        public Sdctb014TipoParticipacao()
        {
            Sdctb005GrupoDocumentos = new HashSet<Sdctb005GrupoDocumento>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
            Sdctb024SolicitanteIndenizacaos = new HashSet<Sdctb024SolicitanteIndenizacao>();
        }

        public short NuTipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public string DeAbreviaTipoParticipacao { get; set; }

        public virtual ICollection<Sdctb005GrupoDocumento> Sdctb005GrupoDocumentos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
        public virtual ICollection<Sdctb024SolicitanteIndenizacao> Sdctb024SolicitanteIndenizacaos { get; set; }
    }
}
