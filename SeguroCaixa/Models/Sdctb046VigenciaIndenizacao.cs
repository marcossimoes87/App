using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb046VigenciaIndenizacao
    {
        public int NuVigenciaIndenizacao { get; set; }
        public short NuTipoIndenizacao { get; set; }
        public decimal VrLimiteIndenizacao { get; set; }
        public DateTime DtInicioVigencia { get; set; }
        public DateTime? DtFimVigencia { get; set; }

        public virtual Sdctb007TipoIndenizacao NuTipoIndenizacaoNavigation { get; set; }
    }
}
