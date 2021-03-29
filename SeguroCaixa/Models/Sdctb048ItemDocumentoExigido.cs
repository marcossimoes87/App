using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb048ItemDocumentoExigido
    {
        public int NuItemDocumentoExigido { get; set; }
        public short NuTipoDocumento { get; set; }
        public int NuItemDocumento { get; set; }

        public virtual Sdctb047TipoItemDocumento NuItemDocumentoNavigation { get; set; }
    }
}
