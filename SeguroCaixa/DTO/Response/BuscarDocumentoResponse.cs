using System.IO;

namespace SeguroCaixa.DTO.Response
{
    public class BuscarDocumentoResponse
    {
        //
        // Summary:
        //     The number of bytes present in the response body.
        public long ContentLength { get; set; }
        //
        // Summary:
        //     Content
        public Stream Content { get; set; }
        //
        // Summary:
        //     The media type of the body of the response. For Download Blob this is 'application/octet-stream'
        public string ContentType { get; set; }
    }
}
