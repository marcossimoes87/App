using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class DocumentoExigidoRequest
    {
        public short NuDocumentoExigido { get; set; }
        public long NuDocumentoCapturado { get; set; }
    }
}
