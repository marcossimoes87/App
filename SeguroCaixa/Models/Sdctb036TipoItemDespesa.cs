using System;
using System.Collections.Generic;

namespace SeguroCaixa.Models
{
    public partial class Sdctb036TipoItemDespesa
    {
        public Sdctb036TipoItemDespesa()
        {
            Sdctb035ItemDespesas = new HashSet<Sdctb035ItemDespesa>();
        }

        public int NuTipoItemDespesa { get; set; }
        public string DeTipoItemDespesa { get; set; }

        public virtual ICollection<Sdctb035ItemDespesa> Sdctb035ItemDespesas { get; set; }
    }
}
