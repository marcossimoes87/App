using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{

    public class MotivoIndeferimentoResponse
    {
        public short NuMotivoIndeferimento { get; set; }
        public short NuTipoIndeferimento { get; set; }
        public string DeMotivoIndeferimento { get; set; }
    }
}
