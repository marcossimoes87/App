using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb024SolicitanteIndenizacao
    {
        public short NuSolicitanteIndenizacao { get; set; }
        [Required]
        public short NuTipoIndenizacao { get; set; }
        [Required]
        public short NuTipoParticipacao { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual Sdctb007TipoIndenizacao NuTipoIndenizacaoNavigation { get; set; }
        public virtual Sdctb014TipoParticipacao NuTipoParticipacaoNavigation { get; set; }
    }
}
