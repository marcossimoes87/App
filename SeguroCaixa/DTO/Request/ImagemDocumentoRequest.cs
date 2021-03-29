using Microsoft.AspNetCore.Http;

namespace SeguroCaixa.DTO.Request
{
    public class ImagemDocumentoRequest
    {
        public int NumeroProcesso { get; set; }
        public short IdDocumentoExigido { get; set; }
        public string ExtensaoDocumento { get; set; }
        public IFormFileCollection Documento { get; set; }
        public string NomeDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public short? Pagina { get; set; }
    }

    public class ImagemDocumentoAnexo
    {
        public long IdDocumentoCapturado { get; set; }
        public string ExtensaoDocumento { get; set; }
        public string NomeDocumento { get; set; }
        public string UrlDocumento { get; set; }
    }
}
