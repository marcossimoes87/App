using System;

namespace SeguroCaixa.DTO.Response
{
    public class DocumentoEnviadoResponse
    {
        public DateTime DataEnvio { get; set; }
        public string NomeDocumento { get; set; }
        public string NomeTipoDocumento { get; set; }
    }
}
