using Microsoft.AspNetCore.Http;

namespace SeguroCaixa.DTO.Request
{
    public class ListaDocumentoRequest
    {
        public short IdTipoPedido { get; set; }
        public short IdTipoParticipacao { get; set; }
    }
}
