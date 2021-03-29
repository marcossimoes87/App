using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb035ItemDespesa
    {
        public long NuItemDespesa { get; set; }
        public long NuEmitente { get; set; }
        public int NuTipoItemDespesa { get; set; }
        public decimal VrItemDespesa { get; set; }

        public virtual Sdctb034Emitente NuEmitenteNavigation { get; set; }
        public virtual Sdctb036TipoItemDespesa NuTipoItemDespesaNavigation { get; set; }
    }
}
