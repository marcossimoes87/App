using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosEmitentesResponse
    {
        public long NuEmitente { get; set; }
        public long NuDocumentoCapturado { get; set; }
        public long NuIdentificacao { get; set; }
        public string NuRecibo { get; set; }
        public List<DadosItemDespesasResponse> ItensDespesa { get; set; }

    }
}
