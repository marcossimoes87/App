using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class DocumentosInfoResponse
    {
        public short? NuTipoDocumento { get; set; }
        public short? QtPaginas { get; set; }
        public long NuDocumentoCapturado { get; set; }
        public string DeAbreviaturaTipoDocumento { get; set; }
        public string DeUrlImagem { get; set; }
        public string DeCaminhoBlob { get; set; }
        public int NuGedamPedido { get; set; }
        public string DeTipoDocumento { get; set; }
        public int IdTipoDocumento { get; set; }
        public short NuGrupoDocumento { get; set; }
        public string DeAbreviaturaGrupoDocumento { get; set; }
        public string DeNomeArquivo { get; set; }
        public short NuDocumentoExigido { get; set; }
        public int TipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public List<Documento> DocumentosGrupo { get; set; }
        public bool? Aprovado { get; set; }
    }
}
