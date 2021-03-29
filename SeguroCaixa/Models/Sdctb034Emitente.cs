using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb034Emitente
    {
        public Sdctb034Emitente()
        {
            Sdctb035ItemDespesas = new HashSet<Sdctb035ItemDespesa>();
        }

        public long NuEmitente { get; set; }
        public long NuDocumentoCapturado { get; set; }
        public long NuIdentificacao { get; set; }
        public string NuRecibo { get; set; }

        public virtual Sdctb002DocumentoCapturado NuDocumentoCapturadoNavigation { get; set; }
        public virtual ICollection<Sdctb035ItemDespesa> Sdctb035ItemDespesas { get; set; }
    }
}
