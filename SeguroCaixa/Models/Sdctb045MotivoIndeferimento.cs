using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb045MotivoIndeferimento
    {
        public short NuMotivoIndeferimento { get; set; }
        public short NuTipoIndeferimento { get; set; }
        public string DeMotivoIndeferimento { get; set; }

        public virtual Sdctb044TipoIndeferimento NuTipoIndeferimentoNavigation { get; set; }

    }
}
