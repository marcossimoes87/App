using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosItemDespesasResponse
    {
        public long NuItemDespesa { get; set; }
        public long NuEmitente { get; set; }
        public int NuTipoItemDespesa { get; set; }
        public decimal VrItemDespesa { get; set; }

    }
}
