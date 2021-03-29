using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class DocumentoConfirmacaoProcessoRequest
    {
        public long nuDocumentoCapturado { get; set; }
        public bool? fgDocumentoAprovado { get; set; }
    }
}
