using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb049ItemDocumentoConteudo
    {
        public int NuItemDocumentoConteudo { get; set; }
        public long NuDocumentoCapturado { get; set; }
        public int NuItemDocumento { get; set; }
        public string DeConteudo { get; set; }

        public virtual Sdctb002DocumentoCapturado NuDocumentoCapturadoNavigation { get; set; }
        public virtual Sdctb047TipoItemDocumento NuItemDocumentoNavigation { get; set; }
    }
}
