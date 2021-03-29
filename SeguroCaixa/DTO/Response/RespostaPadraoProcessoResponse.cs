using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class RespostaPadraoProcessoResponse
    {

        public List<TipoIndeferimento> tipoIndeferimento { get; set; }
        public List<MotivoIndeferimento> motivoIndeferimento{ get; set; }
        public class TipoIndeferimento
        {
            public short NuTipoIndeferimento { get; set; }
            public string DeTipoIndeferimento { get; set; }
        }

        public class MotivoIndeferimento
        {
            public short NuMotivoIndeferimento { get; set; }
            public string DeMotivoIndeferimento { get; set; }
        }

    }
}
