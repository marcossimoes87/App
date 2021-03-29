using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class MensagemUploadResponse
    {
        public int CodigoRetorno { get; set; }
        public List<RespostaDocumentoUpload> Documentos { get; set; }
    }

    public class RespostaDocumentoUpload
    {
        public long IdDocumentoCapturado { get; set; }
        public string UrlDocumento { get; set; }
    }

}
