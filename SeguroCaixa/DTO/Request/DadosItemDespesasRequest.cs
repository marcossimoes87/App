using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class DadosItemDespesasRequest
    {
        public long? nuItemDespesa { get; set; }
        public long? nuEmitente { get; set; }
        public int nuTipoItemDespesa { get; set; }
        public decimal vrItemDespesa { get; set; }
        public bool? valido { get; set; }
        public bool? novo { get; set; }

        public bool IcExcluido { get; set; } = false;
    }
}
