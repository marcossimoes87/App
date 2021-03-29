using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class PendenciaDocumentoResponse
    {
        public long IdPedido { get; set; }
        public string DePedido { get; set; }

        public List<Documento> Documentos { get; set; }
    }
}
