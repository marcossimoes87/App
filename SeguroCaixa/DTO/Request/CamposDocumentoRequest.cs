using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class CamposDocumentoRequest
    {
        public long nuDocumentoCapturado { get; set; }
        public List<ComposConteudo> camposDocumento { get; set; }
        public class ComposConteudo
        {
            public int? nuItemDocumentoConteudo { get; set; }
            public string titulo { get; set; }
            public string conteudo { get; set; }
            public int nuItemConteudo { get; set; }
        }

    }
}
