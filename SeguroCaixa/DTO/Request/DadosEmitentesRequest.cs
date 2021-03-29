using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class DadosEmitentesRequest
    {
        public long? nuEmitente { get; set; }
        public long nuDocumentoCapturado { get; set; }
        public long nuIdentificacao { get; set; }
        public string nuRecibo { get; set; }
        public List<DadosItemDespesasRequest> itensDespesa { get; set; }
        public bool? valido { get; set; }
        public bool? novo { get; set; }
        public bool IcExcluido { get; set; } = false;
    }
}
